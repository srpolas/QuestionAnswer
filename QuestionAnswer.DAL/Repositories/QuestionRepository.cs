using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class QuestionRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<Question>(dbContext), IQuestionRepository
{
}

