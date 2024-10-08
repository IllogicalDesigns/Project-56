using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] float displayTime = 3f;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= 0)
            timer -= Time.deltaTime;

        dialogueText.gameObject.SetActive(timer > 0f);
    }

    public void DisplayDialogue(string dialogueString) {
        dialogueText.text = dialogueString;
        timer = displayTime;
    }
}
