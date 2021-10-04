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
        "volcano",
        "town"
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
        
        
        BaseTile playerTile = _playerHandler.GetPlayerTile();
        if (playerTile != null && playerTile == this) {
            _playerHandler.Damage();
        }
        
        List<BaseTile> effected = new List<BaseTile>();
        effected.Add(this);
        
        BaseTile[] neighbours = _changeHandler.GetAllNeighbourTilesExcluding(this, nonEffectingTypes);
        foreach (BaseTile tile in neighbours) {
            _turnHandler.AddEvent(1, tile.cellPosition, "crater-dirt");
            effected.Add(tile);
            if (tile == playerTile) {
                _playerHandler.Damage();
            }
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
