using System;
using System.Collections.Generic;
using System.Linq;

public static class FigureRoster
{
  private static readonly GeomanticFigure[] All =
  {
    new GeomanticFigure("Via", "1111", Quality.Mobile, Suit.None),
    new GeomanticFigure("Puer", "1101", Quality.Mobile, Suit.Wands),
    new GeomanticFigure("Puella", "1011", Quality.Stable, Suit.Cups),
    new GeomanticFigure("Cauda Draconis", "1110", Quality.Mobile, Suit.Wands),
    new GeomanticFigure("Caput Draconis", "0111", Quality.Stable, Suit.Pentacles),
    new GeomanticFigure("Carcer", "1001", Quality.Stable, Suit.Pentacles),
    new GeomanticFigure("Fortuna Minor", "1100", Quality.Mobile, Suit.Wands),
    new GeomanticFigure("Fortuna Major", "0011", Quality.Stable, Suit.Pentacles),
    new GeomanticFigure("Amissio", "1010", Quality.Mobile, Suit.Wands),
    new GeomanticFigure("Acquisitio", "0101", Quality.Stable, Suit.Swords),
    new GeomanticFigure("Conjunctio", "0110", Quality.Mobile, Suit.Swords),
    new GeomanticFigure("Laetitia", "1000", Quality.Mobile, Suit.Wands),
    new GeomanticFigure("Tristitia", "0001", Quality.Stable, Suit.Pentacles),
    new GeomanticFigure("Rubeus", "0100", Quality.Mobile, Suit.Swords),
    new GeomanticFigure("Albus", "0010", Quality.Stable, Suit.Cups),
    new GeomanticFigure("Populus", "0000", Quality.Stable, Suit.Cups)
  };

  private static readonly Dictionary<string, GeomanticFigure> ByName = All.ToDictionary(f => f.Name);
  public static IReadOnlyList<GeomanticFigure> All16 => All;

  public static GeomanticFigure Get(string name)
  {
    if (ByName.TryGetValue(name, out var figure)) return figure;
    throw new ArgumentException($"No geomantic figure named '{name}'", nameof(name));
  }
}