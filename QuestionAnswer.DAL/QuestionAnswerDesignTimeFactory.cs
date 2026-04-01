using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuestionAnswer.DAL;

public class QuestionAnswerDesignTimeFactory : IDesignTimeDbContextFactory<QuestionAnswerDbContext>
{
    public QuestionAnswerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<QuestionAnswerDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=OstadForumDb;Trusted_Connection=True;TrustServerCertificate=True;");
        return new QuestionAnswerDbContext(optionsBuilder.Options);
    }
}

