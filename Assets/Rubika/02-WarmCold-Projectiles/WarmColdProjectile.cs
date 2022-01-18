using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WarmColdProjectile : MonoBehaviour
{
    public LayerMask layerMask = (LayerMask)(-1);
    public Gradient gradient = new Gradient();
    public float intensity = 1000;
    [Range(0f,1f)]
    public float readValue = 0f;
    public Color colorValue = Color.black;

    Material material;
    HDAdditionalLightData light;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponentInChildren<Renderer>().material;
        light = GetComponentInChildren<HDAdditionalLightData>();

        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        VolumeManager.instance.Update(transform, layerMask);
        readValue = VolumeManager.instance.stack.GetComponent<WarmColdVolumeComponent>().heat.value;
        colorValue = gradient.Evaluate(readValue);

        material.SetColor("_EmissiveColor", colorValue * intensity * readValue );
        light.color = colorValue;
    }
}
