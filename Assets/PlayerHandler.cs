using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private GameObject mousedOverTile; //hold ref to tile we are mousing over
    private Vector3 tilePos;
    private Vector3 gridPos;
    private Grid _grid;
    private Transform _playerObj;
   
    
    
    public void init(Grid grid, Transform playerObj)
    {
        _grid = grid;
        _playerObj = playerObj;
    }
    
    
    //set ref to tile we are mousing over
    public void SetMousedTile(GameObject inputTile)
    {
        mousedOverTile = inputTile;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        if (mousedOverTile != null)
        {
            tilePos = mousedOverTile.GetComponent<BaseTile>().GetPosition();
            Debug.Log("Moused Over Tile: " + mousedOverTile.name);
            Debug.Log("Position: " + tilePos);
            gridPos = _grid.GetComponent<TileChangeHandler>().GetTileGridCoordinate(tilePos);
            Debug.Log("Grid Position: " + gridPos);
            if(Input.GetMouseButtonDown(0))
            {
            _playerObj.position = tilePos;
            }
        }
        mousedOverTile = null;
    }
}
