namespace Application.Features.Sample;

public record SampleDto
{
    public static SampleDto Instance { get; } = new();
    public string? Name { get; set; }
}

