using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    IntegerField max;
    IntegerField current;
    IntegerField regen;
    HealthBar healthBar;
    
    SerializedProperty maxProperty;
    SerializedProperty currentProperty;
    SerializedProperty regenProperty;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualElement firstLine = new VisualElement();
        firstLine.style.flexDirection = FlexDirection.Row;
        firstLine.Add(healthBar = new HealthBar());
        firstLine.Add(max = new IntegerField()
        { 
            tooltip = "Max possible of the health"
        });
        root.Add(firstLine);
        root.Add(current = new IntegerField("Current")
        {
            tooltip = "Current amount of life"
        });
        root.Add(regen = new IntegerField("Regen")
        {
            tooltip = "Amount of life regained at next regen step"
        });
        
        max.RegisterValueChangedCallback(UpdateValue(maxProperty, ClampPositif, max));
        current.RegisterValueChangedCallback(UpdateValue(currentProperty, ClampCurrent, current));
        regen.RegisterValueChangedCallback(UpdateValue(regenProperty, ClampPositif, regen));
        
        Bind();

        return root;
    }

    void OnEnable()
    {
        maxProperty = serializedObject.FindProperty("max");
        currentProperty = serializedObject.FindProperty("current");
        regenProperty = serializedObject.FindProperty("regen");
    }

    // Don't use ImGUI
    public sealed override void OnInspectorGUI() { }

    void Bind()
    {
        max.SetValueWithoutNotify(maxProperty.intValue);
        current.SetValueWithoutNotify(currentProperty.intValue);
        regen.SetValueWithoutNotify(regenProperty.intValue);
        healthBar.Update(maxProperty.intValue, currentProperty.intValue, regenProperty.intValue);
    }

    (int, bool) ClampPositif(int i)
        => i switch
        {
            < 0 => (0, true),
            _ => (i, false)
        };
            
    (int, bool) ClampCurrent(int i)
    {
        if (i > maxProperty.intValue)
            return (maxProperty.intValue, true);
        return i switch
        {
            < 0 => (0, true),
            _ => (i, false)
        };
    }

    EventCallback<ChangeEvent<int>> UpdateValue(SerializedProperty property, System.Func<int, (int, bool)> clamp, IntegerField field)
    {
        return (ChangeEvent<int> evt) =>
        {
            (int clampedValue, bool needRefresh) = clamp(evt.newValue);
            if (needRefresh)
                field.SetValueWithoutNotify(clampedValue);
            property.intValue = clampedValue;
            serializedObject.ApplyModifiedProperties();
            healthBar.Update(maxProperty.intValue, currentProperty.intValue, regenProperty.intValue);
        };
    }
}