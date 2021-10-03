using UnityEngine;

public class BaseTile: MonoBehaviour {
    protected TileChangeHandler _changeHandler;
    protected TurnHandler _turnHandler;
    protected PlayerHandler _playerHandler;

    public string indexName = "default";
    public bool isWalkable;
    public int generatedWeight = 0;
    public int disasterWeight = 0;
    
    public Vector3Int cellPosition;
    
    public virtual void Init(TileChangeHandler changer, TurnHandler turner, PlayerHandler player) {
        _changeHandler = changer;
        _turnHandler = turner;
        _playerHandler = player;
        
        OnTileCreated();
    }

    public virtual void Activate() {
        
    }
    
    protected void CenterTile() {
        Vector3Int cellPos = _changeHandler.GetCellAtPosition(transform.position);
        Vector3 newPos = _changeHandler.GetTileCentreAtCell(cellPos);
        transform.position = newPos;
        cellPosition = cellPos;
    }

    public virtual void OnTileCreated() {
        CenterTile();
    }

    public string GetTileIndex() {
        return indexName;
    }
    
    public Vector3 GetPosition() {
        return transform.position;
    }
    //prints name of object mouse is over
    void OnMouseOver()
    {
        _playerHandler.SetMousedTile(this);
    }
}