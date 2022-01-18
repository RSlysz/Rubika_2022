using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WarmColdVolumeComponent : VolumeComponent
{
    public FloatParameter heat = new FloatParameter(0);
}
