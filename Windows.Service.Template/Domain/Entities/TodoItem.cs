using Windows.Service.Template.Domain.Common;
using Windows.Service.Template.Domain.Enums;

namespace Windows.Service.Template.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public int ItemId { get; set; }

    public string Title { get; set; }

    public string Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    public bool Done { get; set; }

    public int ListId { get; set; }
    public TodoList List { get; set; } = null!;
}

