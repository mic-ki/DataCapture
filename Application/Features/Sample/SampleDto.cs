namespace Application.Features.Sample;

public record SampleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Age { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}

