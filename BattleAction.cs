public enum BattleActionType
{
  Attack,
  Skip
}

public class BattleAction
{
  public BattleActionType Type {get;}
  public Combatant Actor {get;}
  public Combatant Target {get;}


  private BattleAction(BattleActionType type, Combatant actor, Combatant target)
  {
    Type = type;
    Actor = actor;
    Target = target;
  }

  public static BattleAction Attack(Combatant actor, Combatant target)
    => new BattleAction(BattleActionType.Attack, actor, target);

  public static BattleAction Skip(Combatant actor)
    => new BattleAction(BattleActionType.Skip, actor, null);

  public override string ToString() => Target == null
    ? $"{Actor.Name}: {Type}"
    : $"{Actor.Name}: {Type} -> {Target.Name}";
}

