using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class CommentRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<Comment>(dbContext), ICommentRepository
{
}

