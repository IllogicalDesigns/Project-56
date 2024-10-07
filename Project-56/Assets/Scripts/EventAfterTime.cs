using UnityEngine;
using UnityEngine.Events;

public class EventAfterTime : MonoBehaviour
{
    [SerializeField] float time = 5f;
    [SerializeField] UnityEvent onTimerEnd;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (time < 0) {
            onTimerEnd.Invoke();
            this.enabled = false;
        }
    }
}
