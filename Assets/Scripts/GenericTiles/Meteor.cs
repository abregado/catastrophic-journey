using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor: BaseTile {
    private string[] nonEffectingTypes = {
        "mountain",
    };

    private string[] secondRingNotEffecting = {
        "mountain",
        "hill",
        "crater-dirt",
        "volcano"
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
        base.Activate();
        _particles.Play();

        List<BaseTile> effected = new List<BaseTile>();
        effected.Add(this);
        
        BaseTile[] neighbours = _changeHandler.GetAllNeighbourTilesExcluding(this, nonEffectingTypes);
        foreach (BaseTile tile in neighbours) {
            _turnHandler.AddEvent(1, tile.cellPosition, "crater-dirt");
            effected.Add(tile);
        }
        
        foreach (BaseTile tile in neighbours) {
            BaseTile[] secondRing = _changeHandler.GetNeighbourTiles(tile);
            foreach (BaseTile second in secondRing) {
                if (effected.Contains(second) == false &&
                    _changeHandler.IsOfWantedType(second, secondRingNotEffecting) == false) {
                    _turnHandler.AddEvent(2, second.cellPosition, "crater-dirt");
                    effected.Add(second);
                }
            }
        }
        
    }
}
