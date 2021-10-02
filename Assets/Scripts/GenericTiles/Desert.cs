using UnityEngine;

public class Desert: BaseTile {
    public override void Activate() {
        CenterTile();
        Vector3 randPos = _changeHandler.GetNeighbourPositionOfType(transform.position, "grass");
        if (randPos == new Vector3(-1000,-1000,-1000)) {
            Debug.Log("no event added");
            return;
        }

        _turnHandler.AddEvent(3, randPos, "desert");
        Debug.Log("Desert added a spread event at " + randPos);
    }
}
