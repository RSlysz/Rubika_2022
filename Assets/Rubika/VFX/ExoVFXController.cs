using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class ExoVFXController : MonoBehaviour
{
    private VisualEffect vfx;

    public string eventToCall = "Fire";

    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            vfx.SendEvent(eventToCall);
    }
}
