using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LavaHot: BaseTile {
    private string[] effectingTypes = {
        "grass",
        "desert",
        "locust-swarm",
        "lava-cold"
    };
    public override void Activate() {
        CenterTile();
        
        _turnHandler.AddEvent(5, transform.position, "lava-cold");
        Vector3 randPos = _changeHandler.GetNeighbourPositionOfTypes(transform.position, effectingTypes);
        
        if (randPos.x == -1000f) {
            Debug.Log("lava had no suitable neighbours");
            return;
        }

        if (_changeHandler.GetGenericTileAtPosition(randPos).indexName == "lava-cold") {
            //spawn an extra hot lava here
            _turnHandler.AddEvent(2, randPos, "lava-hot");    
        }
        else {
            if (Random.RandomRange(0, 100) < 80) {
                _turnHandler.AddEvent(2, randPos, "lava-hot");
            }
        }

    }
}
