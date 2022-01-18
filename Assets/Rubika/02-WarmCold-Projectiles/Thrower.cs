using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Thrower : MonoBehaviour
{
    public Rigidbody projectile;
    public float outputSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var newProj = Instantiate(projectile);
            newProj.transform.position = transform.position;
            newProj.transform.rotation = transform.rotation;
            newProj.velocity = newProj.transform.forward * outputSpeed;
        }
    }
}
