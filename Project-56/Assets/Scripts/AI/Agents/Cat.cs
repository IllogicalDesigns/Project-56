using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

public class Cat : GAgent
{
    public const string patrolGoal = "patrolGoal";
    public const string investigateGoal = "investigateGoal";
    public const string visialInvestigateGoal = "visialInvestigateGoal";
    public const string attackGoal = "attackGoal";
    public const string chaseGoal = "chaseGoal";
    public const string stalkGoal = "stalkGoal";
    public const string restGoal = "restGoal";

    public const string visualOnPlayer = "visualOnPlayer";
    public const string attackState = "attackState";
    public const string pointOfInterest = "pointOfInterest";
    public const string isStimulated = "isStimulated";
    public const string AttackOffCoolDown = "AttackOffCoolDown";

    public const string investigateState = "investigateState";

    GameObject player;
    IVisionSensor visionSensor;
    IHearingSensor hearingSensor;

    public Stimulus topStim;
    public Stimulus visualStim;

    public float attackThresh = 2f;
    public float minThreshold = 0.5f;
    public float maxThresh = 4f;

    public bool canHearThings = true;

    [Space]
    public float investigateTime = 10f;
    public float investigateTimer;
    public float attackTime = 10f;
    public float attackTimer;

    public enum CatBehavior {
        Patrol,
        Investigate,
        Attack,
        Chase
    }

    public CatBehavior behaviourState;

    public float attackStateTimer;
    public float upMulti = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.player.gameObject;
        AddGoal(patrolGoal, null, 6, false);

        visionSensor = GetComponent<IVisionSensor>();
        hearingSensor = GetComponent<IHearingSensor>();
    }

    public bool IsPointInFront(Vector3 point) {
        // Direction from cat to the point
        Vector3 directionToPoint = (point - transform.position).normalized;

        // The forward direction of the cat
        Vector3 forward = transform.forward;

        // Dot product between the forward direction and the direction to the point
        float dotProduct = Vector3.Dot(forward, directionToPoint);

        // If the dot product is greater than 0, the point is in front
        return dotProduct > 0;
    }

    public void SetBehaviorState(CatBehavior newState) {
        behaviourState = newState;
    }

    private void Update() {
        if(investigateTimer > 0) { investigateTimer -= Time.deltaTime; }
        if(attackTimer > 0) {  attackTimer -= Time.deltaTime; }

        HandleSeeingThePlayer();
    }

    private void HandleSeeingThePlayer() {
        var multi = visionSensor.CanWeSeeTarget(visualStim.gameObject);

        if (multi <= 0f) {
            agentState.RemoveState(visualOnPlayer);
            return;
        } else {
            agentState.SetState(visualOnPlayer, true);
            visualStim.AdjustStim(multi * Time.deltaTime);
        }

        if (visualStim.awareness > minThreshold && AddGoal(investigateGoal, player, 5, true)) {
            investigateTimer = investigateTime;
            topStim = visualStim;
            Replan();
        }

        if (visualStim.awareness > attackThresh && AddGoal(attackGoal, player, 3, true)) {
            attackTimer = attackTime;
            AddGoal(chaseGoal, player, 4, true);
            topStim = visualStim;
            Replan();
        }

    }


    public void SoundStimuli(Stimulus stimulus, bool ignoreHearing = false, float boost = 0f) {
        if (!hearingSensor.CanWeHear(stimulus.gameObject)) return;
        if(AddGoal(investigateGoal, stimulus, 5, true)) {
            investigateTimer = investigateTime;
            topStim = stimulus;
            Replan();
        }
    }

}

