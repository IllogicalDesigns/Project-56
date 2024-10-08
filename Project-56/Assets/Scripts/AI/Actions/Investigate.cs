using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Investigate : GAction
{
    [SerializeField] float waitAtInvestigate = 2f;
    [SerializeField] float speed = 3f;
    [SerializeField] float fastSpeed = 8f;
    [SerializeField] float lookAtSpeed = 3f;
    Transform stimTransform;
    [SerializeField] AudioClip meow;

    public override IEnumerator Perform() {
        if(meow) AudioSource.PlayClipAtPoint(meow, transform.position);

        //gAgent.agent.SetDestination(transform.position);
        //gAgent.agent.isStopped = true;

        //yield return new WaitForSeconds(0.5f);

        var cat = GetComponent<Cat>();
        var stim = cat.topStim;
        stimTransform = stim.transform;

        gAgent.agent.speed = speed;

        yield return gAgent.Goto(stim.transform);

        //gAgent.agent.SetDestination(transform.position);
        //gAgent.agent.isStopped = true;

        //var vec = stim.transform.position;
        //vec.y = transform.position.y;
        //yield return transform.DOLookAt(vec, lookAtSpeed);

        //yield return new WaitForSeconds(lookAtSpeed + waitAtInvestigate);

        //cat.topStim = null; //We no longer have a top stim, we investigated it away
        //stimTransform = null;

        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddPreconditions(Cat.isStimulated, null);
        AddEffects(Cat.investigateGoal, null);
    }

    public override void Interruppted() {
        stimTransform = null;
        base.Interruppted();
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }
}
