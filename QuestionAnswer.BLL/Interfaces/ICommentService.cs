using QuestionAnswer.Contract;

namespace QuestionAnswer.BLL.Interfaces;

public interface ICommentService
{
    Task AddCommentAsync(int answerId, string userEmail, string content, int? parentCommentId = null);
    Task<IReadOnlyList<CommentTreeDto>> GetCommentsTreeForAnswerAsync(int answerId);
    Task UpdateCommentAsync(int commentId, string userEmail, string content);
    Task DeleteCommentAsync(int commentId, string userEmail);
}
