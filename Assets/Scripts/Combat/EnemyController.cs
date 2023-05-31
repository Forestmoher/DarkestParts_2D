using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    public List<Enemy> enemies;
    private Actor _currentEnemy;
    //private Dictionary<Enemy, Tile> _enemyTiles;
    [SerializeField] private PlayerController _player;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private CombatManager _combatManager;

    public UnityEvent EnemyReady;
    public event Action<CombatPhase> ChangeState;

    #region Setup
    private void OnEnable()
    {
        _combatManager.CombatStarted.AddListener(OnCombatStart);
        _combatManager.TurnStarted += HandleEnemyTurn;
    }

    private void OnDisable()
    {
        _combatManager.CombatStarted.RemoveListener(OnCombatStart);
        _combatManager.TurnStarted -= HandleEnemyTurn;
    }

    private async void OnCombatStart()
    {
        await Task.Delay(100);
        SnapToGrid();
        EnemyReady.Invoke();
    }

    //align enemies to nearest tile 
    private void SnapToGrid()
    {
        foreach (var enemy in enemies)
        {
            enemy.SnapToGrid(_gridManager);
        }
    }
    #endregion Setup

    private void FixedUpdate()
    {
        //if (_currentEnemy == null) return;
        //MoveEnemy();
    }

    private void HandleEnemyTurn(Actor actor)
    {
        if (actor.isPlayer) return;
        _currentEnemy = actor;

        MakeDecision();
        ////if player not in range, move towards
        //if(!_player.currentTile.IsInRange(actor.currentTile, actor.atkRange, true) && !_combatManager.hasMoved)
        //{
        //    DecideMove();
        //}
        ////if in range, attack
        //if(_player.currentTile.IsInRange(actor.currentTile, actor.atkRange, true) && !_combatManager.hasTakenAction)
        //{
        //    DecideAttack();
        //}
    }

    private async void MakeDecision()
    {
        //is player in range
        if(_player.currentTile.IsInRange(_currentEnemy.currentTile, _currentEnemy.atkRange, true))
        {
            await DecideAttack();
            _combatManager.ChangeActor();
        }
        else
        {
            if(_combatManager.hasMoved)
            {
                _combatManager.ChangeActor();
            }
            else
            {
                await DecideMove();
                MakeDecision();
            }
        }
    }

    private async Task DecideMove()
    {
        print("decide move");
        _combatManager.inMovePhase = true;
        _currentEnemy.pathfinder = new();

        //currently choosing one of the players neighbor tiles to move to. Need to find a better way to do this
        var rnd = UnityEngine.Random.Range(0, 4);
        _currentEnemy.path = _currentEnemy.pathfinder.FindPath(_currentEnemy.currentTile, _player.currentTile.GetNeighborTiles()[rnd]);

        GetActualPath();

        await MoveEnemy();
    }

    //limits path to length of actors speed
    private void GetActualPath()
    {
        var newPath = new List<Tile>();
        for (var i = 0; i < _currentEnemy.stats.speed; i++)
        {
            if (newPath.Count == _currentEnemy.path.Count) break;
            newPath.Add(_currentEnemy.path[i]);
        }
        _currentEnemy.path = newPath;
    }

    private async Task MoveEnemy()
    {
        while (_currentEnemy.path.Count > 0)
        {
            await _currentEnemy.MoveAlongPath();
            //if enemy is in range before reaching end of path, stop pathing
            if (_player.currentTile.IsInRange(_currentEnemy.currentTile, _currentEnemy.atkRange, true))
            {
                _currentEnemy.path.Clear();
            }
        }
        _combatManager.hasMoved = true;
        _currentEnemy.animator.SetAnimation(Vector2.zero);
        await Task.Yield();
    }

    private async Task DecideAttack()
    {
        print("decide attack");
        _combatManager.inActionPhase = true;
        _combatManager.HandleAttackAction(_currentEnemy, _player);
        await Task.Yield();
    }
}
