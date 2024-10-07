using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HurtEffects : MonoBehaviour
{
    [SerializeField] AudioSource beatSource;
    [SerializeField] Image hurtOverlayImage;
    [SerializeField] Image OnImpact;
    [SerializeField] float pulseSpeedd = 0.5f;
    [SerializeField] float speed = 1f;
    public Health playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = GameManager.player.GetComponent<Health>();
        hurtOverlayImage.DOFade(0.1f, pulseSpeedd).SetLoops(-1, LoopType.Yoyo);
        OnImpact.gameObject.SetActive(false);
    }

    private void OnEnable() {
        playerHealth = GameManager.player.GetComponent<Health>();

        playerHealth.DamageTaken += OnDamaged;

    }

    private void OnDisable() {
        playerHealth = GameManager.player.GetComponent<Health>();

        playerHealth.DamageTaken -= OnDamaged;

    }

    private void OnDamaged(Damage damage) {
        OnImpact.gameObject.SetActive(true);

        OnImpact.DOFade(1f, 0.1f).OnComplete(() => {
            OnImpact.DOFade(0f, speed).OnComplete(() => {
                OnImpact.gameObject.SetActive(false);
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.health < (playerHealth.maxHealth / 4) + 1) {
            hurtOverlayImage.gameObject.SetActive(true);
            if(!beatSource.isPlaying) beatSource.Play();
            beatSource.pitch = 1.1f;
            beatSource.volume = 0.75f;
        }
        else if (playerHealth.health < (playerHealth.maxHealth / 2) + 1) {
            hurtOverlayImage.gameObject.SetActive(true);
            if (!beatSource.isPlaying) beatSource.Play();
            beatSource.pitch = 0.8f;
            beatSource.volume = 0.5f;
        } else {
            hurtOverlayImage.gameObject.SetActive(false);
            if (beatSource.isPlaying) beatSource.Stop();
        }  
    }
}
