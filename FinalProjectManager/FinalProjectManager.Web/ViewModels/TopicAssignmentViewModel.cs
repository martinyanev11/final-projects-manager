using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectManager.Web.ViewModels;

public class TopicAssignmentViewModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? TopicId { get; set; }
    public IEnumerable<SelectListItem> AvailableTopics { get; set; } = [];
}
