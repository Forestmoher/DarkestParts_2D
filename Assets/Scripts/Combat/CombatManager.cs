using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEditorInternal.Profiling;
using UnityEngine;
using UnityEngine.Events;

public enum CombatPhase { Move, Action, Flee, None}

public class CombatManager : MonoBehaviour
{
    public bool inCombat;
    public bool inMovePhase;
    public bool inActionPhase;
    public bool inFleePhase;
    public bool hasMoved;
    public bool hasTakenAction;
    public bool playerReady;
    public bool enemyReady;
    public bool isPlayerTurn;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PlayerController _player;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private CombatUI _combatUI;
    [SerializeField] private List<Actor> _actors;
    [SerializeField] private Actor _currentActor;

    public UnityEvent CombatStarted;
    public event Action<Actor> TurnStarted;

    #region Setup
    private void Start()
    {
        inCombat = false;
        inMovePhase = false;
        inActionPhase = false;
        inFleePhase = false;
        hasMoved = false;
        hasTakenAction = false;
        isPlayerTurn = false;
        playerReady = false;
        enemyReady = false;
    }

    private void OnEnable()
    {
        _combatUI.EnterMovePhase.AddListener(() => ChangePhase(CombatPhase.Move));
        _combatUI.EnterActionPhase.AddListener(() => ChangePhase(CombatPhase.Action));
        _combatUI.CancelCurrentPhase.AddListener(() => ChangePhase(CombatPhase.None));
        _combatUI.TryFleeCombat.AddListener(() => ChangePhase(CombatPhase.Flee));
        _gridManager.OnTileSelected.AddListener(OnTileSelected);
        _player.PlayerReady.AddListener(OnPlayerReady);
        _enemyController.EnemyReady.AddListener(OnEnemyReady);
        //_enemyController.MoveComplete.AddListener(OnEnemyReady);
    }

    private void OnDisable()
    {
        _combatUI.EnterMovePhase.RemoveListener(() => ChangePhase(CombatPhase.Move));
        _combatUI.EnterActionPhase.RemoveListener(() => ChangePhase(CombatPhase.Action));
        _combatUI.CancelCurrentPhase.RemoveListener(() => ChangePhase(CombatPhase.None));
        _combatUI.TryFleeCombat.RemoveListener(() => ChangePhase(CombatPhase.Flee));
        _gridManager.OnTileSelected.RemoveListener(OnTileSelected);
        _player.PlayerReady.RemoveListener(OnPlayerReady);
        _enemyController.EnemyReady.RemoveListener(OnEnemyReady);
    }

    public void EnterCombat()
    {
        inCombat = true;
        CombatStarted.Invoke();
        //actors = new List<Actor>();
    }

    private void OnPlayerReady()
    {
        _actors.Add(_player);
        playerReady = true;
        if (enemyReady) SetTurnOrder();
    }

    private void OnEnemyReady()
    {
        foreach(var actor in _enemyController.enemies)
        {
            _actors.Add(actor);
        }
        enemyReady = true;

        if (playerReady) SetTurnOrder();
    }

    //order actors list based on turn speed
    private void SetTurnOrder()
    {
        List<Actor> sortedList = _actors.OrderByDescending(actor => actor.stats.speed).ToList();
        _actors = sortedList;
        _currentActor = _actors.First();
        StartTurn();
    }
    #endregion Setup

    public void OnTileSelected()
    {
        if (inMovePhase && isPlayerTurn && !hasMoved)
        {
            _combatUI.ShowActionsContainer(true);
            _player.HandleGridMovement();
        }
        if(inActionPhase && isPlayerTurn && !hasTakenAction && _gridManager.selectedTile.occupiedActor != null)
        {
            hasTakenAction = true;
            HandleAttackAction(_player, _gridManager.selectedTile.occupiedActor);
        }
        CheckEndTurn();
    }

    //private void UpdateEnemyState(CombatPhase phase)
    //{
    //    switch (phase)
    //    {
    //        case CombatPhase.Move:
    //            if (inActionPhase) hasTakenAction = true;
    //            ChangePhase(CombatPhase.Move);
    //            break;
    //        case CombatPhase.Action:
    //            if (inMovePhase) hasMoved = true;
    //            ChangePhase(CombatPhase.Action);
    //            break;
    //    }
    //    CheckEndTurn();
    //}

    private void CheckEndTurn()
    {
        if(hasMoved && hasTakenAction)
        {
            ChangeActor();
        }
    }

    //move to actor script
    public void HandleAttackAction(Actor attacker, Actor target)
    {
        print("attacking");
        target.currentHealth -= attacker.stats.strength;
        hasTakenAction = true;
    }

    //cycle actors turn and raise event
    public void ChangeActor()
    {
        if(_currentActor == _actors.Last())
        {
            _currentActor = _actors.First();
        }
        else
        {
            _currentActor = _actors[_actors.IndexOf(_currentActor) + 1];
        }
        StartTurn();
    }

    private void StartTurn()
    {
        isPlayerTurn = _currentActor == _player;
        inMovePhase = false;
        inActionPhase = false;
        inFleePhase = false;
        hasMoved = false;
        hasTakenAction = false;
        TurnStarted?.Invoke(_currentActor);
    }

    //update active combat state
    public void ChangePhase(CombatPhase phase)
    {
        switch(phase)
        {
            case CombatPhase.None:
                inFleePhase = false;
                inMovePhase = false;
                inActionPhase = false;
                break;
            case CombatPhase.Move:
                inMovePhase = true;
                inActionPhase = false;
                break;
            case CombatPhase.Action:
                inMovePhase = false;
                inActionPhase = true;
                break;
            case CombatPhase.Flee:
                inFleePhase = true;
                inMovePhase = false;
                inActionPhase = false;
                break;
        }
    }
}
