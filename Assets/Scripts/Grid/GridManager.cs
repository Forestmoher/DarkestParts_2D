using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    //[SerializeField] private int _width, _height;
    //[SerializeField] private Transform _camTransform;
    //[SerializeField] private Tilemap _collisionMap;

    [SerializeField] private Tile _walkableTile, _obstacleTile;
    [SerializeField] private Tilemap _groundMap;
    [SerializeField] private GameObject _tilesContainer;
    public List<Tile> tiles;
    public Tile selectedTile;

    [SerializeField] private PlayerController _player;
    [SerializeField] private CombatUI _combatUI;
    [SerializeField] private CombatManager _combatManager;

    public UnityEvent OnTileSelected;

    void Start()
    {
        //GenerateGrid();
    }

    private void OnEnable()
    {
        _combatManager.CombatStarted.AddListener(GenerateGrid);
        _combatUI.CancelCurrentPhase.AddListener(ResetTileHighlight);
        _combatUI.TryFleeCombat.AddListener(TakeDownGrid);
    }

    private void OnDisable()
    {
        _combatManager.CombatStarted.RemoveListener(GenerateGrid);
        _combatUI.CancelCurrentPhase.RemoveListener(ResetTileHighlight);
        _combatUI.TryFleeCombat.RemoveListener(TakeDownGrid);
    }

    public void GenerateGrid()
    {
        foreach (var position in _groundMap.cellBounds.allPositionsWithin)
        {
            if (!_groundMap.HasTile(position))
            {
                continue;
            }
            var newTile = Instantiate(_walkableTile, new Vector3(position.x + .5f, position.y + .5f, position.z), Quaternion.identity);
            newTile.transform.SetParent(_tilesContainer.transform, false);
            newTile.name = $"{position.x},{position.y}";
            tiles.Add(newTile);

            var isOffset = (position.x % 2 == 0 && position.y % 2 != 0) || (position.x % 2 != 0 && position.y % 2 == 0);
            newTile.GetComponent<Tile>().Init(isOffset, new Vector2(position.x, position.y), this);
        }
    }

    private void TakeDownGrid()
    {
        foreach(var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
    }

    public bool TryGetTile(Vector2 position, out Tile tile)
    {
        var foundTile = tiles.Where(x => x.position == position).FirstOrDefault();
        if (foundTile)
        {
            tile = foundTile;
            return true;
        }
        else
        {
            tile = null;
            return false;
        }
    }

    public void SelectTile(Tile selectedTile)
    {
        //if the tile is blocked
        if (_combatManager.inMovePhase && selectedTile.isBlocked) return;
        //if there is no actor in the tile
        if (_combatManager.inActionPhase && selectedTile.occupiedActor == null) return;
        this.selectedTile = selectedTile;
        OnTileSelected.Invoke();
        ResetTileHighlight();
    }

    private void ResetTileHighlight()
    {
        foreach (var tile in tiles) tile.ResetHighlight();
    }
}
