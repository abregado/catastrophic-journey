using UnityEngine;

public class LocustSwarm: BaseTile {
    public override void Activate() {
        CenterTile();
        _turnHandler.AddEvent(3, transform.position, "desert");
        Vector3 randPos = _changeHandler.GetNeighbourPositionOfType(transform.position, "grass");
        
        if (randPos.x == -1000f) {
            return;
        }

        _turnHandler.AddEvent(3, randPos, "locust-swarm");
        //Debug.Log("Desert added a spread event at " + randPos);
    }
}
