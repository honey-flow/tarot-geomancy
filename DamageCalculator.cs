using System;

public static class DamageCalculator
{
  //----- Tunables -----//
  public const double BaseCritChance = 0.15;
  public const double CritBonusFraction = 0.5;
  public const int MaxResistance = 300;
  public const double MaxHitChance = 100.0;

  //----- Step 1: Accuracy -----//
  public static double RawHitChance(Move move, Combatant target)
  {
    return move.Accuracy - target.Eva;
  }

  public static double HitChance(Move move, Combatant target)
  {
    double raw = RawHitChance(move, target);
    return Math.Clamp(raw, 0, MaxHitChance);
  }

  //----- Step 2: Overflow -----//
  public static double OverflowCrit(Move move, Combatant target)
  {
    double raw = RawHitChance(move, target);
    return Math.Max(0, raw - 100.0);
  }

  public static double CritChance(Move move, Combatant target)
  {
    return BaseCritChance + OverflowCrit(move, target)/100.0;
  }

  //----- Step 3: Scaling Stat -----//
  public static int AttackingStat(Combatant attacker, Move move)
  {
    switch (move.ScalesFrom)
    {
      case ScalingStat.Attack: return attacker.Atk;
      case ScalingStat.Magic: return attacker.Mag;
      default: return attacker.Atk;
    }
  }

  //----- Step 4: Base Attack Damage -----//
  public static double BaseDamage(Combatant attacker, Move move)
  {
    int stat = AttackingStat(attacker, move);
    return move.Power/100.0 * stat;
  }

  //----- Step 5: Defense Modification -----//
  public static double AfterDefense(double baseDamage, Combatant target)
  {
    return baseDamage * 100.0 / (100.0 + target.Def); 
  }

  //----- Step 6: Resistance Modification -----//
  public static double AfterResistance(double damage, Combatant target, Suit element)
  {
    int res = Math.Clamp(target.ResistanceTo(element), -99, MaxResistance);
    return damage * 100.0 / (100.0 + res);
  }

  //----- Step 7: Resolve -----//
  public static int Resolve(Combatant attacker, Combatant target, Move move, bool isCrit, bool ignoresDefense)
  {
    double baseDamage = BaseDamage(attacker, move);
    double mitigated = AfterDefense(baseDamage, target);
    double damage = ignoresDefense? baseDamage : mitigated;
    if (isCrit) damage += mitigated * CritBonusFraction;
    damage = AfterResistance(damage, target, move.Element);
    return (int)Math.Round(Math.Max(1, damage));
  }
}