using TMPro;
using UnityEngine;

public class PebbleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    Throw thrower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thrower = FindAnyObjectByType<Throw>();
    }

    // Update is called once per frame
    void Update()
    {
        if(thrower.infinateAmmo)
            text.text = "NA";
        else
            text.text = thrower.ammo.ToString();
    }
}
