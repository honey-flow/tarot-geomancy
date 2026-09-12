using System;
using Godot;

public static class DebugReports
{
  public static void Roster()
  {
    GD.Print("=== FIGURE ROSTER ===");
    GD.Print("Figure            Lines   HP  Atk  Mag  Def  Spd   Quality  Element");
    foreach (var f in FigureRoster.All16)
    {
      GD.Print($"{f.Name,-16}  {f.Glyph}  {f.MaxHP,4 } {f.Atk, 4} {f.Mag, 4} {f.Def, 4} {f.Spd, 4}   {f.MovementQuality, -7}  {f.RulingElement}");
    }
  }

  public static void Deck()
  {
    GD.Print($"=== ESSENCE DECK ({EssenceCard.All.Count} cards) ===");
    foreach (Suit suit in new[] {Suit.Wands, Suit.Swords, Suit.Cups, Suit.Pentacles})
    {
      var line = $"{suit, -10}: ";
      foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
        line += $"{EssenceCard.StatPercentFor(rank), 3} ";
      GD.Print(line);
    }
    var king = EssenceCard.Get(Suit.Cups, CardRank.King);
    GD.Print($"  sample: {king} +{king.StatPercent}% boosts {king.BoostedStat}  RES {king.ResistanceValue}");
  }

  public static void Geometry()
  {
    GD.Print("=== DIAMOND GEOMETRY ===");
    foreach (SlotPosition slot in Enum.GetValues(typeof(SlotPosition)))
    {
      GD.Print($"{slot, -6} upright: {DiamondGeometry.HomeSuit(slot), -10} reversed: {DiamondGeometry.OpposingSuit(slot), -10}");
    }

    GD.Print($"  Cups in South?  {DiamondGeometry.TryOrientation(SlotPosition.South, Suit.Cups, out _)}");
    GD.Print($"  Wands in South? {DiamondGeometry.TryOrientation(SlotPosition.South, Suit.Wands, out _)}");
    GD.Print($"  Wands in North? {DiamondGeometry.TryOrientation(SlotPosition.North, Suit.Wands, out _)}");
  }

  public static void Diamonds(BattleControl battle)
  {
    GD.Print("=== LOADOUTS ===");
    foreach (var c in battle.Combatants)
    {
      GD.Print($"{c.Name, -14} {c.Diamond}   element {c.Element}");
      GD.Print($"  ATK {c.BaseAtk, 3} -> {c.Atk, 3}   MAG {c.BaseMag, 3} -> {c.Mag, 3}   "+
                 $"DEF {c.BaseDef, 3} -> {c.Def, 3}   Spd {c.BaseSpd, 3} -> {c.Spd, 3}   EVA {c.Eva}");
      
      var riders = "";
      foreach (Suit s in new[] {Suit.Wands, Suit. Swords, Suit.Cups, Suit.Pentacles})
        if (c.HasRider(s)) riders += s + " ";
      GD.Print($"   riders: {(riders == "" ? "none" : riders)}");
      GD.Print($"   RES W:{c.ResistanceTo(Suit.Wands), 3}   S:{c.ResistanceTo(Suit.Swords), 3}   "+
                      $"C:{c.ResistanceTo(Suit.Cups), 3}   P:{c.ResistanceTo(Suit.Pentacles), 3}");

      foreach (SlotPosition slot in Enum.GetValues(typeof(SlotPosition)))
      {
        var card = c.Diamond.CardAt(slot);
        if (card == null) continue;
        GD.Print($"    {slot, -6}   {card, -18} {c.Diamond.OrientationAt(slot)}");
      }
    }
  }

  public static void Damage(BattleControl battle){
    GD.Print("=== DAMAGE MATRIX (normal / crit, Basic Attack) ===");
    foreach (var a in battle.Combatants)
    {
      foreach (var d in battle.Combatants)
      {
        if (a == d) continue;

        bool trueDmg = a.HasRider(Suit.Wands);
        int normal = DamageCalculator.Resolve(a, d, Move.BasicAttack, false, trueDmg);
        int crit = DamageCalculator.Resolve(a, d, Move.BasicAttack, true, trueDmg);

        Suit element = Move.BasicAttack.Element == Suit.None ? a.Element : Move.BasicAttack.Element;
        GD.Print($"{a.Name, -14} -> {d.Name, -14}  {normal, 4} / {crit ,4}   [{element}, DEF {d.Def}, RES {d.ResistanceTo(element)} {(trueDmg ? ", TRUE" : "")}]");
      }
    }
  }

  public static void Accuracy(BattleControl battle)
  {
    GD.Print("=== ACCURACY ===");
      foreach (var d in battle.Combatants)
      {
        GD.Print($"{d.Name, -14} EVA {d.Eva, 4}    " +
                 $"raw {DamageCalculator.RawHitChance(Move.BasicAttack, d), 6:F1}    " +
                 $"hit {DamageCalculator.HitChance(Move.BasicAttack, d), 6:F1}    " +
                 $"overflow {DamageCalculator.OverflowCrit(Move.BasicAttack, d), 5:F1}    " +
                 $"crit {DamageCalculator.CritChance(Move.BasicAttack, d), 5:F2}");
      }
  }

  public static void DiamondBuild()
  {
        GD.Print("=== DIAMOND BUILD WALKTHROUGH ===");
        var d = new EssenceDiamond();
        GD.Print($"  empty                            {d}");

        var wandsKing = EssenceCard.Get(Suit.Wands, CardRank.King);
        var cupsTwo = EssenceCard.Get(Suit.Cups, CardRank.Two);

        GD.Print($"  King of Wands -> South: {d.TryPlace(SlotPosition.South, wandsKing),-5} {d}");
        GD.Print($"  Two of Cups -> South: {d.TryPlace(SlotPosition.South, cupsTwo), -5} {d}   (illegal axis)");
        GD.Print($"  King of Wands -> North: {d.TryPlace(SlotPosition.North, wandsKing), -5} {d}   (reversed)");
        GD.Print($"  Two of Cups -> West: {d.TryPlace(SlotPosition.West, cupsTwo), -5} {d}");

        GD.Print($"  ATK% {d.PercentFor(StatKind.Atk)}   MAG% {d.PercentFor(StatKind.Mag)}   " +
                 $"  Wands Rider {d.HasRider(Suit.Wands)}   RES {d.ResistanceFor(Suit.Wands)}");
  }
}