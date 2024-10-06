using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Attack : GAction {

    [SerializeField] float speed = 3f;
    [SerializeField] float stoppingDist = 0.5f;

    Transform stimTransform;

    [SerializeField] GameObject hurtBox;
    [SerializeField] float activeTime = 0.5f;

    public override IEnumerator Perform() {
        var cat = GetComponent<Cat>();
        var stim = cat.topStim;
        stimTransform = stim.transform;

        gAgent.agent.speed = speed;

        yield return gAgent.Goto(stim.transform, stoppingDist);

        hurtBox.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        hurtBox.SetActive(false);

        BlacklistAction();

        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        AddEffects(Cat.attackGoal, null);
    }

    public override void Interruppted() {
        stimTransform = null;
        base.Interruppted();
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }
}

