using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class AnswerRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<Answer>(dbContext), IAnswerRepository
{
}

