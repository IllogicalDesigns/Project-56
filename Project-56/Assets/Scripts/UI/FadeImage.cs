using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeImage : MonoBehaviour
{
    Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.color = Color.black;

        image.DOFade(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
