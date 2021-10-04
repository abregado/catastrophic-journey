using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EffectTile: BaseTile {
    private ParticleSystem _particles;

    public override void Init(TileChangeHandler changer, TurnHandler turner, PlayerHandler player) {
        base.Init(changer, turner, player);
        _particles = transform.GetComponentInChildren<ParticleSystem>();
    }

    public override void Activate() {
        base.Activate();
        _particles.Play();

        if (indexName == "crater-dirt") {
            BaseTile playerTile = _playerHandler.GetPlayerTile();
            if (playerTile != null && playerTile.cellPosition == cellPosition) {
                _playerHandler.Damage();
            }
        }
    }
}
