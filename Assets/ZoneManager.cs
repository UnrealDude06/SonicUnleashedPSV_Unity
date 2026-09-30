using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {
    public ZoneChunkLoader[] allChunks;
    public float checkInterval = 2f;

    void Start() {
        InvokeRepeating("CheckChunks", 0f, checkInterval);
    }

    void CheckChunks() {
        foreach (var chunk in allChunks) {
            chunk.CheckDistance();
        }
    }
}