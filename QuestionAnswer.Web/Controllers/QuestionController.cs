using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestionAnswer.BLL.Interfaces;
using QuestionAnswer.Web.Models;

namespace QuestionAnswer.Web.Controllers;

public class QuestionController(
    IQuestionService questionService,
    IAnswerService answerService,
    IVoteService voteService,
    ICommentService commentService) : Controller
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IAnswerService _answerService = answerService;
    private readonly IVoteService _voteService = voteService;
    private readonly ICommentService _commentService = commentService;

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _questionService.GetQuestionDetailsAsync(id);
        if (dto == null)
            return NotFound();

        var model = new QuestionDetailsViewModel
        {
            QuestionId = dto.QuestionId,
            Title = dto.Title,
            Description = dto.Description,
            CategoryName = dto.CategoryName,
            AuthorName = dto.AuthorName,
            CreatedAt = dto.CreatedAt,
            ViewCount = dto.ViewCount,
            QuestionScore = dto.QuestionScore,
            TagNames = dto.TagNames,
            IsAccepted = dto.Answers.Any(a => a.IsAccepted),
            Answers = new List<AnswerViewModel>()
        };

        foreach (var a in dto.Answers)
        {
            var comments = await _commentService.GetCommentsTreeForAnswerAsync(a.AnswerId);
            model.Answers.Add(new AnswerViewModel
            {
                AnswerId = a.AnswerId,
                Content = a.Content,
                AuthorName = a.AuthorName,
                CreatedAt = a.CreatedAt,
                Score = a.Score,
                IsAccepted = a.IsAccepted,
                Comments = comments.ToList()
            });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddAnswer(AnswerInputModel model)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Challenge();

        if (string.IsNullOrWhiteSpace(model?.Content))
        {
            TempData["AnswerError"] = "Answer content is required.";
            return RedirectToAction(nameof(Details), new { id = model!.QuestionId });
        }

        await _answerService.AddAnswerAsync(model.QuestionId, email, model.Content);
        return RedirectToAction(nameof(Details), new { id = model.QuestionId });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var dto = await _questionService.GetCreatePageDataAsync();
        var model = new QuestionCreateViewModel
        {
            Categories = dto.Categories.Select(c => new SelectListItem { Value = c.Value, Text = c.Text }),
            Tags = dto.Tags.Select(t => new SelectListItem { Value = t.Value, Text = t.Text })
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(QuestionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var dto = await _questionService.GetCreatePageDataAsync();
            model.Categories = dto.Categories.Select(c => new SelectListItem { Value = c.Value, Text = c.Text });
            model.Tags = dto.Tags.Select(t => new SelectListItem { Value = t.Value, Text = t.Text });
            return View(model);
        }

        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Challenge();

        var newTagNames = (model.NewTags ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim());

        await _questionService.CreateQuestionAsync(
            email,
            model.Title,
            model.Description,
            model.CategoryId,
            model.SelectedTagIds ?? new List<int>(),
            newTagNames);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        var dto = await _questionService.GetQuestionForEditAsync(id, email);
        if (dto == null) return NotFound();

        var createData = await _questionService.GetCreatePageDataAsync();
        var model = new QuestionCreateViewModel
        {
            QuestionId = dto.QuestionId,
            Title = dto.Title,
            Description = dto.Description,
            Categories = createData.Categories.Select(c => new SelectListItem { Value = c.Value, Text = c.Text }),
            Tags = createData.Tags.Select(t => new SelectListItem { Value = t.Value, Text = t.Text }),
            SelectedTagIds = dto.TagNames.Select(tn => int.Parse(createData.Tags.First(t => t.Text == tn).Value)).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, QuestionCreateViewModel model)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        if (!ModelState.IsValid)
        {
            var createData = await _questionService.GetCreatePageDataAsync();
            model.Categories = createData.Categories.Select(c => new SelectListItem { Value = c.Value, Text = c.Text });
            model.Tags = createData.Tags.Select(t => new SelectListItem { Value = t.Value, Text = t.Text });
            return View(model);
        }

        var newTagNames = (model.NewTags ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim());

        await _questionService.UpdateQuestionAsync(id, email, model.Title, model.Description, model.CategoryId, model.SelectedTagIds ?? new List<int>(), newTagNames);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _questionService.DeleteQuestionAsync(id, email);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AcceptAnswer(int id, int questionId)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _answerService.AcceptAnswerAsync(id, email);
        return RedirectToAction(nameof(Details), new { id = questionId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditAnswer(int id, int questionId, string content)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _answerService.UpdateAnswerAsync(id, email, content);
        return RedirectToAction(nameof(Details), new { id = questionId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteAnswer(int id, int questionId)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _answerService.DeleteAnswerAsync(id, email);
        return RedirectToAction(nameof(Details), new { id = questionId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(int answerId, int questionId, string content, int? parentCommentId)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _commentService.AddCommentAsync(answerId, email, content, parentCommentId);
        return RedirectToAction(nameof(Details), new { id = questionId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditComment(int id, int questionId, string content)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _commentService.UpdateCommentAsync(id, email, content);
        return RedirectToAction(nameof(Details), new { id = questionId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int id, int questionId)
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Challenge();

        await _commentService.DeleteCommentAsync(id, email);
        return RedirectToAction(nameof(Details), new { id = questionId });
    }
}
