using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Quake: BaseTile {
    public override void Activate() {
        base.Activate();
        
        BaseTile neighbour = _changeHandler.GetRandomNeighbourTileExcludingTypes(this, new string[]{"quake"});
        _turnHandler.AddEvent(1, this.cellPosition, "destroy", true);
        
        if (neighbour != null && Random.Range(0,100)<90)
        {
            //neighbour.transform.position = neighbour.transform.position + new Vector3(0f,0f,-100f);
            //tile.isWalkable = false;

            _turnHandler.AddEvent(1, neighbour.cellPosition, "quake", true);
        }
        
        
        
        //foreach (BaseTile tile in neighbours)
        //{
        //    tile.transform.position = tile.transform.position + new Vector3(0f,0f,-100f);
        //   tile.isWalkable = false;
        //    if (Random.Range(0,100) >= 50)
        //    {
                //_changeHandler.ChangeTileAtCell(tile.cellPosition, "quake", false);
        //        _turnHandler.AddEvent(1, tile.cellPosition, "quake", true);
        //     }
        

    }
}
