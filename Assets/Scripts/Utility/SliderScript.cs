using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] Slider slider;

    bool isInitialized = false;

    private void Start()
    {
        if(slider == null)
        {
            if(TryGetComponent(out Slider outSlider))
            {
                slider = outSlider;
            }
        }

        isInitialized = slider != null;
    }

    public void SetSliderValuePercent(float health, float maxHealth) => SetSliderValue(health / maxHealth);

    public void SetSliderValue(float setTo)
    {
        if (isInitialized == false) return;

        if (setTo > slider.maxValue) slider.value = slider.maxValue;
        else if (setTo < 0) slider.value = 0;
        else slider.value = setTo;
    }
    public void SetSliderMaxValue(float setTo)
    {
        if (isInitialized == false || setTo <= 0) return;

        slider.maxValue = setTo;

        if (slider.value > slider.maxValue) SetSliderValue(slider.maxValue);
    }
}
