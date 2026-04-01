using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class QuestionTagRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<QuestionTag>(dbContext), IQuestionTagRepository
{
}

