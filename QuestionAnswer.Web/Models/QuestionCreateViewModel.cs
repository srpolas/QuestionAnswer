using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuestionAnswer.Web.Models;

public class QuestionCreateViewModel
{
    public int QuestionId { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Tags")]
    public List<int> SelectedTagIds { get; set; } = new();

    [Display(Name = "New Tags (comma separated)")]
    public string? NewTags { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Tags { get; set; } = Enumerable.Empty<SelectListItem>();
}

