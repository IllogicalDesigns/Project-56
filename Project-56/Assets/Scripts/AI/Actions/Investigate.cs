using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Investigate : GAction
{
    [SerializeField] float waitAtInvestigate = 2f;
    [SerializeField] float speed = 3f;
    [SerializeField] float lookAtSpeed = 3f;
    Transform stimTransform;

    public override IEnumerator Perform() {
        gAgent.agent.SetDestination(transform.position);
        gAgent.agent.isStopped = true;
        yield return new WaitForSeconds(0.5f);

        var cat = GetComponent<Cat>();
        var stim = cat.topStim;
        stimTransform = stim.transform;

        gAgent.agent.speed = speed;

        yield return gAgent.Goto(stim.transform.position);

        gAgent.agent.SetDestination(transform.position);
        gAgent.agent.isStopped = true;

        var vec = stim.transform.position;
        vec.y = transform.position.y;
        yield return transform.DOLookAt(vec, lookAtSpeed);

        yield return new WaitForSeconds(lookAtSpeed + waitAtInvestigate);

        cat.topStim = null; //TODO?
        stimTransform = null;

        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddEffects(Cat.investigateGoal, null);
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }
}
