using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Control
{
	private Button _attackButton;
	private Button _restartButton;
	private CheckBox _debugToggle;
	private LineEdit _seedField;
	private Label _seedLabel;
	private Label _battleStateLabel;

	private readonly Random _seedGenerator = new Random();

	private BattleControl _battle;
	private Combatant _pendingPlayerUnit;

	private readonly Dictionary<int, Label> _combatantRows = new Dictionary<int, Label>();
	private readonly Dictionary<int, VBoxContainer> _teamRows = new Dictionary<int, VBoxContainer>();

	public override void _Ready()
	{ 
		_attackButton = GetNode<Button>("AttackButton");
		_restartButton = GetNode<Button>("RestartButton");
		_debugToggle = GetNode<CheckBox>("DebugToggle");
		_seedField = GetNode<LineEdit>("SeedField");
		_seedLabel = GetNode<Label>("SeedLabel");
		_teamRows[BattleControl.PlayerTeam] = GetNode<VBoxContainer>("PlayerRows");
		_teamRows[BattleControl.EnemyTeam] = GetNode<VBoxContainer>("EnemyRows");
		_battleStateLabel = GetNode<Label>("BattleStateLabel");

		_attackButton.Pressed += OnAttackPressed;
		_restartButton.Pressed += OnRestartPressed;
		_debugToggle.Toggled += OnDebugToggled;

		_seedField.Visible = _debugToggle.ButtonPressed;

		OnRestartPressed();

		/**foreach (var (name, lines) in roster)
		{
			var f = new GeomanticFigure(name, lines, quality, suit);
			GD.Print($"{f}  HP {f.MaxHP}  ATK {f.Atk}  MAG {f.Mag}  DEF {f.Def}  SPD {f.Spd}");	
		}
**/
	}

	private void StartBattle(int seed)
	{
		if (_battle != null) _battle.LogEmitted -= OnBattleLog;

		_pendingPlayerUnit = null;


		_battle = new BattleControl(seed);
		_battle.LogEmitted += OnBattleLog;

		_seedLabel.Text = $"Seed: {seed}";
		GD.Print($"=== New battle, seed: {seed} ===");

		AddFigure("Populus", BattleControl.PlayerTeam, ControlSource.Player);
		AddFigure("Fortuna Minor", BattleControl.PlayerTeam, ControlSource.Player);
		AddFigure("Laetitia", BattleControl.EnemyTeam, ControlSource.AI).Riders.Add(Suit.Wands);
		AddFigure("Rubeus", BattleControl.EnemyTeam, ControlSource.AI);


		_battle.StartRound();
		BuildCombatantRows();
		RefreshUI();
		RequestNextDeclaration();
	}

	private void BuildCombatantRows()
	{
		_combatantRows.Clear();
		foreach (var container in _teamRows.Values)
		{
			foreach(var child in container.GetChildren())
			{
				container.RemoveChild(child);
				child.QueueFree();
			}
		}

		foreach (var c in _battle.Combatants)
		{
			if(!_teamRows.TryGetValue(c.TeamId, out var container)) continue;
		
			var row = new Label();
			row.Name= $"{c.Id}_{c.Name}";
			
			container.AddChild(row);

			_combatantRows[c.Id] = row;
		}
	}

	private Combatant AddFigure(string figureName, int teamId, ControlSource controller)
	{
		var figure = FigureRoster.Get(figureName);

		var combatant = _battle.AddCombatant(
			figure.Name,
			teamId,
			maxHP: figure.MaxHP,
			atk: figure.Atk,
			mag: figure.Mag,
			def: figure.Def,
			spd: figure.Spd,
			eva: 0);

		combatant.Controller = controller;
		return combatant;
	}

	private void OnAttackPressed()
	{
		if (_pendingPlayerUnit == null) return;
		//Player Declares Attack and Target
		var target = _battle.LivingEnemiesOf(_pendingPlayerUnit).FirstOrDefault();
		if (target == null) return;
		_battle.Declare(BattleAction.Attack(_pendingPlayerUnit, target, Move.BasicAttack));

		_pendingPlayerUnit = null;

		RequestNextDeclaration();
	}

	private void OnRestartPressed()
	{
		StartBattle(ResolveSeed());
	}

	private int ResolveSeed()
	{
		if (_debugToggle.ButtonPressed && int.TryParse(_seedField.Text, out int typed))
			return typed;
		return _seedGenerator.Next();
	}

	private void OnDebugToggled(bool pressed)
	{
		_seedField.Visible = pressed;
	}

	public void RequestNextDeclaration()
	{
		if (_battle.Phase == BattlePhase.Finished) return;
		var unit = _battle.AwaitingDeclaration.FirstOrDefault();
		if (unit == null)
		{
			RunResolution();
			return;
		}
		if (unit.Controller != ControlSource.Player)
		{
			_battle.Declare(_battle.GetAIAction(unit));
			RequestNextDeclaration();
			return;
		}

		_pendingPlayerUnit = unit;
		RefreshUI();
	}

	public void RunResolution()
	{
		if (_battle.Phase != BattlePhase.Declaration) return;
		_battle.BeginResolution();
		_battle.ResolveEntireRound();
		if (_battle.Phase != BattlePhase.Finished)
		{
			_battle.StartRound();
			RequestNextDeclaration();
		}
		RefreshUI();
	}

	private void OnBattleLog(string message)
	{
		GD.Print(message);
	}

	private void RefreshUI()
	{
		foreach (var c in _battle.Combatants)
		{
			if (!_combatantRows.TryGetValue(c.Id, out var row)) continue;

			row.Text = $"{c.Name}   HP: {c.CurrentHP}/{c.MaxHP}";
			row.Modulate = c.IsAlive ? Colors.White : new Color(0.4f, 0.4f, 0.4f);
			if (c == _pendingPlayerUnit) row.Text += "  <";
		}

		if (_battle.Phase == BattlePhase.Finished)
		{
			_battleStateLabel.Text = _battle.WinningTeam == BattleControl.PlayerTeam
				? "Victory"
				: "Defeat";
			_attackButton.Disabled = true;
		}
		else
		{
			_battleStateLabel.Text = $"Round {_battle.RoundNumber}";
			_attackButton.Disabled = false;
		}
	}
}