using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class LoadoutPanel : Panel
{
  private Label _figureLabel;
  private OptionButton _figureSelector;
  private VBoxContainer _cardList;

  private readonly Dictionary<SlotPosition, Button> _slotButtons = new Dictionary<SlotPosition, Button>();

  private BattleControl _battle;
  private PlayerInfo _player;
  private bool _allowAllTeams;

  private Combatant _editing;

  private SlotPosition? _selectedSlot;

  private static readonly Color Dimmed = new Color(0.6f, 0.7f, 0.7f);

  private static string Arrow(Orientation o)
    => o == Orientation.Upright ? "↑" : "↓";
  
  private static string EffectText(EssenceCard card, Orientation o)
    => o == Orientation.Upright ? $"+{card.StatPercent}% {card.BoostedStat}" : $"RES {card.ResistanceValue}";

  public override void _Ready()
  {
   _figureLabel = GetNode<Label>("FigureLabel");
   _figureSelector = GetNode<OptionButton>("FigureSelector");
   _cardList = GetNode<VBoxContainer>("CardList");

   _slotButtons[SlotPosition.South] = GetNode<Button>("SouthSlot");
   _slotButtons[SlotPosition.East] = GetNode<Button>("EastSlot");
   _slotButtons[SlotPosition.West] = GetNode<Button>("WestSlot");
   _slotButtons[SlotPosition.North] = GetNode<Button>("NorthSlot");

   foreach (var pair in _slotButtons)
    {
      SlotPosition slot = pair.Key;
      pair.Value.Pressed += () => OnSlotPressed(slot);
    }

    _figureSelector.ItemSelected += OnFigureSelected;

    Visible = false;
  }

  public void Open(BattleControl battle, PlayerInfo player, bool allowAllTeams = false)
  {
    _battle = battle;
    _player = player;
    _selectedSlot = null;
    _allowAllTeams = allowAllTeams;

    PopulateFigureSelector();

    _editing = allowAllTeams
      ? _battle.Combatants.FirstOrDefault()
      : _battle.Combatants.FirstOrDefault(c => c.Controller == ControlSource.Player);

    Visible = true;
    Refresh();
  }

  public void Close() => Visible = false;

  private void PopulateFigureSelector()
  {
    _figureSelector.Clear();

    foreach (var c in _battle.Combatants)
    {
      string side = c.TeamId == BattleControl.PlayerTeam ? "P" : "E";

      if (!_allowAllTeams && c.Controller != ControlSource.Player) continue;
      _figureSelector.AddItem($"{side} {c.Name} ", c.Id);
    }
  }

  private void OnFigureSelected(long index)
  {
    int combatantId = _figureSelector.GetItemId((int)index);
    _editing = _battle.Combatants.FirstOrDefault(c => c.Id == combatantId);

    _selectedSlot = null;

    Refresh();
  }

  private void OnSlotPressed(SlotPosition slot)
  {
    if (_selectedSlot == slot) _selectedSlot = null;
    else _selectedSlot = slot;

    Refresh();
  }

  private void Refresh()
  {
    RefreshHeader();
    RefreshSlots();
    RebuildCardList();
  }

  private void RefreshHeader()
  {
    if (_editing == null)
    {
      _figureLabel.Text = "(no figure)";
      return;
    }

    _figureLabel.Text = $"{_editing.Name}  {_editing.Diamond.Glyph}  {_editing.Diamond.Lines}   " +
                        $"ATK {_editing.BaseAtk} -> {_editing.Atk}  " +
                        $"MAG {_editing.BaseMag} -> {_editing.Mag}  " +
                        $"DEF {_editing.BaseDef} -> {_editing.Def}  " +
                        $"SPD {_editing.BaseSpd} -> {_editing.Spd}";
  }

  private void RefreshSlots()
  {
    foreach (var pair in _slotButtons)
    {
      SlotPosition slot = pair.Key;
      Button button = pair.Value;

      Suit home = DiamondGeometry.HomeSuit(slot);
      Suit opposing = DiamondGeometry.OpposingSuit(slot);

      EssenceCard card = _editing?.Diamond.CardAt(slot);

      if (card == null)
      {
        button.Text = $"{slot}\n(empty)\n{home}↑ / {opposing}↓";
      }
      else
      {
        Orientation o = _editing.Diamond.OrientationAt(slot).Value;
        button.Text = $"{slot}\n{card.Name}{Arrow(o)}";
      }

      bool isSelected = _selectedSlot == slot;
      button.Modulate = isSelected ? Colors.White : Dimmed;
    }
  }

  private void RebuildCardList()
  {
    foreach (var child in _cardList.GetChildren())
    {
      _cardList.RemoveChild(child);
      child.QueueFree();
    }

    if (_selectedSlot == null || _editing == null) return;
    
    SlotPosition slot = _selectedSlot.Value;

    var legal = _player.AvailableEssences.Where(card =>DiamondGeometry.TryOrientation(slot, card.CardSuit, out _)).ToList();

    foreach (var card in legal)
    {
      var button = new Button();

      DiamondGeometry.TryOrientation(slot, card.CardSuit, out var o);

      button.Text = $"{card.Name} {Arrow(o)} {EffectText(card, o)}";

      var captured = card;
      button.Pressed += () => OnCardChosen(captured);

      _cardList.AddChild(button);
    }    
  }

  private void OnCardChosen(EssenceCard card)
  {
    if (_selectedSlot == null || _editing == null) return;

    _editing.Diamond.TryPlace(_selectedSlot.Value, card);

    _selectedSlot = null;

    Refresh();
  }
}