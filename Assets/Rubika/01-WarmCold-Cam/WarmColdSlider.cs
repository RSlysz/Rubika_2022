using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Slider))]
public class WarmColdSlider : MonoBehaviour
{
    public Transform playerSampler;
    public LayerMask layerMask = (LayerMask)(-1);

    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        VolumeManager.instance.Update(playerSampler, layerMask );
        slider.value = VolumeManager.instance.stack.GetComponent<WarmColdVolumeComponent>().heat.value;

        // slider.value = Mathf.Sin(Time.time) * 0.5f + 0.5f;
    }
}
