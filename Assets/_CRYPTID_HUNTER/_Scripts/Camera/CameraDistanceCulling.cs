using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraDistanceCulling : MonoBehaviour
{
    Camera[] cameras = new Camera[2];
    private float[] distances = new float[32];


    [SerializeField] private float playerLayer = 2f;
    [SerializeField] private float cryptidLayer = 200f;
    [SerializeField] private float terrainLayer = 200f;
    [SerializeField] private float propsLayer = 100f;
    [SerializeField] private float grassLayer = 100f;
    [SerializeField] private float triggerLayer = 1f;
    [SerializeField] private float backgroundLayer = 0f; //0 is infinite


    void Start()
    {
        cameras = GetComponentsInChildren<Camera>();
        UpdateDistances();
    }
    [Button("UpdateDistances")]
    private void UpdateDistances()
    {
        if (cameras.Length == 0)
            return;

        distances[8] = playerLayer;
        distances[9] = cryptidLayer;
        distances[10] = terrainLayer;
        distances[11] = propsLayer;
        distances[12] = grassLayer;
        distances[13] = triggerLayer;
        distances[14] = backgroundLayer;

        for(int i = 0; i < cameras.Length; ++i)
        {
            cameras[i].layerCullDistances = distances;
        }
    }
}
