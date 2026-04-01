using QuestionAnswer.Contract;

namespace QuestionAnswer.BLL.Interfaces;

public interface ITagService
{
    Task<IReadOnlyList<TagDto>> GetAllAsync();
}
