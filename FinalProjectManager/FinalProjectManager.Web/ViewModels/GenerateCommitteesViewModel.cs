using System.ComponentModel.DataAnnotations;

namespace FinalProjectManager.Web.ViewModels;

public class GenerateCommitteesViewModel
{
    [Required, Range(1, 20), Display(Name = "Брой комисии")]
    public int CommitteeCount { get; set; } = 2;

    [Required, Range(1, 10), Display(Name = "Учители в комисия")]
    public int SupervisorsPerCommittee { get; set; } = 3;
}
