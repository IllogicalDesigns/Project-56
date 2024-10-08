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

    public float coolDown = 3f;
    public float timer;

    public override bool PrePerform() {
        if (timer > 0)
            return false;

        return base.PrePerform();
    }

    public override IEnumerator Perform() {
        //var cat = GetComponent<Cat>();
        //var stim = cat.topStim;
        //stimTransform = stim.transform;

        gameObject.SendMessage("SetBehaviorState", Cat.CatBehavior.Chase);

        attackSource.Play();

        gAgent.agent.speed = speed;

        hurtBox.SetActive(true);
        timer = coolDown;
        gAgent.agentState.RemoveState(Cat.AttackOffCoolDown);
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
        AddPreconditions(Cat.attackState, true);
        //AddPreconditions(Cat.AttackOffCoolDown, true);
        AddEffects(Cat.attackGoal, null);
    }

    public override void Interruppted() {
        hurtBox.SetActive(false);
        stimTransform = null;
        attackSource.Stop();
        base.Interruppted();
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }

    private void Update() {
        if(timer > 0) {
            timer -= Time.deltaTime;
        } else {
            gAgent.agentState.SetState(Cat.AttackOffCoolDown, true);
        }

        if (running && !gAgent.agentState.hasState(Cat.visualOnPlayer)) {
            gAgent.Replan();
        }
    }
}

