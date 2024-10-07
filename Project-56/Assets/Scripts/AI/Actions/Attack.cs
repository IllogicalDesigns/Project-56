using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Attack : GAction {

    [SerializeField] float speed = 3f;
    [SerializeField] float stoppingDist = 0.5f;

    Transform stimTransform;

    [SerializeField] GameObject hurtBox;
    [SerializeField] float activeTime = 0.5f;

    Transform player;

    [SerializeField] AudioSource attackSource;
    [SerializeField] AudioClip audioClip;

    public override IEnumerator Perform() {
        //var cat = GetComponent<Cat>();
        //var stim = cat.topStim;
        //stimTransform = stim.transform;

        attackSource.Play();

        gAgent.agent.speed = speed;

        hurtBox.SetActive(true);
        yield return gAgent.Goto(player, stoppingDist);

        
        yield return new WaitForSeconds(activeTime);
        hurtBox.SetActive(false);

        AudioSource.PlayClipAtPoint(audioClip, transform.position);
        //BlacklistAction();

        attackSource.Stop();

        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        player = GameManager.player.transform;
        AddPreconditions(Cat.visualOnPlayer, true);
        AddEffects(Cat.attackGoal, null);
    }

    public override void Interruppted() {
        stimTransform = null;
        attackSource.Stop();
        base.Interruppted();
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }
}

