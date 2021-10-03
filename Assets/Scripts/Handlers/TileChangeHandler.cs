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
    private PlayerHandler _playerHandler;
    private Transform _playerObj;
    public Res resources;
    
    private Dictionary<string, GameObject> _tilePrefabs;

    [SerializeField]
    private Transform _tileParent;

    private List<BaseTile> _tiles;

    void Start() {
        _grid = GetComponent<Grid>();
        _tilemap = transform.Find("Tilemap").transform.GetComponent<Tilemap>();
        _tiles = new List<BaseTile>();
        
        _playerObj = GameObject.Find("PlayerObj").GetComponent<Transform>();
        _playerHandler = FindObjectOfType<PlayerHandler>(); //get ref ro playerhandler
        _playerHandler.init(_grid, _playerObj); //pass ref to grid to playerhandler

        BuildTilePrefabDictionary();
        _turnHandler = FindObjectOfType<TurnHandler>();
         
        BaseTile[] tilesToInit = FindObjectsOfType<BaseTile>();
        foreach (BaseTile tile in tilesToInit) {
            BaseTile maptile = tile;
            maptile.Init(this, _turnHandler, _playerHandler);
            _tiles.Add(maptile);
        }
        Debug.Log("Initialized " + tilesToInit.Length + " tiles");
        
        _turnHandler.Init(this);
        
        GenerateLevel();
    }

    public Vector3 GetTileCentreAtPosition(Vector3 position) {
        Vector3Int cell = _grid.WorldToCell(position);
        Vector3 result = _grid.GetCellCenterWorld(cell);
        return result;
    }
    
    public Vector3Int GetCellAtPosition(Vector3 position) {
        Vector3Int cell = _grid.WorldToCell(position);
        return cell;
    }

    public BaseTile GetTileAtCellByList(Vector3Int cell) {
        foreach (BaseTile tile in _tiles) {
            if (tile.cellPosition == cell) {
                return tile;
            }
        }

        return null;
    }
    
    public BaseTile GetTileAtPositionByList(Vector3 position) {
        Vector3Int cell = _grid.WorldToCell(position);
        foreach (BaseTile tile in _tiles) {
            if (tile.cellPosition == cell) {
                return tile;
            }
        }
        return null;
    }
    public Vector3 GetTileGridCoordinate(Vector3 position) {
        Vector3Int cell = _grid.WorldToCell(position);
        return cell;
    }

    public BaseTile GetGenericTileAtPosition(Vector3 position, bool debug = true) {
        RaycastHit hit;
        int mask = 1 << 6;

        Vector3 origin = position + new Vector3(0, 5f, 0);
        Vector3 direction = new Vector3(0, -1f, 0);
        float distance = 6f;

        if (debug) {
            Debug.DrawRay(origin,direction * distance, Color.red,5f);    
        }
        if (Physics.Raycast(origin, direction, out hit, distance)) {
            Debug.Log("hit");
            BaseTile tile = hit.collider.transform.GetComponent<BaseTile>();
            if (tile != null) {
                return tile;
            }
        }
        return null;
    }

    public BaseTile[] GetNeighbour(Vector3 position) {
        List<BaseTile> neighbours = new List<BaseTile>();
        
        Vector3Int centerCell = _grid.WorldToCell(position);

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) { 
                Vector3Int neighbourCell = new Vector3Int(x, y, 0);
            }
        }

        return neighbours.ToArray();
    }

    public Vector3Int[] GetNeighbourCells(Vector3Int cell) {
        Vector3[] neighbours = GetNeighbourPositions(_grid.GetCellCenterWorld(cell));
        Debug.Log(neighbours.Length + " neighbours found");
        return new []{Vector3Int.one};
    }
    
    public Vector3[] GetNeighbourPositions(Vector3 position) {
        List<Vector3> neighbourPositions = new List<Vector3>();
        
        Vector3Int centerCell = _grid.WorldToCell(position);

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
        //Debug.Log("Neighbours count: " + neighbourPositions.Count);
        return neighbourPositions.ToArray();
    }

    public Vector3 GetNeighbourPositionOfTypes(Vector3 center, string[] typeIndexes) {
        Vector3[] neighbours = GetNeighbourPositions(center);
        
        List<Vector3> correctType = new List<Vector3>();
        
        foreach (Vector3 pos in neighbours) {
            BaseTile tile = GetGenericTileAtPosition(pos);
            if (tile != null) {
                bool result = false;
                string tileIndex = tile.GetTileIndex();

                foreach (string index in typeIndexes) {
                    if (tileIndex == index) {
                        result = true;
                    }
                }    
                
                if (result) {
                    correctType.Add(pos);
                }
            }
        }

        if (correctType.Count == 0) {
            return new Vector3(-1000f,-1000f,-1000f);
        }
        
        Vector3 randPos = correctType[Random.Range(0,correctType.Count-1)];
        
        return randPos;
    }
    
    public Vector3[] GetAllNeighbourPositionsOfTypes(Vector3 center, string[] typeIndexes) {
        Vector3[] neighbours = GetNeighbourPositions(center);
        
        List<Vector3> correctType = new List<Vector3>();
        
        foreach (Vector3 pos in neighbours) {
            BaseTile tile = GetGenericTileAtPosition(pos);
            if (tile != null) {
                bool result = false;
                string tileIndex = tile.GetTileIndex();

                foreach (string index in typeIndexes) {
                    if (tileIndex == index) {
                        result = true;
                    }
                }    
                
                if (result) {
                    correctType.Add(pos);
                }
            }
        }

        return correctType.ToArray();
    }

    public Vector3Int[] GetAllNeighbourCellsOfTypes(Vector3Int center, string[] typeIndexes) {
        Vector3Int[] neighbours = GetNeighbourCells(center);
        
        List<Vector3Int> correctType = new List<Vector3Int>();
        
        foreach (Vector3Int pos in neighbours) {
            BaseTile tile = GetTileAtCellByList(pos);
            if (tile != null) {
                bool result = false;
                string tileIndex = tile.GetTileIndex();

                foreach (string index in typeIndexes) {
                    if (tileIndex == index) {
                        result = true;
                    }
                }    
                
                if (result) {
                    correctType.Add(_grid.WorldToCell(pos));
                }
            }
        }

        return correctType.ToArray();
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
        //Debug.Log("Neighbours of type " + typeIndex + " found " + correctType.Count);

        if (correctType.Count == 0) {
            return new Vector3(-1000f,-1000f,-1000f);
        }
        
        Vector3 randPos = correctType[Random.Range(0,correctType.Count-1)];
        
        return randPos;
    }

    public void ChangeTileAtPosition(Vector3 position, string newTileIndex) {
        //Debug.Log("Changing tile to " + newTileIndex);
        BaseTile oldTile = GetTileAtPositionByList(position);
        
        if (oldTile != null) {
            Debug.Log("destroying old tile");
            Destroy(oldTile.gameObject);    
        }

        if (newTileIndex != "destroy") {
            GameObject prefab = GetTilePrefab(newTileIndex);
            GameObject newGO = Instantiate(prefab, _tileParent);
            newGO.transform.SetPositionAndRotation(position, quaternion.identity);
            BaseTile newTile = newGO.GetComponent<BaseTile>();
            newTile.Init(this, _turnHandler, _playerHandler));
            newTile.Activate();
        }
    }

    public void ChangeTileAtCell(Vector3Int cell, string newTileIndex) {
        Vector3 position = _grid.GetCellCenterWorld(cell);
        ChangeTileAtPosition(position,newTileIndex);
        Destroy(oldTile.gameObject);
        GameObject newGO = Instantiate(prefab, position, Quaternion.identity);
        BaseTile newTile = newGO.GetComponent<BaseTile>();
        newTile.Init(this,_turnHandler, _playerHandler);
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


    private void GenerateLevel() {
        List<Vector3Int> waterStarts = new List<Vector3Int>();
        List<Vector3Int> waterTiles = new List<Vector3Int>();
        List<Vector3Int> mountainTiles = new List<Vector3Int>();
        
        //Basic generation
        List<String> pool = new List<string>();

        foreach (GameObject prefab in resources.tilePrefabs) {
            BaseTile tile = prefab.GetComponent<BaseTile>();
            if (tile.isDisaster == false && tile.generatedWeight > 0) {
                for (int i = 0; i < tile.generatedWeight; i++) {
                    pool.Add(tile.indexName);
                }
            }
        }

        _tilemap.CompressBounds();
        BoundsInt bounds = _tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                string randomType = pool[Random.Range(0, pool.Count)];
                Vector3Int newCell = new Vector3Int(x, y, 0);
                ChangeTileAtCell(newCell,randomType);
                if (randomType == "water") {
                    waterStarts.Add(newCell);
                }
                if (randomType == "mountain") {
                    mountainTiles.Add(newCell);
                }
            }
        }
        
        //Add hills around Mountains
        ApplyHills(mountainTiles.ToArray());
        
        //Water worming
        
        
        ApplyMoisture(waterStarts.ToArray());
        ApplyMoisture(waterTiles.ToArray());
        
        //Forest spreading
    }

    private void FlowWater(Vector3Int cell, int stepsRemaining) {
        string[] allowedDestinations = {"desert","grass"};
    }
    
    private void ApplyMoisture(Vector3Int[] cells) {
        foreach (Vector3Int cell in cells) {
            string[] allowedDestinations = {"desert"};
            Vector3Int[] neighbours = GetAllNeighbourCellsOfTypes(cell, allowedDestinations);

            foreach (Vector3Int ncell in neighbours) {
                ChangeTileAtCell(ncell, "grass");
            }
        }
    }
    
    private void ApplyHills(Vector3Int[] cells) {
        Debug.Log(cells.Length);
        foreach (Vector3Int cell in cells) {
            ChangeTileAtCell(cell,"destroy");
        }
        
    }
}

