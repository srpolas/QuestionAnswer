using QuestionAnswer.DAL.Interfaces;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.DAL.Repositories;

public class VoteRepository(QuestionAnswerDbContext dbContext)
    : GenericRepository<Vote>(dbContext), IVoteRepository
{
}

