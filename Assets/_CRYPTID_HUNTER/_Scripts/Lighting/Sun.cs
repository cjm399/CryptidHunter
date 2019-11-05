using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    [SerializeField] float maxIntensity = 3f;
    [SerializeField] float minIntensity = 0f;
    [SerializeField] AnimationCurve sunIntensityCurve;
    Light sunLight;

    private Transform cachedTransform;

    private void Start()
    {
        sunLight = GetComponent<Light>();
        cachedTransform = transform;
    }

    private void Update()
    {
        sunLight.intensity = sunIntensityCurve.Evaluate(cachedTransform.position.y / 100)*maxIntensity;
    }
}
