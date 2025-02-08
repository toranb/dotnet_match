using System.Collections.Immutable;

namespace Match.Models;

public record Engine
{
    public bool Animating { get; init; }
    public bool Winner { get; init; }
    public ImmutableList<Card> Cards { get; init; }
}
