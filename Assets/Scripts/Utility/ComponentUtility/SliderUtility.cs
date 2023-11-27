using UnityEngine;
using UnityEngine.UI;

public class SliderUtility : ComponentUtility<Slider>
{
    public void SetSliderValuePercent(float health, float maxHealth) => SetSliderValue(health / maxHealth);

    public void SetSliderValue(float setTo)
    {
        if (isInitialized == false) return;

        if (setTo > component.maxValue) component.value = component.maxValue;
        else if (setTo < 0) component.value = 0;
        else component.value = setTo;
    }
    public void SetSliderMaxValue(float setTo)
    {
        if (isInitialized == false || setTo <= 0) return;

        component.maxValue = setTo;

        if (component.value > component.maxValue) SetSliderValue(component.maxValue);
    }
}