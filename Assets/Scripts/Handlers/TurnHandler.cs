﻿using System;
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
        
    }

    public void StartGame() {
        _turnCount = 0;
        ClearEvents();
    }

    public void ClearEvents() {
        _turnEvents.Clear();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && _gameRunning) {
            DoTurn();    
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            _gameRunning = !_gameRunning;
        }
    }

    public void DoTurn() {
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
                _changeHandler.ChangeTileAtCell(te.cell, te.newTileIndex);
                _turnEvents.RemoveAt(i);
            }
        }
        
    }

    public void AddEvent(int wait, Vector3Int cell,string newTileIndex,bool overrideEvent = false) {
        if (overrideEvent == false && TileAlreadyHasSoonerEvent(cell, wait + _turnCount)) {
            Debug.Log("already has earlier event for change: " + newTileIndex);
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