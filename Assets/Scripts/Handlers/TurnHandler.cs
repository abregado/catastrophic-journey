using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnHandler: MonoBehaviour {
    private TileChangeHandler _changeHandler;
    private PlayerHandler _player;
    
    public int _turnCount;
    public bool _gameRunning = true;
    
    private List<TurnEvent> _turnEvents;
    private Dictionary<string, BaseTile> _queuedSounds;
    
    public void Init(TileChangeHandler changer, PlayerHandler player) {
        _changeHandler = changer;
        _player = player;
        _turnEvents = new List<TurnEvent>();
        _queuedSounds = new Dictionary<string, BaseTile>();
    }
    
    public void StartGame() {
        _turnCount = 0;
        ClearEvents();
    }

    public void ClearEvents() {
        _turnEvents.Clear();
    }

    private void Update() {
        if (Mathf.Floor(Time.time * 10f) % 2 == 0) {
            var keys = _queuedSounds.Keys;
            var list = _queuedSounds.ToArray();
            if (list.Length > 0) {
                if (_queuedSounds[list[0].Key] != null) {
                    list[0].Value.PlaySound(list[0].Key);
                }
                _queuedSounds.Remove(list[0].Key);
            }
        }
    }

    public void DoTurn() {
        _turnCount++;

        if (_turnCount % 15 == 0) {
            _player.Damage();
        }
        
        ProcessEventStack();

        GenerateNewDisasters();
    }

    private void ProcessEventStack() {
        _changeHandler.StopAllTileSounds();
        _queuedSounds.Clear();
        
        if (_turnEvents.Count == 0) {
            return;
        }

        for (int i = _turnEvents.Count-1; i >= 0 ; i--) {
            if (_turnEvents[i] != null && _turnEvents[i].turn <= _turnCount) {
                TurnEvent te = _turnEvents[i];
                _changeHandler.ChangeTileAtCell(te.cell, te.newTileIndex);
                _turnEvents.RemoveAt(i);
            }
        }
        
        _player.DoPlayerTileChecks();
    }

    private void GenerateNewDisasters() {
        BaseTile playerTile = _player.GetPlayerTile();
        int xAbs = playerTile.cellPosition.x;
        
        //spawn a chasm behind the player every turn
        int spawnX = xAbs + Random.Range(2, 6);
        Vector3Int disasterCell = _changeHandler.GetRandomStripPos(spawnX);
        AddEvent(3,disasterCell,"quake",true);
        
        //spawn in front of the player every second turn.
        if (_turnCount % 2 == 0) {
            spawnX = Math.Max(xAbs,_turnCount*-2) - Random.Range(6, 15);
            disasterCell = _changeHandler.GetRandomStripPos(spawnX);
            string randomDisaster = _changeHandler.GetRandomDisaster();
            AddEvent(3,disasterCell,randomDisaster,true);
        }
        
        //spawn disasters progressivly along the map
        if (_turnCount % 2 == 1) {
            spawnX =  Random.Range(xAbs, (_turnCount*-2 + 5));
            disasterCell = _changeHandler.GetRandomStripPos(spawnX);
            string randomDisaster = _changeHandler.GetRandomDisaster();
            AddEvent(3,disasterCell,randomDisaster,true);
        }
        
    }

    public void AddEvent(int wait, Vector3Int cell,string newTileIndex,bool overrideEvent = false) {
        if (overrideEvent == false && TileAlreadyHasSoonerEvent(cell, wait + _turnCount)) {
            //Debug.Log("already has earlier event for change: " + newTileIndex);
            return;
        }
        _turnEvents.Add(new TurnEvent(_turnCount + wait,cell,newTileIndex));
        List<TurnEvent> sortedList = _turnEvents.OrderBy(o => o.turn).ToList();
        _turnEvents = sortedList;
    }

    private bool TileAlreadyHasSoonerEvent(Vector3 position, int turn) {
        foreach (TurnEvent te in _turnEvents) {
            if (position == te.cell && te.turn >= turn) {
                return true;
            }
        }

        return false;
    }

    public void QueueSound(string soundType, BaseTile tile) {
        if (_queuedSounds.ContainsKey(soundType) == false) {
            _queuedSounds.Add(soundType,tile);
        }
    }
    
}

public class TurnEvent {
    public int turn;
    public Vector3Int cell;
    public string newTileIndex;

    public TurnEvent(int turn, Vector3Int cell, string newTileIndex) {
        this.turn = turn;
        this.cell = cell;
        this.newTileIndex = newTileIndex;
    }

    public override bool Equals(object obj) {
        TurnEvent te = obj as TurnEvent;
        return te.turn == turn && te.cell == cell;
    }
    
}