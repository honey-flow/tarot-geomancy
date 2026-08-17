using System;

public enum ControlSource
{
  AI,
  Player
}

public class Combatant
{
  public int Id {get;}
  public string Name {get;}
  public int TeamId {get;set;}
  public int MaxHP {get;set;}
  public int CurrentHP {get;private set;}
  public int Atk {get;set;}
  public int Spd {get;set;}

  public bool IsStunned {get;set;}

  public bool IsAlive => CurrentHP > 0;
  public bool CanAct => IsAlive && !IsStunned;
  public ControlSource Controller {get;set;} = ControlSource.AI;

  public Combatant(int id, string name, int teamId, int maxHp, int atk, int spd)
  {
      Id = id;
      Name = name;
      TeamId = teamId;
      MaxHP = maxHp;
      CurrentHP = maxHp;
      Atk = atk;
      Spd = spd;
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