using UnityEngine;
using UnityEngine.UI;

public class HPSlider : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Slider slider;
    [SerializeField] Slider underSlider;
    [SerializeField] float dropSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.maxValue = health.maxHealth;
        underSlider.maxValue = health.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.gameObject.SetActive(health.health < health.maxHealth);
        underSlider.gameObject.SetActive(health.health < health.maxHealth);
        slider.value = health.health;

        if(underSlider.value > slider.value)
            underSlider.value -= dropSpeed * Time.deltaTime;

        if(underSlider.value < slider.value) underSlider.value = slider.value;
    }
}
