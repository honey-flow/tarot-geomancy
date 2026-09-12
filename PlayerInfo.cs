using System.Linq;
using System.Collections.Generic;

public class PlayerInfo
{
  private readonly Dictionary<EssenceCard, int> _essences = new();

  public bool UnlimitedEssences {get;set;}

  public int CountOf(EssenceCard card)
  {
    _essences.TryGetValue(card, out int count);
    return count;
  }
  public void AddEssenceCard(EssenceCard card, int n = 1)
  {
    _essences[card] = CountOf(card) + n;
  }
  public IEnumerable<EssenceCard> AvailableEssences =>
    UnlimitedEssences
      ? EssenceCard.All
      : _essences.Where(kv => kv.Value > 0).Select(kv => kv.Key);

}