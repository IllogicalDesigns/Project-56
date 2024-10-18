using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame() {
        FindAnyObjectByType<GameManager>().TogglePausedState();
    }
}

