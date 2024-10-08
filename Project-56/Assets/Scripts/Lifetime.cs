using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float lifetime = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
