using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RingManager : MonoBehaviour {
    public Transform[] rings;
    [SerializeField] private float rotationSpeed = 30f;

    void Awake () {
        rings = GameObject.FindGameObjectsWithTag("Ring")
                          .Select(r => r.transform)
                          .ToArray();
    }

    void Update () {
        for (int i = 0; i < rings.Length; i++) {
            if (rings[i] != null) {
                rings[i].Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            }
        }
    }
}