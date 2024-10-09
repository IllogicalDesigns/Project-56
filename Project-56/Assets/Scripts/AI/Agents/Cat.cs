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
    public const string stalkGoal = "stalkGoal";
    public const string restGoal = "restGoal";

    public const string visualOnPlayer = "visualOnPlayer";
    public const string attackState = "attackState";
    public const string pointOfInterest = "pointOfInterest";
    public const string isStimulated = "isStimulated";
    public const string AttackOffCoolDown = "AttackOffCoolDown";

    GameObject player;
    IVisionSensor visionSensor;
    IHearingSensor hearingSensor;  //TODO are we not hearing?

    List<Stimulus> soundStimuli = new List<Stimulus>();
    public Stimulus topStim;
    public Stimulus visualStim;

    public float thresholdForPlayerVisualReaction = 1f;
    public float minThreshold = 0.5f;
    public float maxThresh = 4f;

    bool canSeePlayer;

    float stalkPlayerTimer = 10f;
    float stalkTimer;

    public bool canHearThings = true;

    public enum CatBehavior {
        Patrol,
        Investigate,
        Chase,
        //Hunt,
        //WantRest
    }

    public CatBehavior behaviourState;

    public float attackStateTimer;
    public float upMulti = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.player.gameObject;
        //AddGoal(stalkGoal, null, 4, false);
        AddGoal(patrolGoal, null, 6, false);
        //AddGoal(restGoal, null, 7, false);
        AddGoal(investigateGoal, null, 2, false);
        AddGoal(attackGoal, null, 1, false);

        visionSensor = GetComponent<IVisionSensor>();
        hearingSensor = GetComponent<IHearingSensor>();
    }

    public void SetBehaviorState(CatBehavior newState) {
        behaviourState = newState;
    }

    Transform FindClosestCheeseObject(Transform nearTransform) {
        FindAnyObjectByType<Dialogue>().DisplayDialogue("Recalculating...");
        // Get all objects with the "Cheese" script attached
        var allCheeses = FindObjectsOfType<Cheese>();

        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = nearTransform.position;

        var bestCheese = allCheeses[0];

        foreach (Cheese cheese in allCheeses) {
            // Calculate the distance between this object and the Cheese object
            float distanceToCheese = Vector3.Distance(cheese.transform.position, currentPosition);

            // If this distance is smaller than the previously stored one, update it
            if (distanceToCheese < closestDistance) {
                closestDistance = distanceToCheese;
                bestCheese = cheese;
            }
        }


        return bestCheese.transform;
    }

    private void Update() {
        HandleSeeingThePlayer();

        if (topStim != null && topStim.awareness <= 0) {
            agentState.RemoveState(isStimulated);
            topStim = null;
        }

        if(topStim == null)
            agentState.RemoveState(isStimulated);
    }

    private void HandleSeeingThePlayer() {
        var multi = visionSensor.CanWeSeeTarget(player);

        if (multi > 0f) {
            agentState.SetState(visualOnPlayer, visualStim.awareness);
            visualStim.AdjustStim(multi * Time.deltaTime);
            if (attackStateTimer < maxThresh) attackStateTimer += upMulti * Time.deltaTime;
        }
        else {
            agentState.RemoveState(visualOnPlayer);
            if (attackStateTimer > 0f) { attackStateTimer -= Time.deltaTime; }
        }

        if (attackStateTimer > thresholdForPlayerVisualReaction) {
            agentState.SetState(attackState, true);
        }
        else if (attackStateTimer > minThreshold && topStim != visualStim) {
            topStim = visualStim;
            agentState.SetState(isStimulated, true);
            Replan();
        }
        else if (attackStateTimer <= 0.1f) {
            agentState.RemoveState(attackState);
        }
    }

    public void TouchedTheCat() {
        //sound stim, ignoredd
        //player is immediately hunted
        //If the player is on the back eject them from the back with anim
    }

    public void SoundStimuli(Stimulus stimulus, bool ignoreHearing = false, float boost = 0f) {
        if (!canHearThings)
            return;
        
        if (topStim != null && topStim == stimulus) { return; }  //restimulated, ignore
        if (!hearingSensor.CanWeHear(stimulus.gameObject)) { return; }

        if (topStim != null) {  // is the new stim closer?
            var topDist = Vector3.Distance(topStim.transform.position, transform.position);
            var newDist = Vector3.Distance(topStim.transform.position, transform.position);

            if (topDist < newDist) { return; }
        }

        if (topStim != null && stimulus.awareness + boost < topStim.awareness) { return; }

        agentState.SetState(isStimulated, true);
        topStim = stimulus;

        if(!agentState.hasState(attackState))
            Replan();
    }
}

