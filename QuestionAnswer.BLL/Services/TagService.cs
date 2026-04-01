using QuestionAnswer.BLL.Interfaces;
using QuestionAnswer.Contract;
using QuestionAnswer.DAL.Interfaces;

namespace QuestionAnswer.BLL.Services;

public class TagService(IUnitOfWork unitOfWork) : ITagService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<TagDto>> GetAllAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();
        return tags.Select(t => new TagDto
        {
            TagId = t.TagId,
            Name = t.Name,
            Slug = t.Slug
        }).ToList();
    }
}
