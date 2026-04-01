using Microsoft.EntityFrameworkCore;
using QuestionAnswer.BLL.Interfaces;
using QuestionAnswer.Contract;
using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.BLL.Services;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task AddCommentAsync(int answerId, string userEmail, string content, int? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email is required.", nameof(userEmail));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        var user = await ResolveUserAsync(userEmail);

        var comment = new Comment
        {
            AnswerId = answerId,
            UserId = user.UserId,
            ParentCommentId = parentCommentId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<CommentTreeDto>> GetCommentsTreeForAnswerAsync(int answerId)
    {
        var comments = await _unitOfWork.Comments
            .Query()
            .Include(c => c.User)
            .Where(c => c.AnswerId == answerId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        var byParent = comments
            .GroupBy(c => c.ParentCommentId ?? -1)
            .ToDictionary(g => g.Key, g => g.ToList());
        return BuildTree(byParent, -1);

        static List<CommentTreeDto> BuildTree(Dictionary<int, List<Comment>> byParent, int parentKey)
        {
            if (!byParent.TryGetValue(parentKey, out var list))
                return new List<CommentTreeDto>();

            return list.Select(c => new CommentTreeDto
            {
                CommentId = c.CommentId,
                Content = c.Content,
                AuthorName = c.User?.Email ?? "",
                CreatedAt = c.CreatedAt,
                Replies = BuildTree(byParent, c.CommentId)
            }).ToList();
        }
    }

    public async Task UpdateCommentAsync(int commentId, string userEmail, string content)
    {
        var comment = await _unitOfWork.Comments.Query()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment == null) throw new KeyNotFoundException();
        if (comment.User.Email != userEmail) throw new UnauthorizedAccessException();

        comment.Content = content.Trim();
        _unitOfWork.Comments.Update(comment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId, string userEmail)
    {
        var comment = await _unitOfWork.Comments.Query()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment == null) throw new KeyNotFoundException();
        if (comment.User.Email != userEmail) throw new UnauthorizedAccessException();

        _unitOfWork.Comments.Remove(comment);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<User> ResolveUserAsync(string userEmail)
    {
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
        return user;
    }
}
