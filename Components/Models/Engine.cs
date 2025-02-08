namespace Match.Models;

public record Engine
{
    public bool Animating { get; init; }
    public bool Winner { get; init; }
    public List<Card> Cards { get; init; }
}
