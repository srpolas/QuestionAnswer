using QuestionAnswer.Contract;

namespace QuestionAnswer.BLL.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync();
    Task CreateAsync(CreateCategoryDto dto);
}
