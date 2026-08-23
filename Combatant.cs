using System;
using System.Collections.Generic;

public class Combatant
{
  public int Id {get;}
  public string Name {get;}
  public int TeamId {get;}
  public int MaxHP {get;}
  public int CurrentHP {get;private set;}
  public int BaseAtk {get;}
  public int Atk => BaseAtk;
  public int BaseMag {get;}
  public int Mag => BaseMag;
  public int BaseDef {get;}
  public int Def => BaseDef;
  public int BaseSpd {get;}
  public int Spd  => BaseSpd;
  public int BaseEva {get;}
  public int Eva => BaseEva;
  
  public HashSet<Suit> Riders {get;} = new HashSet<Suit>();
  public bool HasRider(Suit suit) => Riders.Contains(suit);

  public bool IsStunned {get;set;}

  public bool IsAlive => CurrentHP > 0;
  public bool CanAct => IsAlive && !IsStunned;
  public ControlSource Controller {get;set;} = ControlSource.AI;
  public Suit Element {get;set;} = Suit.None;

  private readonly Dictionary<Suit, int> _resistance = new Dictionary<Suit, int>();

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

  public int ResistanceTo(Suit element)
  {
    _resistance.TryGetValue(element, out int value);
    return value; 
  }

  public void AddResistance(Suit element, int amount)
  {
    _resistance[element] = ResistanceTo(element) + amount;
  }

  public override string ToString() => $"{Name} ({CurrentHP}/{MaxHP})";
}