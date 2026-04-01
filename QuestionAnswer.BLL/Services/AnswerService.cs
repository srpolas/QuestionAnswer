using Microsoft.EntityFrameworkCore;
using QuestionAnswer.BLL.Interfaces;
using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.BLL.Services;

public class AnswerService(IUnitOfWork unitOfWork) : IAnswerService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task AddAnswerAsync(int questionId, string userEmail, string content)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email is required.", nameof(userEmail));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        var users = await _unitOfWork.Users.FindAsync(u => u.Email == userEmail);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            user = new User
            {
                Name = userEmail,
                Email = userEmail,
                PasswordHash = string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        var answer = new Answer
        {
            QuestionId = questionId,
            UserId = user.UserId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsAccepted = false
        };
        await _unitOfWork.Answers.AddAsync(answer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAnswerAsync(int answerId, string userEmail, string content)
    {
        var answer = await _unitOfWork.Answers.Query()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnswerId == answerId);

        if (answer == null) throw new KeyNotFoundException();
        if (answer.User.Email != userEmail) throw new UnauthorizedAccessException();

        answer.Content = content.Trim();
        _unitOfWork.Answers.Update(answer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAnswerAsync(int answerId, string userEmail)
    {
        var answer = await _unitOfWork.Answers.Query()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AnswerId == answerId);

        if (answer == null) throw new KeyNotFoundException();
        if (answer.User.Email != userEmail) throw new UnauthorizedAccessException();

        _unitOfWork.Answers.Remove(answer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AcceptAnswerAsync(int answerId, string userEmail)
    {
        var answer = await _unitOfWork.Answers.Query()
            .Include(a => a.Question).ThenInclude(q => q.User)
            .FirstOrDefaultAsync(a => a.AnswerId == answerId);

        if (answer == null) throw new KeyNotFoundException();
        
        // Only the question author can accept an answer
        if (answer.Question.User.Email != userEmail) throw new UnauthorizedAccessException();

        // Optional: Unaccept other answers for the same question if we only allow one accepted answer
        var otherAnswers = await _unitOfWork.Answers.FindAsync(a => a.QuestionId == answer.QuestionId && a.IsAccepted);
        foreach (var oa in otherAnswers)
        {
            oa.IsAccepted = false;
            _unitOfWork.Answers.Update(oa);
        }

        answer.IsAccepted = true;
        _unitOfWork.Answers.Update(answer);
        await _unitOfWork.SaveChangesAsync();
    }
}
