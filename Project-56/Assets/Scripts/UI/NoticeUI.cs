using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NoticeUI : MonoBehaviour {
    [SerializeField] Slider slider;
    [SerializeField] Slider visualSlider;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI visualtext;
    [SerializeField] Cat cat;
    [SerializeField] bool hide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //slider.maxValue = cat;
        slider.value = 0;
    }

    // Update is called once per frame
    void Update() {
        if(hide) slider.gameObject.SetActive(cat.topStim != null);

        text.text = (cat.topStim == null ? "null" : cat.topStim.name) + ":" + cat.currentAction.actionName;

        HandleTopStim();
        HandleVisualStim();
    }

    void HandleTopStim() {
        if (cat.topStim == null)
            return;

        slider.maxValue = cat.topStim.maxAwareness;
        slider.value = cat.topStim.awareness;
    }

    void HandleVisualStim() {
        if (cat.visualStim == null)
            return;

        visualSlider.maxValue = cat.visualStim.maxAwareness;
        visualSlider.value = cat.visualStim.awareness;

        if(visualSlider.value > cat.thresholdForPlayerVisualReaction) {
            visualtext.text = "threshold reached";
        } else if (visualSlider.value > cat.minThreshold) {
            visualtext.text = "min threshold";
        }
    }
}

