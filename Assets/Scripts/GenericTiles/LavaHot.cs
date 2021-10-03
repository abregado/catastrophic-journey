using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LavaHot: BaseTile {
    private string[] effectingTypes = {
        "grass",
        "desert",
        "locust-swarm"
    };
    public override void Activate() {

        _turnHandler.AddEvent(3, cellPosition, "lava-cold",true);
        BaseTile randTile = _changeHandler.GetRandomNeighbourTileOfTypes(this, effectingTypes);
        
        
        if (randTile != null && Random.Range(0,100)<50) {
            _turnHandler.AddEvent(1, randTile.cellPosition, "lava-hot");
            return;
        }
        
        randTile = _changeHandler.GetRandomNeighbourTileOfTypes(this, new [] {"lava-cold"});
        if (randTile != null && Random.Range(0,100)<15) {
            _turnHandler.AddEvent(1, randTile.cellPosition, "lava-hot");
        }
    }
}
