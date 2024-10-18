using UnityEngine;

public class QuitButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_WEBGL 
        gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitTheGame() {
        Debug.Log("QuitTheGame was called, attempting to quit");
        Application.Quit();
    }
}
