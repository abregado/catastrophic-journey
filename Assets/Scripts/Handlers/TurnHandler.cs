using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TurnHandler: MonoBehaviour {
    private TileChangeHandler _changeHandler;
    
    public int _turnCount;
    public bool _gameRunning = true;

    private List<TurnEvent> _turnEvents;
    
    public void Init(TileChangeHandler changer) {
        _changeHandler = changer;
        
        _turnEvents = new List<TurnEvent>();
        
        StartGame();
    }

    private void StartGame() {
        _turnCount = 0;
        _turnEvents.Clear();
        
        BaseTile[] tilesToActivate = FindObjectsOfType<BaseTile>();
        foreach (BaseTile tile in tilesToActivate) {
            tile.Activate();
        }
        Debug.Log("Activated " + tilesToActivate.Length + " tiles");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && _gameRunning) {
            DoTurn();    
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            _gameRunning = !_gameRunning;
        }
    }

    private void DoTurn() {
        _turnCount++;
        ProcessEventStack();
    }

    private void ProcessEventStack() {
        if (_turnEvents.Count == 0) {
            return;
        }

        for (int i = _turnEvents.Count-1; i >= 0 ; i--) {
            if (_turnEvents[i] != null && _turnEvents[i].turn <= _turnCount) {
                TurnEvent te = _turnEvents[i];
                Debug.Log(te.position);
                Debug.Log(te.newTileIndex);
                _changeHandler.ChangeTileAtPosition(te.position, te.newTileIndex);
                _turnEvents.RemoveAt(i);
            }
        }
        
    }

    public void AddEvent(int wait, Vector3 position,string newTileIndex) {
        _turnEvents.Add(new TurnEvent(_turnCount+wait,position,newTileIndex));
        List<TurnEvent> sortedList = _turnEvents.OrderBy(o => o.turn).ToList();
        _turnEvents = sortedList;
    }
    
}

public class TurnEvent {
    public int turn;
    public Vector3 position;
    public string newTileIndex;

    public TurnEvent(int turn, Vector3 position, string newTileIndex) {
        this.turn = turn;
        this.position = position;
        this.newTileIndex = newTileIndex;
    }

    public override bool Equals(object obj) {
        TurnEvent te = obj as TurnEvent;
        return te.turn == turn && te.position == position;
    }
    
}