namespace Example.Models;

public class Card
{
    public int Id { get; set; }
    public bool Flipped { get; set; }
    public bool Paired { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
