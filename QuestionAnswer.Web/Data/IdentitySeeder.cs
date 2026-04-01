using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestionAnswer.DAL;
using QuestionAnswer.Domain.Entities;

namespace QuestionAnswer.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var db = scope.ServiceProvider.GetRequiredService<QuestionAnswerDbContext>();

        // 1. Roles
        const string adminRole = "Admin";
        const string userRole = "User";
        if (!await roleManager.RoleExistsAsync(adminRole)) await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (!await roleManager.RoleExistsAsync(userRole)) await roleManager.CreateAsync(new IdentityRole(userRole));

        // 2. Admin User
        const string adminEmail = "admin@ostadforum.com";
        const string adminPassword = "Admin@123";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, adminRole);
            
            // Sync with Domain User
            if (!await db.Users.AnyAsync(u => u.Email == adminEmail))
            {
                db.Users.Add(new User { Name = "Administrator", Email = adminEmail, PasswordHash = "...", CreatedAt = DateTime.UtcNow });
                await db.SaveChangesAsync();
            }
        }

        // 3. Regular Users
        var seedUsers = new[]
        {
            new { Name = "John Doe", Email = "john@example.com" },
            new { Name = "Jane Smith", Email = "jane@example.com" },
            new { Name = "Dev Master", Email = "dev@example.com" }
        };

        foreach (var uData in seedUsers)
        {
            if (await userManager.FindByEmailAsync(uData.Email) == null)
            {
                var u = new IdentityUser { UserName = uData.Email, Email = uData.Email, EmailConfirmed = true };
                await userManager.CreateAsync(u, "User@123");
                await userManager.AddToRoleAsync(u, userRole);

                if (!await db.Users.AnyAsync(u => u.Email == uData.Email))
                {
                    db.Users.Add(new User { Name = uData.Name, Email = uData.Email, PasswordHash = "...", CreatedAt = DateTime.UtcNow });
                }
            }
        }
        await db.SaveChangesAsync();

        // 4. Categories
        var categories = new[]
        {
            new { Name = "C# & .NET", Slug = "csharp-dotnet", Desc = "Questions about C#, ASP.NET Core, and the .NET ecosystem." },
            new { Name = "Web Development", Slug = "web-dev", Desc = "Frontend and Backend web development topics." },
            new { Name = "Databases", Slug = "databases", Desc = "SQL Server, PostgreSQL, MongoDB and more." },
            new { Name = "DevOps", Slug = "devops", Desc = "Docker, CI/CD, Azure, and AWS." }
        };

        foreach (var cat in categories)
        {
            if (!await db.Categories.AnyAsync(c => c.Slug == cat.Slug))
            {
                db.Categories.Add(new Category { Name = cat.Name, Slug = cat.Slug, Description = cat.Desc, CreatedAt = DateTime.UtcNow });
            }
        }
        await db.SaveChangesAsync();

        // 5. Tags
        var tags = new[] { "aspnetcore", "efcore", "javascript", "react", "sql-server", "docker", "testing" };
        foreach (var tagName in tags)
        {
            if (!await db.Tags.AnyAsync(t => t.Name == tagName))
            {
                db.Tags.Add(new Tag { Name = tagName, Slug = tagName, CreatedAt = DateTime.UtcNow });
            }
        }
        await db.SaveChangesAsync();

        // 6. Questions & Answers (Partial check)
        if (!await db.Questions.AnyAsync())
        {
            var john = await db.Users.FirstAsync(u => u.Email == "john@example.com");
            var jane = await db.Users.FirstAsync(u => u.Email == "jane@example.com");
            var catNet = await db.Categories.FirstAsync(c => c.Slug == "csharp-dotnet");
            var catDb = await db.Categories.FirstAsync(c => c.Slug == "databases");
            var tagEf = await db.Tags.FirstAsync(t => t.Name == "efcore");
            var tagSql = await db.Tags.FirstAsync(t => t.Name == "sql-server");

            // Q1
            var q1 = new Question
            {
                Title = "How to optimize EF Core migrations?",
                Description = "I have a large project and migrations are getting slow. Any tips for splitting them or optimizing the process?",
                CategoryId = catNet.CategoryId,
                UserId = john.UserId,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                ViewCount = 150
            };
            db.Questions.Add(q1);
            await db.SaveChangesAsync();
            db.QuestionTags.Add(new QuestionTag { QuestionId = q1.QuestionId, TagId = tagEf.TagId });

            var a1 = new Answer
            {
                QuestionId = q1.QuestionId,
                UserId = jane.UserId,
                Content = "You should consider splitting your DbContext if it grows too large, or manually pruning old migrations that are already applied in all environments.",
                IsAccepted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };
            db.Answers.Add(a1);
            await db.SaveChangesAsync();

            db.Comments.Add(new Comment { AnswerId = a1.AnswerId, UserId = john.UserId, Content = "Great advice, thanks!", CreatedAt = DateTime.UtcNow });

            // Q2
            var q2 = new Question
            {
                Title = "SQL Server Deadlock issues in high traffic app",
                Description = "Our application is experiencing frequent deadlocks during peak hours. We are using Serializable isolation level. Should we change it?",
                CategoryId = catDb.CategoryId,
                UserId = jane.UserId,
                CreatedAt = DateTime.UtcNow.AddHours(-5),
                ViewCount = 45
            };
            db.Questions.Add(q2);
            await db.SaveChangesAsync();
            db.QuestionTags.Add(new QuestionTag { QuestionId = q2.QuestionId, TagId = tagSql.TagId });

            db.Answers.Add(new Answer { QuestionId = q2.QuestionId, UserId = john.UserId, Content = "Serializable is very strict. Try using Read Committed Snapshot Isolation (RCSI) instead.", CreatedAt = DateTime.UtcNow.AddHours(-2) });

            await db.SaveChangesAsync();
        }
    }
}
