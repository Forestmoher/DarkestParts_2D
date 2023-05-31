using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorStats stats;
    public int currentHealth;
    public bool isPlayer;
    public Tile currentTile;
    public ActorAnimator animator;
    public float stepSpeed;
    protected Rigidbody2D _rb;
    public Pathfinder pathfinder;
    public List<Tile> path;

    //temp for testing enemy AI
    public int atkRange = 2;

    protected void Awake()
    {
        currentHealth = stats.maxHealth;
        pathfinder = new Pathfinder();
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<ActorAnimator>();
    }

    //used if called from Update
    //public void MoveAlongPath()
    //{
    //    var step = moveSpeed * Time.fixedDeltaTime;
    //    _rb.position = Vector2.MoveTowards(_rb.position, path[0].transform.position, step);
    //    animator.SetAnimation(path[0].transform.position - _rb.transform.position);
    //    //snap actor to center of tile 
    //    if (Vector2.Distance(_rb.position, path[0].transform.position) < .0001f)
    //    {
    //        _rb.position = path[0].transform.position;
    //        currentTile = path[0];
    //        currentTile.OnActorEnter(this);
    //        //OnEnterTile?.Invoke(this);
    //        path.RemoveAt(0);
    //    }
    //}

    public async Task MoveAlongPath()
    {
        animator.SetAnimation(path[0].transform.position - _rb.transform.position);
        LeanTween.move(_rb.gameObject, path[0].transform.position, stepSpeed);
        await Task.Delay((int)(stepSpeed * 1000));
        currentTile = path[0];
        currentTile.OnActorEnter(this);
        path.RemoveAt(0);
    }

    //align actor to nearest tile 
    public void SnapToGrid(GridManager grid)
    {
        foreach (var tile in grid.tiles)
        {
            if ((_rb.transform.position - tile.transform.position).magnitude < .6f)
            {
                transform.position = tile.transform.position;
                currentTile = tile;
                currentTile.OnActorEnter(this);
                return;
            }
        }
    }

    public void Attack()
    {
        //do something
    }
}
