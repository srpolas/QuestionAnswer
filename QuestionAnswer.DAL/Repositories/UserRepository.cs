using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class UserRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<User>(dbContext), IUserRepository
{
}

