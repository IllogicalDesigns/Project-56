using UnityEngine;

public class CrouchedVignette : MonoBehaviour
{
    [SerializeField] GameObject vingette;
    PlayerMovement player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.player;
    }

    // Update is called once per frame
    void Update()
    {
        vingette.SetActive(player.crouched);
    }
}
