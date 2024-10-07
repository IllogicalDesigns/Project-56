using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable() {
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
