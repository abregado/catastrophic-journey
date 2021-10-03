using TMPro;
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
