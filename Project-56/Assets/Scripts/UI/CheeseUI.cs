using TMPro;
using UnityEngine;

public class CheeseUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    GameManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        manager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        text.text = manager.cheeseCollected.ToString() + "/" + manager.cheeseGoal.ToString();
    }
}
