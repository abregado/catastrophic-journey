using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

public class TileChangeHandler : MonoBehaviour {
    private Grid _grid;
    private Tilemap _tilemap;
    private TurnHandler _turnHandler;
    public Res resources;
    
    private Dictionary<string, GameObject> _tilePrefabs;

    void Start() {
        _grid = GetComponent<Grid>();
        _tilemap = transform.Find("Tilemap").transform.GetComponent<Tilemap>();
        _turnHandler = FindObjectOfType<TurnHandler>();
        
        BaseTile[] tilesToInit = FindObjectsOfType<BaseTile>();
        foreach (BaseTile tile in tilesToInit) {
            IMapTile maptile = tile as IMapTile;
            maptile.Init(this, _turnHandler);
        }
    }

    public Vector3 GetTileCentreAtPosition(Vector3 position) {
        Vector3Int cell = _tilemap.WorldToCell(position);
        Vector3 result = _tilemap.GetCellCenterWorld(cell);
        return result;
    }

    public IMapTile GetGenericTileAtPosition(Vector3 position) {
        RaycastHit hit;
        if (Physics.Raycast(
                position + new Vector3(0, 3, 0), 
                transform.TransformDirection(Vector3.forward), 
                out hit,
                5f)) {
            IMapTile tile = hit.collider.transform.GetComponent<IMapTile>();
            if (tile != null) {
                return tile;
            }
        }

        return null;
    }

    public IMapTile[] GetNeighbours(Vector3 position) {
        List<IMapTile> neighbours = new List<IMapTile>();
        
        Vector3Int centerCell = _tilemap.WorldToCell(position);

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) { 
                Vector3Int neighbourCell = new Vector3Int(x, y, 0);
            }
        }

        return neighbours.ToArray();
    }
    
    public Vector3[] GetNeighbourPositions(Vector3 position) {
        List<Vector3> neighbourPositions = new List<Vector3>();
        
        Vector3Int centerCell = _tilemap.WorldToCell(position);

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x != centerCell.x && y != centerCell.y) {
                    _tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                }
            }
        }

        return neighbourPositions.ToArray();
    }

    public Vector3 GetNeighbourPositionOfType(Vector3 center, string typeIndex) {
        Vector3[] neighbours = GetNeighbourPositions(transform.position);
        
        List<Vector3> correctType = new List<Vector3>();

        foreach (Vector3 pos in neighbours) {
            IMapTile tile = GetGenericTileAtPosition(pos);
            if (tile.GetTileIndex() == typeIndex) {
                correctType.Add(pos);
            }
        }

        if (correctType.Count == 0) {
            return Vector3.negativeInfinity;
        }

        Vector3 randPos = correctType[Random.Range(0,correctType.Count-1)];
        return randPos;
    }

    public void ChangeTileAtPosition(Vector3 position, string newTileIndex) {
        Debug.Log("changing tile");
    }

    private void BuildTilePrefabDictionary() {
        foreach (GameObject go in resources.tilePrefabs) {
            IMapTile gt = go.GetComponent<IMapTile>();
            if (gt == null) {
                Debug.LogError("TilePrefab "+ go.name + " is missing GenericTile Component");
            }
            _tilePrefabs.Add(gt.GetTileIndex(),go);
        }
    }
    
    public GameObject GetTilePrefab(string index) {
        if (_tilePrefabs.ContainsKey(index) == false) {
            Debug.LogError("No TilePrefab found with index " + index);
        }
        return _tilePrefabs[index];
    }
}
