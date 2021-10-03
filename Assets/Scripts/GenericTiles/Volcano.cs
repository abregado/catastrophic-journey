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
        "lava-hot",
    };

    private DOTweenAnimation _animation;
    private ParticleSystem _particles;

    public override void Init(TileChangeHandler changer, TurnHandler turner) {
        base.Init(changer, turner);
        _animation = transform.GetComponentInChildren<DOTweenAnimation>();
        _particles = transform.GetComponentInChildren<ParticleSystem>();
        //Debug.Log(_animation);
        _particles.Play();
    }

    public override void Activate() {
        CenterTile();

        if (Random.Range(0, 100) < 11) {
            //volcano stops erupting
            return;
        }

        _turnHandler.AddEvent(Random.Range(3, 7), transform.position, "volcano");
        Vector3 randPos = _changeHandler.GetNeighbourPositionOfTypes(transform.position, effectingTypes);

        if (randPos.x == -1000f) {
            //Debug.Log("volcano had no suitable neighbours");
            return;
        }

        if (_changeHandler.GetGenericTileAtPosition(randPos).indexName == "lava-hot") {
            if (Random.Range(0, 100) < 80) {
                _turnHandler.AddEvent(1, transform.position, "volcano");
            }
        } else if (_changeHandler.GetGenericTileAtPosition(randPos).indexName == "lava-cold") {
            if (Random.Range(0, 100) < 80) {
                _turnHandler.AddEvent(1, randPos, "lava-hot");
            }
        } else {
            _turnHandler.AddEvent(1, randPos, "lava-hot");
        }

    }
}
