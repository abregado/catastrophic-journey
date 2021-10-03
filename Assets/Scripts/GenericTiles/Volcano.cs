using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Volcano: BaseTile {
    private string[] effectingTypes = {
        "grass",
        "desert",
        "locust-swarm",
        "lava-cold",
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

        if (Random.Range(0, 100) < 11) {
            //volcano stops erupting
            return;
        }
        
        _particles.Play();
        _turnHandler.AddEvent(Random.Range(3, 7), cellPosition, "volcano");
        BaseTile[] neighbours = _changeHandler.GetAllNeighbourTilesOfTypes(this, effectingTypes);
        
        if (neighbours.Length == 0) {
            return;
        }

        for (int i = 0; i < Math.Min(3,neighbours.Length); i++) {
            BaseTile randTile = neighbours[Random.Range(0, neighbours.Length)];
            if (randTile != null) {
                _turnHandler.AddEvent(1, randTile.cellPosition, "lava-hot");    
            }
        }
    }
}
