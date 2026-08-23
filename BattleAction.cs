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
  public Move FigureMove {get;}

  private BattleAction(BattleActionType type, Combatant actor, Combatant target, Move figureMove)
  {
    Type = type;
    Actor = actor;
    Target = target;
    FigureMove = figureMove;
  }

  public static BattleAction Attack(Combatant actor, Combatant target, Move figureMove)
    => new BattleAction(BattleActionType.Attack, actor, target, figureMove);

  public static BattleAction Skip(Combatant actor)
    => new BattleAction(BattleActionType.Skip, actor, null, null);

  public override string ToString()
  {
    if(Type == BattleActionType.Skip) {
      return $"{Actor.Name} skips";
    } else if(Target == null){
      return $"{Actor.Name} used {FigureMove.Name}";
    } else {
      return $"{Actor.Name} used {FigureMove.Name} on {Target.Name}";
    }
  }
}

