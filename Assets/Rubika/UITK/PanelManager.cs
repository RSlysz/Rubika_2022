using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class PanelManager : MonoBehaviour
{
    public UIDocument uiDocument;
    HealthBar healthBar;
    
    public Health trackedHealth;

    private void OnEnable()
    {
        VisualElement root = uiDocument.rootVisualElement;
        healthBar = root.Q<HealthBar>();
    }

    private void Update()
    {
        if (healthBar != null && trackedHealth != null)
            healthBar.Update(trackedHealth.max, trackedHealth.current, trackedHealth.regen);
    }
}

