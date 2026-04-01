namespace QuestionAnswer.BLL.Interfaces;

public interface IAnswerService
{
    Task AddAnswerAsync(int questionId, string userEmail, string content);
    Task UpdateAnswerAsync(int answerId, string userEmail, string content);
    Task DeleteAnswerAsync(int answerId, string userEmail);
    Task AcceptAnswerAsync(int answerId, string userEmail);
}
