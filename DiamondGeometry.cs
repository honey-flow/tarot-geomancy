public enum SlotPosition //Mirrors Wands, Swords, Cups, Pentacles order for readibility, not actual indexing
{
  South,  //Wands     - Fire
  East,   //Swords    - Air
  West,   //Cups      - Water 
  North   //Pentacles - Earth
}

public enum Orientation
{
  Upright,
  Reversed
}

public static class DiamondGeometry
{
  public static Suit HomeSuit(SlotPosition slot)
  {
    switch (slot)
    {
      case SlotPosition.South: return Suit.Wands;
      case SlotPosition.East: return Suit.Swords;
      case SlotPosition.West: return Suit.Cups;
      case SlotPosition.North: return Suit.Pentacles;
      default: return Suit.None;
    }
  }

  public static Suit OpposingSuit(SlotPosition slot)
  {
    switch (slot)
    {
      case SlotPosition.South: return Suit.Pentacles;
      case SlotPosition.East: return Suit.Cups;
      case SlotPosition.West: return Suit.Swords;
      case SlotPosition.North: return Suit.Wands;
      default: return Suit.None;
    }
  }

  public static Suit LegalSuit(SlotPosition slot, Orientation orientation)
    => orientation == Orientation.Upright
      ? HomeSuit(slot)
      : OpposingSuit(slot);

  public static bool IsLegal(SlotPosition slot, Suit suit, Orientation orientation)
    => suit == LegalSuit(slot, orientation);

  public static bool TryOrientation(SlotPosition slot, Suit suit, out Orientation orientation)
  {
    if (suit == HomeSuit(slot)) 
    {
      orientation = Orientation.Upright; 
      return true;
    }
    if (suit == OpposingSuit(slot))
    {
      orientation = Orientation.Reversed;
      return true;
    }
    orientation = Orientation.Upright;
    return false;
  }
}