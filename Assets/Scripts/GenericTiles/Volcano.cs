using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Volcano: BaseTile {
    private string[] effectingTypes = {
        "grass",
        "water",
        "forest",
        "desert",
        "locust-swarm",
    };

    private DOTweenAnimation _animation;
    private ParticleSystem _particles;

    public override void Init(TileChangeHandler changer, TurnHandler turner, PlayerHandler player) {
        base.Init(changer, turner, player);
        _animation = transform.GetComponentInChildren<DOTweenAnimation>();
        _particles = transform.GetComponentInChildren<ParticleSystem>();
        //Debug.Log(_animation);
        
    }

    public override void Activate() {

        if (Random.Range(0, 100) < 5) {
            //volcano stops erupting
            _particles.Stop();
            return;
        }
        
        _particles.Play();
        _turnHandler.AddEvent(Random.Range(1, 3), cellPosition, "volcano",true);
        
        //check for cold lava and set one to hot
        BaseTile neighbourColdLava = _changeHandler.GetRandomNeighbourTileOfTypes(this, new[]{"lava-cold"});
        
        if (neighbourColdLava != null) {
            _turnHandler.AddEvent(1, neighbourColdLava.cellPosition, "lava-hot");
        }

        BaseTile[] neighbours = _changeHandler.GetAllNeighbourTilesOfTypes(this, effectingTypes);
        for (int i = 0; i < Math.Min(3,neighbours.Length); i++) {
            BaseTile randTile = neighbours[Random.Range(0, neighbours.Length)];
            if (randTile != null) {
                _turnHandler.AddEvent(1, randTile.cellPosition, "lava-cold");    
            }
        }
    }
}
