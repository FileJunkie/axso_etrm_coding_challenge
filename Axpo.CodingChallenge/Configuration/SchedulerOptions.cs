using System.ComponentModel.DataAnnotations;

namespace Axpo.CodingChallenge.Configuration;

public class SchedulerOptions
{
    [Required]
    [Range(1, int.MaxValue)]
    public required int IntervalInMinutes { get; init; }
}