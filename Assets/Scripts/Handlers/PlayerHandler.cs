using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerHandler : MonoBehaviour
{
    private GameObject mousedOverTile; //hold ref to tile we are mousing over
    private Vector3 tilePos;
    private Vector3 gridPos;
    private Grid _grid;
    private Transform _playerObj;
    private Transform _cameraTrans;
    private Vector3 camOffset;
    private TurnHandler _turnHandler;
    private Tilemap _selectionTilemap;
    
    public void Init(Grid grid, Transform playerObj, Transform cameraTrans, TurnHandler turnHandler,Tilemap selectionTilemap)
    {
        _grid = grid;
        _playerObj = playerObj;
        _cameraTrans = cameraTrans;
        _turnHandler = turnHandler;
        _selectionTilemap = selectionTilemap;
        camOffset = _cameraTrans.position - _playerObj.position;
    }
    
    //set ref to tile we are mousing over
    public void SetMousedTile(GameObject inputTile)
    {
        mousedOverTile = inputTile;
    }
    
    // Update is called once per frame
    void Update()
    { 
        if (mousedOverTile != null)
        {
            tilePos = mousedOverTile.GetComponent<BaseTile>().GetPosition();
            //Debug.Log("Moused Over Tile: " + mousedOverTile.name);
            //Debug.Log("Position: " + tilePos);
            gridPos = _grid.GetComponent<TileChangeHandler>().GetTileGridCoordinate(tilePos);
            //Debug.Log("Grid Position: " + gridPos);
            if(Input.GetMouseButtonDown(0))
            {
                _playerObj.position = tilePos;
                //Debug.Log(mousedOverTile.GetComponent<BaseTile>().indexName);
                //tilePos.x = tilePos.x - 0.62303f;
                //tilePos.z = tilePos.z + 2.224154f;
                //tilePos.y = 3.881368f;
                //Debug.Log("Camera POS: " + tilePos);
                _cameraTrans.position = tilePos + camOffset;
                _turnHandler.DoTurn();
            }
        }
        mousedOverTile = null;
    }
}
