using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class CategoryRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<Category>(dbContext), ICategoryRepository
{
}

