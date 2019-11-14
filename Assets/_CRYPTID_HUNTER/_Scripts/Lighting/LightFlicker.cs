using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private int smoothing = 10;


    private Queue<float> smoothQueue;
    private float lastSum = 0;
    private Light myLight;

    void Start()
    {
        myLight = GetComponent<Light>();
        smoothQueue = new Queue<float>(smoothing);
    }

    void Update()
    {
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        myLight.intensity = lastSum / (float)smoothQueue.Count;
    }


    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

}
