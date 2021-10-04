using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LavaHot: BaseTile {
    private ParticleSystem _particles;
    private string[] effectingTypes = {
        "grass",
        "water",
        "forest",
        "desert",
        "locust-swarm",
        "crater-dirt"
    };
    public override void Init(TileChangeHandler changer, TurnHandler turner, PlayerHandler player) {
        base.Init(changer, turner, player);      
    }

    public override void Activate() {
        base.Activate();

        _turnHandler.AddEvent(3, cellPosition, "lava-cold",true);
        BaseTile randTile = _changeHandler.GetRandomNeighbourTileOfTypes(this, effectingTypes);
        
        if (randTile != null) {
            _turnHandler.AddEvent(1, randTile.cellPosition, "lava-cold");
            return;
        }
        
        randTile = _changeHandler.GetRandomNeighbourTileOfTypes(this, new [] {"lava-cold"});
        if (randTile != null && Random.Range(0,100)<15) {
            _turnHandler.AddEvent(1, randTile.cellPosition, "lava-hot");
        }
    }
}
