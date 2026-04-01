namespace QuestionAnswer.Contract;

public class AnswerDto
{
    public int AnswerId { get; set; }
    public string Content { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int Score { get; set; }
    public bool IsAccepted { get; set; }
}
