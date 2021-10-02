using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
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
        
        BuildTilePrefabDictionary();
        _turnHandler = FindObjectOfType<TurnHandler>();
        
        BaseTile[] tilesToInit = FindObjectsOfType<BaseTile>();
        foreach (BaseTile tile in tilesToInit) {
            BaseTile maptile = tile as BaseTile;
            maptile.Init(this, _turnHandler);
        }
        Debug.Log("Initialized " + tilesToInit.Length + " tiles");
        
        _turnHandler.Init(this);
    }

    public Vector3 GetTileCentreAtPosition(Vector3 position) {
        Vector3Int cell = _grid.WorldToCell(position);
        Vector3 result = _grid.GetCellCenterWorld(cell);
        return result;
    }

    public BaseTile GetGenericTileAtPosition(Vector3 position) {
        RaycastHit hit;
        int mask = LayerMask.NameToLayer("Tiles");

        Vector3 origin = position + new Vector3(0, 5f, 0);
        Vector3 direction = new Vector3(0, -1f, 0);
        float distance = 5f;
        
        //Debug.DrawRay(origin,direction * distance, Color.green,60f);
        if (Physics.Raycast(origin, direction, out hit, distance)) {
            BaseTile tile = hit.collider.transform.GetComponent<BaseTile>();
            if (tile != null) {
                return tile;
            }
        }
        return null;
    }

    public BaseTile[] GetNeighbours(Vector3 position) {
        List<BaseTile> neighbours = new List<BaseTile>();
        
        Vector3Int centerCell = _grid.WorldToCell(position);

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) { 
                Vector3Int neighbourCell = new Vector3Int(x, y, 0);
            }
        }

        return neighbours.ToArray();
    }
    
    public Vector3[] GetNeighbourPositions(Vector3 position) {
        List<Vector3> neighbourPositions = new List<Vector3>();
        
        Vector3Int centerCell = _grid.WorldToCell(position);
        Debug.Log(centerCell);

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == -1 && y == -1) {
                    continue;
                }
                if (x == -1 && y == 1) {
                    continue;
                }
                if ((x == 0 && y == 0) == false) {
                    neighbourPositions.Add(_grid.GetCellCenterWorld(centerCell + new Vector3Int(x, y, 0)));
                }
            }
        }
        Debug.Log("Neighbours count: " + neighbourPositions.Count);
        return neighbourPositions.ToArray();
    }

    public Vector3 GetNeighbourPositionOfType(Vector3 center, string typeIndex) {
        Vector3[] neighbours = GetNeighbourPositions(center);
        
        List<Vector3> correctType = new List<Vector3>();
        
        foreach (Vector3 pos in neighbours) {
            BaseTile tile = GetGenericTileAtPosition(pos);
            if (tile != null && tile.GetTileIndex() == typeIndex) {
                correctType.Add(pos);
            }
        }
        Debug.Log("Neighbours of type " + typeIndex + " found " + correctType.Count);

        if (correctType.Count == 0) {
            return new Vector3(-1000,-1000,-1000);
        }
        
        Vector3 randPos = correctType[Random.Range(0,correctType.Count-1)];
        
        return randPos;
    }

    public void ChangeTileAtPosition(Vector3 position, string newTileIndex) {
        Debug.Log("changing tile");
        BaseTile oldTile = GetGenericTileAtPosition(position);
        GameObject prefab = GetTilePrefab(newTileIndex);

        Destroy(oldTile.gameObject);
        GameObject newGO = Instantiate(prefab, position, Quaternion.identity);
        BaseTile newTile = newGO.GetComponent<BaseTile>();
        newTile.Init(this,_turnHandler);
        newTile.Activate();
    }

    private void BuildTilePrefabDictionary() {
        _tilePrefabs = new Dictionary<string, GameObject>();
        foreach (GameObject go in resources.tilePrefabs) {
            BaseTile gt = go.GetComponent<BaseTile>();
            if (gt == null) {
                Debug.LogError("TilePrefab "+ go.name + " is missing BaseTile Component");
            }

            if (_tilePrefabs.ContainsKey(gt.GetTileIndex())) {
                Debug.LogError("Duplicate tile index found in res: " + gt.GetTileIndex());
            }
            _tilePrefabs.Add(gt.GetTileIndex(), go);
        }
    }
    
    public GameObject GetTilePrefab(string index) {
        if (_tilePrefabs.ContainsKey(index) == false) {
            Debug.LogError("No TilePrefab found with index " + index);
        }
        return _tilePrefabs[index];
    }
}
