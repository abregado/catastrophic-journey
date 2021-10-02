using UnityEngine;

public interface IMapTile {
    void Init(TileChangeHandler changeHandler, TurnHandler turnHandler);

    void OnTileCreated();
    string GetTileIndex();
    Vector3 GetPosition();
}
