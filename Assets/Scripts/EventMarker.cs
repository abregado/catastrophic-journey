using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EventMarker : MonoBehaviour {
    private TextMeshPro _text;

    void Start() {
        _text = transform.GetComponentInChildren<TextMeshPro>();
    }

    public void SetText(string text) {
        _text.text = text;
    }
}
