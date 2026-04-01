using System;
using System.Collections.Generic;

namespace QuestionAnswer.Domain.Entities;

public class Answer
{
    public int AnswerId { get; set; }
    public int QuestionId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsAccepted { get; set; }
    public DateTime CreatedAt { get; set; }

    public Question Question { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}

