using UnityEngine;

public class Desert: BaseTile, IMapTile {
    public void OnTileCreated() {
        CenterTile();
        Vector3 randPos = _changeHandler.GetNeighbourPositionOfType(transform.position, "grass");
        if (randPos == Vector3.negativeInfinity) {
            return;
        }

        _turnHandler.AddEvent(3, randPos, "desert");
    }
}
