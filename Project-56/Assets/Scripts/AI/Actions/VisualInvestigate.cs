using System.Collections;
using UnityEngine;
using DG.Tweening;

public class VisualInvestigate : GAction {
    [SerializeField] float waitAtInvestigate = 2f;
    [SerializeField] float lookAtSpeed = 3f;
    Transform stimTransform;

    public override IEnumerator Perform() {
        gAgent.agent.SetDestination(transform.position);
        gAgent.agent.isStopped = true;

        var cat = GetComponent<Cat>();
        var stim = cat.topStim;
        stimTransform = stim.transform;

        var vec = stim.transform.position;
        vec.y = transform.position.y;

        //// Calculate the direction to face
        //Vector3 direction = vec - transform.position;

        //// Calculate the angle to rotate
        //if (direction != Vector3.zero) {
        //    // Get the target rotation as an angle around the Y axis
        //    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        //    // Get the current Y angle
        //    float currentAngle = transform.eulerAngles.y;

        //    // Calculate the shortest angle to rotate
        //    float angleToRotate = Mathf.DeltaAngle(currentAngle, targetAngle);

        //    // Rotate incrementally using transform.Rotate
        //    transform.Rotate(0, angleToRotate * Time.deltaTime, 0);
        //    yield return new WaitForEndOfFrame();
        //}


        yield return transform.DOLookAt(vec, lookAtSpeed);

        yield return new WaitForSeconds(lookAtSpeed + waitAtInvestigate);

        //cat.topStim = null; //TODO? yes you can remove this.... but this is never null on cat....
        stimTransform = null;

        CompletedAction();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        cost = 0.1f;
        AddEffects(Cat.investigateGoal, null);
        AddPreconditions(Cat.visualOnPlayer);
    }

    public override void Interruppted() {
        stimTransform = null;
        base.Interruppted();
    }

    private void OnDrawGizmos() {
        if (stimTransform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(stimTransform.position, 0.5f);
    }
}

