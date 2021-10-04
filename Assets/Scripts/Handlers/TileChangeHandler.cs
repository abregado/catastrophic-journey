using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Tilemap _selectionTilemap;
    private TurnHandler _turnHandler;
    private PlayerHandler _playerHandler;
    private Transform _playerObj;
    private Transform _cameraTrans;
    private Transform _lifeBar;
    public Res resources;
    
    private Dictionary<string, GameObject> _tilePrefabs;

    [SerializeField]
    private Transform _tileParent;

    private List<BaseTile> _tiles;

    void Start() {
        BuildTilePrefabDictionary();
        //Debug.Log(_tilePrefabs);
        _grid = GetComponent<Grid>();
        _tilemap = transform.Find("Tilemap").transform.GetComponent<Tilemap>();
        _selectionTilemap = transform.Find("SelectionTilemap").transform.GetComponent<Tilemap>();
        _playerHandler = FindObjectOfType<PlayerHandler>(); //get ref ro playerhandler
        _playerObj = GameObject.Find("PlayerObj").GetComponent<Transform>();
        _cameraTrans = Camera.main.transform;
        _turnHandler = FindObjectOfType<TurnHandler>();
        _lifeBar = GameObject.Find("LifeBar").GetComponent<Transform>();
        //Debug.Log(_lifeBar.position);
        _tiles = new List<BaseTile>();
        
        _turnHandler.Init(this, _playerHandler);
        _playerHandler.Init(_grid, _playerObj, _cameraTrans, _turnHandler, _selectionTilemap, this, _lifeBar); //pass ref to grid to playerhandler

        GenerateLevel();
        
        StartGame();
    }

    public void StartGame() {
        _turnHandler.StartGame();
        _playerHandler.StartGame();
    }

    public void RestartGame() {
        GenerateLevel();
        StartGame();
    }

    public Vector3 GetTileCentreAtCell(Vector3Int cell) {
        Vector3 result = _tilemap.GetCellCenterWorld(cell);
        return result;
    }
    
    public Vector3Int GetCellAtPosition(Vector3 position) {
        Vector3Int cell = _tilemap.WorldToCell(position);
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
    
    public BaseTile GetTileAtPositionByList(Vector3 pos) {
        return GetTileAtCellByList(_tilemap.WorldToCell(pos));
    }
    public Vector3 GetTileGridCoordinate(Vector3 position) {
        Vector3Int cell = _grid.WorldToCell(position);
        return cell;
    }

    public BaseTile[] GetNeighbourTiles(BaseTile tile) {
        List<BaseTile> neighbours = new List<BaseTile>();

        Vector3Int[] offsets = new[] {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, -1, 0),
        };

        if (tile.cellPosition.y % 2 != 0) {
            offsets = new[] {
                new Vector3Int(0, -1, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, -1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(1, 1, 0),
            };
        }
        
        foreach (Vector3Int offset in offsets) {
            Vector3Int cellHere = tile.cellPosition + offset;
            BaseTile tileHere = GetTileAtCellByList(cellHere);
            if (tileHere != null) {
                //Debug.Log(cellHere + " neighbour with tile " + tileHere.indexName);
                neighbours.Add(tileHere);
            }
        }

        return neighbours.ToArray();
    }

    public BaseTile GetRandomNeighbourTileOfTypes(BaseTile centerTile, string[] typeIndexes) {
        BaseTile[] correctType = GetAllNeighbourTilesOfTypes(centerTile, typeIndexes);
        
        if (correctType.Length == 0) {
            return null;
        }
        
        BaseTile randTile = correctType[Random.Range(0,correctType.Length-1)];
        
        return randTile;
    }

    public BaseTile GetRandomNeighbourTileExcludingTypes(BaseTile centerTile, string[] typeIndexes) {
        List<string> types = new List<string>();
        
        foreach (GameObject prefab in resources.tilePrefabs) {
            BaseTile tile = prefab.GetComponent<BaseTile>();
            types.Add(tile.indexName);
        }
        foreach (String typestring in typeIndexes)
        {
            types.Remove(typestring);
        }


        return GetRandomNeighbourTileOfTypes(centerTile, types.ToArray());
    }
    
    public bool IsOfWantedType(BaseTile tile, string[] wantedTypes) {
        foreach (string indexName in wantedTypes) {
            if (tile.indexName == indexName) {
                return true;
            }
        }

        return false;
    }

    public BaseTile[] GetAllNeighbourTilesOfTypes(BaseTile centerTile, string[] typeIndexes) {
        BaseTile[] neighbours = GetNeighbourTiles(centerTile);
        
        List<BaseTile> correctType = new List<BaseTile>();
        
        foreach (BaseTile nTile in neighbours) {
            if (IsOfWantedType(nTile,typeIndexes)) 
            {
                correctType.Add(nTile);
            }
        }
        
        return correctType.ToArray();
    }

    public BaseTile[] GetAllNeighbourTilesExcluding(BaseTile center, string[] typeIndexes) {
        List<string> types = new List<string>();
        
        foreach (GameObject prefab in resources.tilePrefabs) {
            BaseTile tile = prefab.GetComponent<BaseTile>();
            types.Add(tile.indexName);
        }
        foreach (String typestring in typeIndexes)
        {
            types.Remove(typestring);
        }


        return GetAllNeighbourTilesOfTypes(center, types.ToArray());
    }

    public BaseTile[] FloodFillWalkable(BaseTile start, int steps) {
        Dictionary<Vector3Int, int> filled = new Dictionary<Vector3Int, int>();
        filled.Add(start.cellPosition,steps);

        filled = WalkableStep(filled, start, steps);

        List<BaseTile> finishedList = new List<BaseTile>();
        foreach (var pair in filled) {
            finishedList.Add(GetTileAtCellByList(pair.Key));
        }

        return finishedList.ToArray();
    }

    private Dictionary<Vector3Int, int> WalkableStep(Dictionary<Vector3Int, int> dictionary, BaseTile start, int stepsRemaining) {
        
        BaseTile[] neighbours = GetNeighbourTiles(start);
        foreach (BaseTile ntile in neighbours) {
            if (ntile.isWalkable && stepsRemaining - ntile.moveCost > 0) {
                if (stepsRemaining > 0) {
                    if (dictionary.ContainsKey(ntile.cellPosition) == false) {
                        dictionary.Add(ntile.cellPosition, stepsRemaining - ntile.moveCost);
                        dictionary = WalkableStep(dictionary, ntile, stepsRemaining - ntile.moveCost);

                    }
                    else if (dictionary[ntile.cellPosition] < stepsRemaining - ntile.moveCost) {
                        dictionary[ntile.cellPosition] = stepsRemaining - ntile.moveCost;
                        dictionary = WalkableStep(dictionary, ntile, stepsRemaining - ntile.moveCost);
                    }
                }
            }
        }

        return dictionary;
    }

    public BaseTile ChangeTileAtCell(Vector3Int cell, string newTileIndex, bool skipActivate = false) {
        //Debug.Log("Changing tile to " + newTileIndex);
        BaseTile oldTile = GetTileAtCellByList(cell);
        
        if (oldTile != null) {
            _tiles.Remove(oldTile);
            //Debug.Log("destroying old tile");
            Destroy(oldTile.gameObject);    
        }

        if (newTileIndex != "destroy") {
            GameObject prefab = GetTilePrefab(newTileIndex);
            GameObject newGO = Instantiate(prefab, _tileParent);
            newGO.transform.SetPositionAndRotation(_grid.GetCellCenterWorld(cell), quaternion.identity);
            BaseTile newTile = newGO.GetComponent<BaseTile>();
            newTile.Init(this, _turnHandler, _playerHandler);
            if (skipActivate == false) {
                newTile.Activate();
            }

            _tiles.Add(newTile);
            return newTile;
        }

        return null;
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
        ClearAllTiles();
        
        List<BaseTile> waterStarts = new List<BaseTile>();
        List<BaseTile> mountainTiles = new List<BaseTile>();
        List<BaseTile> forestTiles = new List<BaseTile>();
        
        //Basic generation
        List<String> pool = new List<string>();

        foreach (GameObject prefab in resources.tilePrefabs) {
            BaseTile tile = prefab.GetComponent<BaseTile>();
            if (tile.disasterWeight == 0 && tile.generatedWeight > 0) {
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
                BaseTile tile = ChangeTileAtCell(newCell,randomType,true);
                //Debug.Log("Made new "+ tile.indexName +" at cell " + tile.cellPosition);
                if (randomType == "water") {
                    waterStarts.Add(tile);
                }
                if (randomType == "mountain") {
                    mountainTiles.Add(tile);
                }
                if (randomType == "forest") {
                    forestTiles.Add(tile);
                }
            }
        }
        
        //Add hills around Mountains
        ApplyHills(mountainTiles.ToArray());
        
        //Water worming
        
        CreateRivers(waterStarts.ToArray());
        ApplyMoisture();
        
        //Forest spreading

        GrowForests(forestTiles.ToArray());
        //test rocket

        ChangeTileAtCell(new Vector3Int(-238,3,0), "rocket",true);

        ActivateAllTiles();
    }

    private void ClearAllTiles() {
        for (int i = _tileParent.childCount-1; i >= 0; i--) {
            var go = _tileParent.gameObject.transform.GetChild(i);
            Destroy(go.gameObject);
        }
        _tiles.Clear();
        _turnHandler.ClearEvents();
    }

    private void ActivateAllTiles() {
        foreach (BaseTile tile in _tiles) {
            tile.Activate();
        }
    }

    private void GrowForests(BaseTile[] forestTiles) {
        foreach (BaseTile tile in forestTiles) {
            SpreadTrees(tile,9);
        }
    }
    
    private void SpreadTrees(BaseTile tile, int stepsRemaining) {
        string[] allowedDestinations = {"grass"};
        
        BaseTile nextFlow = GetRandomNeighbourTileOfTypes(tile, allowedDestinations);
        if (nextFlow != null) {
            ChangeTileAtCell(nextFlow.cellPosition, "forest", true);
            if (stepsRemaining > 0) {
                SpreadTrees(nextFlow, stepsRemaining - 1);
            }
        }
    }

    private void CreateRivers(BaseTile[] waterStarts) {
        foreach (BaseTile tile in waterStarts) {
            FlowWater(tile,6);
        }
    }
    private void FlowWater(BaseTile tile, int stepsRemaining) {
        string[] allowedDestinations = {"desert","grass"};
        
        BaseTile nextFlow = GetRandomNeighbourTileOfTypes(tile, allowedDestinations);
        if (nextFlow != null) {
            ChangeTileAtCell(nextFlow.cellPosition, "water", true);
            if (stepsRemaining > 0) {
                FlowWater(nextFlow, stepsRemaining - 1);
            }
        }
    }
    
    private void ApplyMoisture() {
        List<BaseTile> waterTiles = new List<BaseTile>();
        
        foreach (BaseTile tile in _tiles) {
            if (tile.indexName == "water") {
                waterTiles.Add(tile);
            }
        }

        foreach (BaseTile waterTile in waterTiles) {
            BaseTile[] neighbours = GetAllNeighbourTilesOfTypes(waterTile,new[] {"desert"});
            foreach (BaseTile waterNeighbour in neighbours) {
                ChangeTileAtCell(waterNeighbour.cellPosition, "grass", true);
            }
        }
    }
    
    private void ApplyHills(BaseTile[] tiles) {
        foreach (BaseTile mountainTile in tiles) {
            BaseTile[] neighbours = GetNeighbourTiles(mountainTile);
            foreach (BaseTile tile in neighbours) {
                if (IsOfWantedType(tile,new []{"grass","desert"})&&Random.Range(0,100)<50)
                {
                    ChangeTileAtCell(tile.cellPosition, "hill",true);
                }
            }
        }
    }

    public Vector3Int GetRandomStripPos(int centerX) {
        _tilemap.CompressBounds();
        BoundsInt bounds = _tilemap.cellBounds;
        int spawnX = centerX + Random.Range(-2, 2);
        int spawnY = Random.Range(bounds.yMin, bounds.yMax);

        return new Vector3Int(spawnX, spawnY, 0);
    }

    public string GetRandomDisaster() {
        List<string> disasters = new List<string>();
        foreach (GameObject prefab in resources.tilePrefabs) {
            BaseTile tile = prefab.GetComponent<BaseTile>();
            if (tile.disasterWeight>0) {
                for (int i = 0; i < tile.disasterWeight; i++) {
                    disasters.Add(tile.indexName);
                }
            }
        }

        return disasters[Random.Range(0, disasters.Count - 1)];
    }

    public void StopAllTileSounds() {
        foreach (BaseTile tile in _tiles) {
            tile.StopSound();
        }
    }
}

