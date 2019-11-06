using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShaderValues : MonoBehaviour
{
    [SerializeField] private Light flashLight;

    void Update()
    {
        Shader.SetGlobalFloat("_FlashLightRange", flashLight.range);
    }
}
