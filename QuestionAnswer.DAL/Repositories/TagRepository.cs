using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class TagRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<Tag>(dbContext), ITagRepository
{
}

