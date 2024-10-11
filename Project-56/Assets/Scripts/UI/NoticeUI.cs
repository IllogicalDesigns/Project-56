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

    bool show;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //slider.maxValue = cat;
        slider.value = 0;

        slider.gameObject.SetActive(false);
        visualSlider.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        visualtext.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            show = !show;
            slider.gameObject.SetActive(show);
            visualSlider.gameObject.SetActive(show);
            text.gameObject.SetActive(show);
            visualtext.gameObject.SetActive(show);
        }

        if(hide) slider.gameObject.SetActive(cat.topStim != null);

        if(cat.topStim != null && cat.currentAction != null) 
            text.text = cat.topStim.name + ":" + cat.currentAction.actionName;
        else if(cat.currentAction != null)
            text.text = "null" + ":" + cat.currentAction.actionName;

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

        if(visualSlider.value > cat.attackThresh) {
            visualtext.text = "threshold reached";
        } else if (visualSlider.value > cat.minThreshold) {
            visualtext.text = "min threshold";
        }
    }
}

