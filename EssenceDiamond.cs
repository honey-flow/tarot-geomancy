using System;
using System.Collections.Generic;

public class EssenceDiamond
{
  private readonly Dictionary<SlotPosition, EssenceCard> _slots = new Dictionary<SlotPosition, EssenceCard>();

  public bool TryPlace(SlotPosition slot, EssenceCard card)
  {
    if (card == null) return false;
    if (!DiamondGeometry.TryOrientation(slot, card.CardSuit, out _)) return false;

    _slots[slot] = card;
    return true;
  }

  public bool Remove(SlotPosition slot) => _slots.Remove(slot);

  public void Clear() => _slots.Clear();

  public EssenceCard CardAt(SlotPosition slot)
  {
    _slots.TryGetValue(slot, out var card);
    return card;
  }

  public Orientation? OrientationAt(SlotPosition slot)
  {
    var card = CardAt(slot);
    if (card == null) return null;

    DiamondGeometry.TryOrientation(slot, card.CardSuit, out var orientation);
    return orientation;
  }

  public bool IsFull => _slots.Count == 4;
  public int FillCount => _slots.Count;

  public string Lines => 
    LineFor(SlotPosition.South) +
    LineFor(SlotPosition.East) +
    LineFor(SlotPosition.West) +
    LineFor(SlotPosition.North);

  public string Glyph => 
    GlyphFor(SlotPosition.South) +
    GlyphFor(SlotPosition.East) +
    GlyphFor(SlotPosition.West) +
    GlyphFor(SlotPosition.North);

  private bool IsActive(SlotPosition slot) => OrientationAt(slot) == Orientation.Upright;

  public int PercentFor(StatKind kind)
  {
    int total = 0;
    foreach (SlotPosition slot in Enum.GetValues(typeof(SlotPosition)))
    {
      if (!IsActive(slot)) continue;
      var card = CardAt(slot);
      if (card.BoostedStat == kind) total += card.StatPercent;
    }
    return total;
  }

  public bool HasRider(Suit suit)
  {
    foreach (SlotPosition slot in Enum.GetValues(typeof(SlotPosition)))
      if (IsActive(slot) && CardAt(slot).CardSuit == suit) return true;
    return false; 
  }

  public int ResistanceFor(Suit element)
  {
    int total = 0;
    foreach (SlotPosition slot in Enum.GetValues(typeof(SlotPosition)))
    {
      var card = CardAt(slot);
      if (card == null || IsActive(slot)) continue;
      if (card.CardSuit == element) total += card.ResistanceValue;
    }
    return total;
  }

  private string LineFor(SlotPosition slot) => IsActive(slot) ? "1" : "0";
  private string GlyphFor(SlotPosition slot) => IsActive(slot) ? "●" : "○";

  public override string ToString() => $"Diamond {Glyph} ({Lines}) {FillCount}/4";
}