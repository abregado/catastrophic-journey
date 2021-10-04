using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerHandler : MonoBehaviour
{
    private BaseTile mousedOverTile; //hold ref to tile we are mousing over
    private Vector3 tilePos;
    private Vector3 gridPos;
    private Grid _grid;
    private Transform _playerObj;
    private Transform _cameraTrans;
    private Vector3 camOffset;
    private Vector3 lifeOffset;
    private TurnHandler _turnHandler;
    private Tilemap _selectionTilemap;
    private BaseTile[] _selectableTiles;
    private TileChangeHandler _changeHandler;
    private Transform _lifeBar;
    private Rocket _rocket;
    private int _health = 5;

    private const int PLAYER_SPEED = 5; 
    
    public void Init(Grid grid, Transform playerObj, Transform cameraTrans, TurnHandler turnHandler,Tilemap selectionTilemap, TileChangeHandler changeHandler, Transform lifeBar)
    {
        _grid = grid;
        _playerObj = playerObj;
        _cameraTrans = cameraTrans;
        _turnHandler = turnHandler;
        _lifeBar = lifeBar;
        _selectionTilemap = selectionTilemap;
        camOffset = _cameraTrans.position - _playerObj.position;
        lifeOffset = _lifeBar.position - _playerObj.position;
        _selectableTiles = new BaseTile[]{};
        _changeHandler = changeHandler;
        
    }
    
    //set ref to tile we are mousing over
    public void SetMousedTile(BaseTile inputTile)
    {
        mousedOverTile = inputTile;
    }

    public void StartGame() {
        UpdateWalkableSelectionArea();
        _health = 5;
        UpdateLifeBar();
    }

    private void UpdateWalkableSelectionArea() {
        //test section tilemap
        BaseTile playerTile = GetPlayerTile();
        _selectableTiles = _changeHandler.FloodFillWalkable(playerTile, PLAYER_SPEED-1);

        List<BaseTile> listTiles = _selectableTiles.ToList();
        listTiles.Remove(_changeHandler.GetTileAtPositionByList(_playerObj.transform.position));
        _selectableTiles = listTiles.ToArray();
        
        _selectionTilemap.ClearAllTiles();
        foreach (BaseTile tile in _selectableTiles) {
            _selectionTilemap.SetTile(tile.cellPosition,_changeHandler.resources.selectionTile);
        }
    }

    public BaseTile GetPlayerTile() {
        return _changeHandler.GetTileAtPositionByList(_playerObj.transform.position);
    }

    private bool IsTileSelectableNow(BaseTile tile) {
        foreach (BaseTile selectable in _selectableTiles) {
            if (selectable.cellPosition == tile.cellPosition) {
                return true;
            }
        }

        return false;
    }

    public void Damage() {
        _health--;
        UpdateLifeBar();
        if (_health == 0) {
            _changeHandler.RestartGame();
        }
    }

    private void Heal() {
        if (_health < 5) {
            _health = 5;
            UpdateLifeBar();
        }
    }

    private void UpdateLifeBar() {
        for(int i = 0;i<5;i++)
        {
            _lifeBar.GetChild(i).gameObject.SetActive(i<_health);
        }
    }
    
    public void DoPlayerTileChecks() {

        BaseTile playerTile = _changeHandler.GetTileAtPositionByList(_playerObj.position);
        if (playerTile == null) {
            Damage();
            return;
        }
        
        if (playerTile.healthChange > 0 || playerTile.indexName == "town") {
            Heal();
        }
        
        if (playerTile.healthChange < 0) {
            Damage();
        }
        
        
        //transform into boat
        if (playerTile.indexName == "water")
        {
            _playerObj.transform.Find("astronautA").gameObject.SetActive(false);
            _playerObj.transform.Find("ship_light").gameObject.SetActive(true);
        }
        else
        {
            _playerObj.transform.Find("astronautA").gameObject.SetActive(true);
            _playerObj.transform.Find("ship_light").gameObject.SetActive(false);
        }
        
        //check for win
        if (playerTile.cellPosition == new Vector3Int(-238,3,0))
        {
            _rocket = FindObjectOfType<Rocket>();
            _rocket.LaunchRocket();
            //_playerObj.gameObject.SetActive(false);
            _playerObj.gameObject.transform.Find("astronautA").gameObject.SetActive(false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            _changeHandler.RestartGame();
            _playerObj.position = Vector3.zero;
        }
        
        if (Input.GetKeyDown(KeyCode.K)) {
            Damage();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            Heal();
        }
        
        if (Input.GetMouseButtonDown(0) && _selectableTiles.Length == 0) {
            _changeHandler.RestartGame();
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && _health < 1) {
            _changeHandler.RestartGame();
            return;
        }

        
        if (mousedOverTile != null) {
            if (IsTileSelectableNow(mousedOverTile)){
                tilePos = mousedOverTile.transform.position;
                //Debug.Log("Moused Over Tile: " + mousedOverTile.name);
                //Debug.Log("Position: " + tilePos);
                gridPos = _grid.GetComponent<TileChangeHandler>().GetTileGridCoordinate(tilePos);
                //Debug.Log("Grid Position: " + gridPos);
                if (Input.GetMouseButtonDown(0)) {
                    _playerObj.position = tilePos;
                    //Debug.Log(mousedOverTile.GetComponent<BaseTile>().indexName);
                    //tilePos.x = tilePos.x - 0.62303f;
                    //tilePos.z = tilePos.z + 2.224154f;
                    //tilePos.y = 3.881368f;
                    //Debug.Log("Camera POS: " + tilePos);
                    _cameraTrans.position = tilePos + camOffset;
                    _lifeBar.position = tilePos + lifeOffset;
                    _turnHandler.DoTurn();
                    UpdateWalkableSelectionArea();
                }
            }
        }
        mousedOverTile = null;
    }
}
