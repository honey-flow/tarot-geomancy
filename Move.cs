public class Move
{
  public string Name {get;}
  public Suit Element {get;}
  public int Power {get;}
  public int Accuracy {get;}
  public ScalingStat ScalesFrom {get;}

  public Move(string name, Suit element, int power, int accuracy, ScalingStat scalesFrom)
  {
    Name = name;
    Element = element;
    Power = power;
    Accuracy = accuracy;
    ScalesFrom = scalesFrom;
  }

  public static readonly Move BasicAttack = new Move(
    "Basic Attack",
    Suit.None,
    power: 100,
    accuracy: 95,
    ScalingStat.Attack
  );
  public static readonly Move Flame = new Move(
    "Flame",
    Suit.Wands,
    power: 100,
    accuracy: 95,
    ScalingStat.Attack
  );

  public override string ToString() => Name;
}