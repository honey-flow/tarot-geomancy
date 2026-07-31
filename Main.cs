using Godot;
using System.Linq;

public partial class Main : Control
{
	private Button _attackButton;
	private Label _playerHPLabel;
	private Label _enemyHPLabel;
	private Label _battleStateLabel;

	private BattleControl _battle;
	private Combatant _player;
	private Combatant _enemy;

	public override void _Ready()
	{
		_attackButton = GetNode<Button>("AttackButton");
		_playerHPLabel = GetNode<Label>("PlayerHPLabel");
		_enemyHPLabel = GetNode<Label>("EnemyHPLabel");
		_battleStateLabel = GetNode<Label>("BattleStateLabel");

		_attackButton.Pressed += OnAttackPressed;

		StartBattle();
	}

	private void StartBattle()
	{
		_battle = new BattleControl(seed: 12345);
		_battle.LogEmitted += OnBattleLog;

		_player = _battle.AddCombatant("Player", BattleControl.PlayerTeam, maxHP: 100, atk: 15, spd: 35);
		_enemy = _battle.AddCombatant("Enemy", BattleControl.EnemyTeam, maxHP: 100, atk: 15, spd: 30);

		_battle.StartRound();
		RefreshUI();
	}

	private void OnAttackPressed()
	{
		if (_battle.Phase == BattlePhase.Finished) return;

		//Player Declares Attack and Target
		var target = _battle.LivingEnemiesOf(_player).FirstOrDefault();
		if (target == null) return;
		_battle.Declare(BattleAction.Attack(_player, target));

		//Enemy Team Declares
		_battle.DeclareForTeam(BattleControl.EnemyTeam);

		//Resolve Attack Sequence
		_battle.BeginResolution();
		_battle.ReturnEntireRound();

		//Move to Next Turn
		if (_battle.Phase != BattlePhase.Finished)
			  _battle.StartRound();

		RefreshUI();
	}

	private void OnBattleLog(string message)
	{
		GD.Print(message);
	}

	private void RefreshUI()
	{
		_playerHPLabel.Text = $"{_player.Name}: {_player.CurrentHP}/{_player.MaxHP}";
		_enemyHPLabel.Text = $"{_enemy.Name}: {_enemy.CurrentHP}/{_enemy.MaxHP}";

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
		}
	}
}