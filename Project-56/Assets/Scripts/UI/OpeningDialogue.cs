using System.Collections;
using UnityEngine;

public class OpeningDialogue : MonoBehaviour
{
    [SerializeField] string[] text;
    [SerializeField] float wait = 1.5f;
    Dialogue dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        dialogue = FindAnyObjectByType<Dialogue>();

        yield return new WaitForSeconds(wait);
        foreach (var item in text) {
            dialogue.DisplayDialogue(item);
            yield return new WaitForSeconds(wait);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
