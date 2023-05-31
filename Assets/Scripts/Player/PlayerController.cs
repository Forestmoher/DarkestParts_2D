using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : Actor
{
    //input 
    private PlayerInput _playerInput;
    private InputAction _upAction;
    private InputAction _downAction;
    private InputAction _leftAction;
    private InputAction _rightAction;

    //free movement
    private Vector2 _direction;
    [SerializeField] private float _freeMoveSpeed;
    [SerializeField] private Transform _visionCone;
    private float _visonRotateSpeed = .1f;

    //combat
    [SerializeField] private bool _inCombat;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private CombatUI _combatUI;

    public UnityEvent PlayerReady;

    private void Awake()
    {
        base.Awake();
        isPlayer = true;
        _inCombat = false;
        _playerInput = GetComponent<PlayerInput>();
        _upAction = _playerInput.actions["Up"];
        _downAction = _playerInput.actions["Down"];
        _leftAction = _playerInput.actions["Left"];
        _rightAction = _playerInput.actions["Right"];
        //_animator = GetComponentInChildren<ActorAnimator>();
    }

    private void OnEnable()
    {
        _combatManager.CombatStarted.AddListener(OnEnterCombat);
        _combatUI.EnterMovePhase.AddListener(OnPlayerSelectMove);
        _combatUI.EnterActionPhase.AddListener(OnPlayerSelectAttack);
        _combatUI.TryFleeCombat.AddListener(OnExitCombat);
    }

    private void OnDisable()
    {
        _combatManager.CombatStarted.RemoveListener(OnEnterCombat);
        _combatUI.EnterMovePhase.RemoveListener(OnPlayerSelectMove);
        _combatUI.EnterActionPhase.RemoveListener(OnPlayerSelectAttack);
        _combatUI.TryFleeCombat.RemoveListener(OnExitCombat);
    }

    void Update()
    {
        //check if any direction key is pressed, and set direction value
        if (_upAction.IsPressed()) _direction = Vector2.up;
        else if (_downAction.IsPressed()) _direction = Vector2.down;
        else if (_leftAction.IsPressed()) _direction = Vector2.left;
        else if (_rightAction.IsPressed()) _direction = Vector2.right;
        else _direction = Vector2.zero;
        //HandleVisionConeRotation();
    }

    private void FixedUpdate()
    {
        //if(!gameManager.inGame) return;
        if (!_inCombat)
        {
            MovePlayer(_direction);
            animator.SetAnimation(_direction);
        }
    }

    //free movement
    private void MovePlayer(Vector2 direction)
    {
        _rb.MovePosition(_rb.position + direction * _freeMoveSpeed * Time.fixedDeltaTime);

        HandleVisionConeRotation();
        //gameManager.audioManager.PlaySound("Walk grass");
    }

    private void HandleVisionConeRotation()
    {
        if (_direction == Vector2.up)
        {
            LeanTween.rotate(_visionCone.gameObject, new Vector3(0,0,0), _visonRotateSpeed);
        }
        if (_direction == Vector2.down)
        {
            LeanTween.rotate(_visionCone.gameObject, new Vector3(0, 0, -180), _visonRotateSpeed);
        }
        if (_direction == Vector2.left)
        {
            LeanTween.rotate(_visionCone.gameObject, new Vector3(0, 0, 90), _visonRotateSpeed);
        }
        if (_direction == Vector2.right)
        {
            LeanTween.rotate(_visionCone.gameObject, new Vector3(0, 0, -90), _visonRotateSpeed);
        }
    }

    #region Combat Movement

    public void OnEnterCombat()
    {
        _inCombat = true;

        //prevent WASD movement
        _playerInput.enabled = false;

        SnapToGrid(_gridManager);

        //initialize list for pathfinding
        path = new List<Tile>();

        PlayerReady.Invoke();
    }

    public void OnExitCombat()
    {
        //enable WASD movement
        _inCombat = false;
        _playerInput.enabled = true;
    }

    //public void OnStartTurn(Actor actor)
    //{
    //    if (!actor.isPlayer) return;
    //}

    public void OnPlayerSelectMove()
    {
        currentTile.OnPlayerSelectMove(stats.speed + 1); //plus one to account for current standing tile
        #region this is only checking tiles in a straight line
        ////loop through all adjacent tiles and increment their distance variable by one
        //for (var i = 0; i < _moveDistance; i++)
        //{
        //    if (_gridManager.TryGetTile(_currentTile.position + new Vector2(0, i) + Vector2.up, out Tile upTile))
        //    {
        //        print(upTile.position);
        //        upTile.ShowMoveHighlight(true);
        //    }
        //    if (_gridManager.TryGetTile(_currentTile.position + new Vector2(0, -i) + Vector2.down, out Tile downTile))
        //    {
        //        print(downTile.position);
        //        downTile.ShowMoveHighlight(true);
        //    }
        //    if (_gridManager.TryGetTile(_currentTile.position + new Vector2(i, 0) + Vector2.right, out Tile rightTile))
        //    {
        //        print(rightTile.position);
        //        rightTile.ShowMoveHighlight(true);
        //    }
        //    if (_gridManager.TryGetTile(_currentTile.position + new Vector2(-i, 0) + Vector2.left, out Tile leftTile))
        //    {
        //        print(leftTile.position);
        //        leftTile.ShowMoveHighlight(true);
        //    }
        //}
        #endregion
    }

    private void OnPlayerSelectAttack()
    {
        currentTile.OnPlayerSelectAttack(2); //change this to determine distance based on action
    }

    public async void HandleGridMovement()
    {
        path = pathfinder.FindPath(currentTile, _gridManager.selectedTile);
        while(path.Count > 0)
        {
            await MoveAlongPath();
        }
        animator.SetAnimation(Vector2.zero);
        _combatManager.hasMoved = true;
    }

    #endregion Combat Movement
}
