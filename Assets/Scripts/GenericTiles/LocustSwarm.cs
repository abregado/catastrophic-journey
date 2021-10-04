using UnityEngine;

public class LocustSwarm: BaseTile {
    public override void Activate() {
        base.Activate();
        
        _turnHandler.AddEvent(3, cellPosition, "desert",true);
        BaseTile randTile = _changeHandler.GetRandomNeighbourTileOfTypes(this, new []{"grass"});
        
        if (randTile != null) {
            _turnHandler.AddEvent(3, randTile.cellPosition, "locust-swarm");    
        }
    }
}
