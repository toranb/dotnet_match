namespace Match.Models;

public record Card
{
    public int Id { get; init; }
    public bool Flipped { get; init; }
    public bool Paired { get; init; }
    public string Name { get; init; }
    public string ImageUrl { get; init; }
}
