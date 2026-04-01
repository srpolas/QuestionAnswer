# QuestionAnswer - Forum Application

A robust ASP.NET Core Q&A application built with an n-tier architecture, featuring full CRUD operations for questions, answers, and comments.

## 🚀 Features

- **N-Tier Architecture**: Separated into Web, Business Logic (BLL), Data Access (DAL), Domain, and Contract layers.
- **Full CRUD Support**: Create, Read, Update, and Delete for Questions, Answers, and Comments.
- **Accepted Answer**: Question authors can mark an answer as "Accepted".
- **Advanced Seeding**: Automatically populates categories, tags, users, and sample questions on first run.
- **Secure Identity**: Integrated ASP.NET Core Identity with specific authorization for content authors.
- **Modern UI**: Clean, responsive design using Bootstrap 5 with custom CSS gradients and professional styling.

## 🛠️ Technology Stack

- **Backend**: .NET 8.0 / C# 12
- **Database**: SQL Server / Entity Framework Core 8.0
- **Frontend**: ASP.NET Core MVC / Bootstrap 5 / jQuery
- **Authentication**: ASP.NET Core Identity

## 📦 Project Structure

- `QuestionAnswer.Web`: MVC Application (Controllers, Views, ViewModels)
- `QuestionAnswer.BLL`: Business logic and service implementations
- `QuestionAnswer.DAL`: Data access, repositories, and DbContext
- `QuestionAnswer.Domain`: Core entities
- `QuestionAnswer.Contract`: Data Transfer Objects (DTOs) and common interfaces

## ⚙️ Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or better)

### Setup

1.  Clone the repository:
    ```bash
    git clone https://github.com/srpolas/QuestionAnswer.git
    ```
2.  Open the solution in Visual Studio or VS Code.
3.  Update the connection string in `appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=QuestionAnswerDb;..."
    }
    ```
4.  Run the application:
    ```bash
    dotnet run --project QuestionAnswer.Web
    ```

### Default Credentials

- **Admin Account**:
  - Email: `admin@ostadforum.com`
  - Password: `Admin@123`
- **Sample User Account**:
  - Email: `john@example.com`
  - Password: `User@123`

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License.
