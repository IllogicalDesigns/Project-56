using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GetVisual : GAction {

    [SerializeField] float speed = 3f;
    [SerializeField] float stoppingDist = 0.5f;

    Transform stimTransform;

    public override IEnumerator Perform() {
        //var cat = GetComponent<Cat>();
        //var stim = cat.topStim;
        //stimTransform = stim.transform;

        gameObject.SendMessage("SetBehaviorState", Cat.CatBehavior.Chase);

        var player = GameManager.player.transform;

        gAgent.agent.speed = speed;

        yield return gAgent.Goto(player.position, stoppingDist);

        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        AddEffects(Cat.visualOnPlayer, null);
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }

    private void Update() {
        if (running && !gAgent.agentState.hasState(Cat.attackState)) {
            gAgent.Replan();
        }
    }
}

