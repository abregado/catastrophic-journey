using UnityEngine;

public class BaseTile: MonoBehaviour {
    protected TileChangeHandler _changeHandler;
    protected TurnHandler _turnHandler;
    
    public string indexName = "default";
    
    public virtual void Init(TileChangeHandler changer, TurnHandler turner) {
        _changeHandler = changer;
        _turnHandler = turner;
        
        OnTileCreated();
    }

    public virtual void Activate() {
        
    }
    
    protected void CenterTile() {
        Vector3 newPos = _changeHandler.GetTileCentreAtPosition(transform.position);
        transform.position = newPos;
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
}