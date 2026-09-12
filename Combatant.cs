using System;

public class Combatant
{
  public int Id {get;}
  public string Name {get;}
  public int TeamId {get;}
  public int MaxHP {get;}
  public int CurrentHP {get;private set;}
  public int BaseAtk {get;}
  public int Atk => WithEssences(BaseAtk, StatKind.Atk);
  public int BaseMag {get;}
  public int Mag => WithEssences(BaseMag, StatKind.Mag);
  public int BaseDef {get;}
  public int Def => WithEssences(BaseDef, StatKind.Def);
  public int BaseSpd {get;}
  public int Spd  => WithEssences(BaseSpd, StatKind.Spd);
  public int BaseEva {get;}
  public int Eva => BaseEva;

  public EssenceDiamond Diamond {get;} = new EssenceDiamond();
  private int WithEssences(int baseValue, StatKind kind) => (int)Math.Round(baseValue * (1 + Diamond.PercentFor(kind) / 100.0));
  public bool HasRider(Suit suit) => Diamond.HasRider(suit);
  public int ResistanceTo(Suit element) => Diamond.ResistanceFor(element);
  
  public bool IsStunned {get;set;}

  public bool IsAlive => CurrentHP > 0;
  public bool CanAct => IsAlive && !IsStunned;
  public ControlSource Controller {get;set;} = ControlSource.AI;
  public Suit Element {get;set;} = Suit.None;

  public Combatant(int id, string name, int teamId, int maxHp, int atk, int mag, int def, int spd, int eva)
  {
      Id = id;
      Name = name;
      TeamId = teamId;
      MaxHP = maxHp;
      CurrentHP = maxHp;
      BaseAtk = atk;
      BaseMag = mag;
      BaseDef = def;
      BaseSpd = spd;
      BaseEva = eva;
  }

  public int TakeDamage(int amount)
  {
    if (amount < 0) amount = 0;
    int before = CurrentHP;
    CurrentHP = Math.Max(0, CurrentHP - amount);
    return before - CurrentHP;
  }

  public int Heal(int amount)
  {
    if (amount < 0 || !IsAlive) return 0;
    int before = CurrentHP;
    CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
    return CurrentHP - before;
  }

  public override string ToString() => $"{Name} ({CurrentHP}/{MaxHP})";
}