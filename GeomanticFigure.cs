using System;
using System.Linq;

public class GeomanticFigure
{
  public string Name {get;}
  public Quality MovementQuality {get;}
  public Suit RulingElement {get;}
  private const int BaseStat = 100;
  private const double HpScale = 2.5;
  private const int SingleSourceSwing = 20;
  private const int DualSourceSwing = 10;
  public bool Fire {get;}
  public bool Air {get;}
  public bool Water {get;}
  public bool Earth {get;}
  public string Lines => 
    (Fire ? "1" : "0") +
    (Air ? "1" : "0") +
    (Water ? "1" : "0") +
    (Earth ? "1" : "0");
  public string Glyph =>
    (Fire ? "●" : "○") +
    (Air ? "●" : "○") +
    (Water ? "●" : "○") +
    (Earth ? "●" : "○");
  public int MaxHP => (int)((BaseStat + (Water ? DualSourceSwing : -DualSourceSwing) + (Earth ? DualSourceSwing : -DualSourceSwing)) * HpScale);
  public int Atk => BaseStat + (Fire ? SingleSourceSwing : -SingleSourceSwing);
  public int Mag => BaseStat + (Air ? DualSourceSwing : -DualSourceSwing) + (Water ? DualSourceSwing : -DualSourceSwing);
  public int Def => BaseStat + (Earth ? SingleSourceSwing : -SingleSourceSwing);
  public int Spd => BaseStat + (Fire ? DualSourceSwing : -DualSourceSwing) + (Air ? DualSourceSwing : -DualSourceSwing);
  
  public GeomanticFigure(string name, string lines, Quality movementQuality, Suit rulingElement){
    if (lines == null || lines.Length != 4 || lines.Any(c => c != '0' && c!= '1')) throw new ArgumentException($"Figure lines must be 4 characters of 0 or 1, got '{lines}'", nameof(lines));

    Name = name;
    MovementQuality = movementQuality;
    RulingElement = rulingElement;
    Fire = lines[0] == '1';
    Air = lines [1] == '1';
    Water = lines[2] == '1';
    Earth = lines[3] == '1'; 
  }

  public override string ToString() => $"{Name}  {Glyph} ({Lines})";
}