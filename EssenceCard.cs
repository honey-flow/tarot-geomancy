using System;
using System.Collections.Generic;

public enum CardRank
{
  Ace = 1,
  Two = 2,
  Three = 3,
  Four = 4,
  Five = 5,
  Six = 6,
  Seven = 7,
  Eight = 8, 
  Nine = 9,
  Ten = 10,
  Page = 11,
  Knight = 12,
  Queen = 13,
  King = 14
}

public enum StatKind
{
  Atk,
  Mag,
  Def,
  Spd
}

public class EssenceCard
{
  public Suit CardSuit {get;}
  public CardRank Rank {get;}

  public string Name => $"{RankName(Rank)} of {CardSuit}";
  public bool IsCourt => Rank >= CardRank.Page;
  public int StatPercent => StatPercentFor(Rank);
  public int ResistanceValue => ResistanceFor(Rank);
  
  public StatKind? BoostedStat => StatForSuit(CardSuit);

  public EssenceCard(Suit suit, CardRank rank)
  {
    if (suit == Suit.None)
      throw new ArgumentException("An essence card must have a suit", nameof(suit));
    
    CardSuit = suit;
    Rank = rank;
  }

  public override string ToString() => Name;

  //----- Scaling -----//
  private const int PipMultiplier = 2;
  private static readonly Dictionary<CardRank, int> CourtStatPercent = new Dictionary<CardRank, int>
  {
    {CardRank.Page, 25},
    {CardRank.Knight, 31},
    {CardRank.Queen, 38},
    {CardRank.King, 45}
  };

  public static int StatPercentFor(CardRank rank)
  {
    if (CourtStatPercent.TryGetValue(rank, out int courtValue)) return courtValue;
    return (int)rank * PipMultiplier;
  }

  public static int ResistanceFor(CardRank rank) => StatPercentFor(rank);

  //----- Suit Mappings -----//
  public static StatKind? StatForSuit(Suit suit)
  {
    switch (suit)
    {
      case Suit.Wands: return StatKind.Atk;
      case Suit.Swords: return StatKind.Spd;
      case Suit.Cups: return StatKind.Mag;
      case Suit.Pentacles: return StatKind.Def;
      default: return null;
    }
  }

  private static readonly List<EssenceCard> _all = BuildDeck();
  public static IReadOnlyList<EssenceCard> All => _all;
  private static List<EssenceCard> BuildDeck()
  {
    var deck = new List<EssenceCard>();
    foreach (Suit suit in new[] {Suit.Wands, Suit.Swords, Suit.Cups, Suit.Pentacles})
      foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
        deck.Add(new EssenceCard(suit, rank));
    return deck;
  }

  public static EssenceCard Get(Suit suit, CardRank rank){
    foreach (var c in _all)
      if (c.CardSuit == suit && c.Rank == rank) return c;
    
    throw new ArgumentException($"No essence card for {rank} or {suit}");
  }

  private static string RankName(CardRank rank) => rank.ToString();
}