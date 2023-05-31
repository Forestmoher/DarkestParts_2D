using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    //UI
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _hoverHighLight;
    [SerializeField] private GameObject _moveHighLight;
    [SerializeField] private GameObject _attackHightlight;
    [SerializeField] private GameObject _occupiedHighlight;

    private GridManager _gridManager;
    public Vector2 position;
    public Tile[] neighborTiles;
    public Actor occupiedActor;

    //pathfinding
    public int g, h;
    public int f { get { return g + h; } }
    public Tile previousTile;
    public bool isBlocked;

    public void Init(bool offset, Vector2 pos, GridManager gridManager)
    {
        neighborTiles = new Tile[4];
        _gridManager = gridManager;
        position = pos;
        if (offset) _renderer.color = _offsetColor;
        else _renderer.color = _baseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverHighLight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverHighLight.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _gridManager.SelectTile(this);
    }

    public void ResetHighlight()
    {
        _moveHighLight.SetActive(false);
        _attackHightlight.SetActive(false);
    }

    public void ShowMoveHighlight(bool show)
    {
        _moveHighLight.SetActive(show);
    }

    public void ShowAttackHighlight(bool show)
    {
        _attackHightlight.SetActive(show);
    }

    public void OnPlayerSelectMove(int distance)
    {
        if (distance == 0) return;

        if (!isBlocked) ShowMoveHighlight(true);

        GetNeighborTiles();

        foreach(var tile in neighborTiles)
        {
            if(tile == null) continue;
            tile.OnPlayerSelectMove(distance - 1);
        }
    }

    public void OnPlayerSelectAttack(int distance)
    {
        if (distance == 0) return;

        if(!isBlocked) ShowAttackHighlight(true);

        GetNeighborTiles();

        foreach (var tile in neighborTiles)
        {
            if (tile == null) continue;
            tile.OnPlayerSelectAttack(distance - 1);
        }
    }

    public Tile[] GetNeighborTiles()
    {
        var tempArray = new Tile[4];
        _gridManager.TryGetTile(position + Vector2.up, out tempArray[0]);
        _gridManager.TryGetTile(position + Vector2.down, out tempArray[1]);
        _gridManager.TryGetTile(position + Vector2.left, out tempArray[2]);
        _gridManager.TryGetTile(position + Vector2.right, out tempArray[3]);
        neighborTiles = tempArray;
        return tempArray;
    }

    public void OnActorEnter(Actor actor)
    {
        if(previousTile!= null) previousTile.OnActorExit();
        _occupiedHighlight.SetActive(true);
        occupiedActor = actor;
        isBlocked = true;
    }

    public void OnActorExit()
    {
        _occupiedHighlight.SetActive(false);
        occupiedActor = null;
        isBlocked = false;
    }

    public bool IsInRange(Tile tile, int range, bool inLine)
    {
        var diff = tile.position - position;
        var x = diff.normalized;
        var y = diff.magnitude;
        var z = diff.normalized.magnitude;
        if(inLine && range < diff.magnitude)
        {
            print("Targer too far");
            return false;
        }
        else if (range >= diff.magnitude)
        {
            print("Target in range");
            return true;
        }
        return false;
    }
}
