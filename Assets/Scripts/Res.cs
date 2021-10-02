using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(fileName = "Res", menuName = "ScriptableObjects/Res", order = 1)]
public class Res : ScriptableObject
{
    public GameObject[] tilePrefabs;

    public GameObject eventMarkerPrefab;
}
