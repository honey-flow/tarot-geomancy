using System;
using System.Collections.Generic;
using System.Linq;

public enum BattlePhase
{
  Declaration,
  Resolution,
  Finished
}

public class BattleControl
{
  public const int PlayerTeam = 0;
  public const int EnemyTeam = 1;

  public List<Combatant> Combatants {get;} = new List<Combatant>();
  public BattlePhase Phase {get;private set;} = BattlePhase.Declaration;
  public int RoundNumber {get;private set;}
  public int? WinningTeam {get;private set;}

  public double CritChance {get;set;} = 0.15;
  public double CritMultiplier {get;set;} = 1.5;

  public event Action<string> LogEmitted;

  private readonly Dictionary<int, BattleAction> _declared = new Dictionary<int, BattleAction>();
  private readonly List<Combatant> _pendingActors = new List<Combatant>();
  private readonly Random _rng;
  private int _nextId;

  public BattleControl(int seed = 12345)
  {
    _rng = new Random(seed);
  }

  //============================
  // Setup
  //============================

  public Combatant AddCombatant(string name, int teamId, int maxHP, int atk, int mag, int def, int spd, int eva)
  {
    var c = new Combatant(_nextId++, name, teamId, maxHP, atk, mag,def, spd, eva);
    Combatants.Add(c);
    return c;
  }

  //============================
  // Queries
  //============================

  public IEnumerable<Combatant> Living
    => Combatants.Where(c => c.IsAlive);

  public IEnumerable<Combatant> LivingOnTeam(int teamId)
    => Combatants.Where(c => c.TeamId == teamId && c.IsAlive);

  public IEnumerable<Combatant> LivingEnemiesOf(Combatant c)
    => Combatants.Where(o => o.TeamId != c.TeamId && o.IsAlive);

  public IEnumerable<Combatant> AwaitingDeclaration
    => Combatants.Where(c => c.CanAct && !_declared.ContainsKey(c.Id));

  public bool AllDeclared => !AwaitingDeclaration.Any();

  //============================
  // Phase 1 - Declaration
  //============================

  public void StartRound()
  {
    if (Phase == BattlePhase.Finished) return;

    RoundNumber++;
    _declared.Clear();
    _pendingActors.Clear();
    Phase = BattlePhase.Declaration;
    Log($"--- Round {RoundNumber} ---");
  }

  public bool Declare(BattleAction action)
  {
    if (Phase != BattlePhase.Declaration) return false;
    if (action?.Actor == null) return false;
    if (!action.Actor.CanAct) return false;

    _declared[action.Actor.Id] = action;
    Log($"Declared: {action}");
    return true;
  }

  public void DeclareForTeam(int teamId)
  {
    var needing = Combatants
      .Where(c => c.TeamId == teamId && c.CanAct && !_declared.ContainsKey(c.Id))
      .ToList();

    foreach (var c in needing)
      Declare(GetAIAction(c));
  }

  public BattleAction GetAIAction(Combatant actor)
  {
    var targets = LivingEnemiesOf(actor).ToList();
    if (targets.Count == 0) return BattleAction.Skip(actor);
    return BattleAction.Attack(actor, targets[_rng.Next(targets.Count)]);
  }
  
  //============================
  // Phase 2 - Resolution
  //============================

  public void BeginResolution()
  {
    if (Phase != BattlePhase.Declaration) return;

    _pendingActors.Clear();
    _pendingActors.AddRange(Combatants.Where(c => _declared.ContainsKey(c.Id)));
    Phase = BattlePhase.Resolution;
  }

  public bool ResolveNextAction()
  {
    if (Phase != BattlePhase.Resolution) return false;

    var actor = TakeNextActor();
    if (actor == null)
    {
      Phase = BattlePhase.Declaration;
      return false;
    }

    Execute(_declared[actor.Id]);
    CheckForDefeat();
    return true;
  }

  public void ResolveEntireRound()
  {
    while (ResolveNextAction()) {}
  }

  private Combatant TakeNextActor()
  {
    _pendingActors.RemoveAll(c => !c.CanAct);
    if (_pendingActors.Count == 0) return null;

    var best = _pendingActors[0];
    foreach (var c in _pendingActors)
    {
      if (c.Spd > best.Spd) best = c;
      else if (c.Spd == best.Spd && c.Id < best.Id) best = c;
    }

    _pendingActors.Remove(best);
    return best;
  }

  //============================
  // Phase 3 - Execution
  //============================

  private void Execute(BattleAction action)
  {
    switch (action.Type)
    {
      case BattleActionType.Attack:
        ExecuteAttack(action);
        break;

      case BattleActionType.Skip:
        Log($"{action.Actor.Name} holds.");
        break;
    }
  }

  private void ExecuteAttack(BattleAction action)
  {
    var attacker = action.Actor;
    var target = action.Target;

    if (target == null || !target.IsAlive)
    {
      Log($"{attacker.Name} attacks, but the target is already down. Nothing happens.");
      return;
    }

    int damage = attacker.Atk;
    bool crit = _rng.NextDouble() < CritChance;
    if (crit) damage = (int)Math.Round(damage*CritMultiplier);

    int dealt = target.TakeDamage(damage);
    Log(crit
      ? $"{attacker.Name} lands a critical hit on {target.Name} for {dealt}. ({target.CurrentHP}/{target.MaxHP})"
      : $"{attacker.Name} hits {target.Name} for {dealt}. ({target.CurrentHP}/{target.MaxHP})");
    
    if (!target.IsAlive)
      Log($"{target.Name} is defeated.");
  }

  //============================
  // Phase 4 - End Conditions
  //============================

  private void CheckForDefeat()
  {
    var survivingTeams = Combatants
      .Where(c => c.IsAlive)
      .Select(c => c.TeamId)
      .Distinct()
      .ToList();

    if (survivingTeams.Count > 1) return;

    Phase = BattlePhase.Finished;
    _pendingActors.Clear();
    WinningTeam = survivingTeams.Count == 1 ? survivingTeams[0] : (int?)null;

    Log(WinningTeam.HasValue
      ? $"Team {WinningTeam.Value} wins."
      : "Everyone is down. Draw.");
  }

  private void Log(string message) => LogEmitted?.Invoke(message);
}