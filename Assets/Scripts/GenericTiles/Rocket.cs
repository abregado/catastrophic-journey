using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rocket : BaseTile
{
    private DOTweenAnimation _animation;
    private ParticleSystem[] _particlesList;
    
    public override void Init(TileChangeHandler changer, TurnHandler turner, PlayerHandler player) {
        base.Init(changer, turner, player);
        //_animation = transform.GetComponentInChildren<DOTweenAnimation>();
        _particlesList = transform.GetComponentsInChildren<ParticleSystem>();
        //Debug.Log(_animation);
        
    }
    
    
    public void LaunchRocket()
    {
        foreach (ParticleSystem particles in _particlesList)
        {
            particles.Play();
        }
        //_animation.Play();
        DOTween.Play("launch");
    }
    
    
    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


}
