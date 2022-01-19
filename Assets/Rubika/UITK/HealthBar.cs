using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : VisualElement
{
    //for it to appear in UI Builder
    public new class UxmlFactory : UxmlFactory<HealthBar, UxmlTraits> { }

    VisualElement health;
    VisualElement regenHint;

    public HealthBar()
    {
        style.flexGrow = 1;
        style.flexDirection = FlexDirection.Row;
        style.backgroundColor = Color.black;
        style.paddingBottom = 1;
        style.paddingTop = 1;
        style.paddingLeft = 1;
        style.paddingRight = 1;

        health = new VisualElement();
        health.style.backgroundColor = Color.red;
        health.style.flexShrink = 0; //unshrinkable part
        Add(health);

        regenHint = new VisualElement();
        regenHint.style.backgroundColor = Color.green;
        regenHint.style.flexShrink = 1;
        regenHint.style.height = Length.Percent(50);
        regenHint.style.alignSelf = Align.FlexEnd; // at bottom
        Add(regenHint);
    }

    public void Update(int max, int current, int regen)
    {
        health.style.width = Length.Percent(100 * current / (float)max);
        regenHint.style.width = Length.Percent(100 * regen / (float)max);
    }
}