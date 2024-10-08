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

    public enum CatBehavior {
        Patrol,
        Investigate,
        Chase,
        Hunt,
        WantRest
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
        var multi = visionSensor.CanWeSeeTarget(player);

        if (multi > 0f) {
            agentState.SetState(visualOnPlayer, true);
            if (attackStateTimer < maxThresh) attackStateTimer += upMulti * Time.deltaTime;
        } else {
            agentState.RemoveState(visualOnPlayer);
            if(attackStateTimer > 0f) { attackStateTimer -= Time.deltaTime; }
        }

        if(attackStateTimer > thresholdForPlayerVisualReaction) {
            agentState.SetState(attackState, true);
        } else if(attackStateTimer <= 0.1f) {
            agentState.RemoveState(attackState);
        }

        if(topStim != null && topStim.awareness <= 0) {
            agentState.RemoveState(isStimulated);
            topStim = null;
        }

        if(topStim == null)
            agentState.RemoveState(isStimulated);

        //HandleSeeingThePlayer();

        //if(stalkTimer > 0) {
        //    stalkTimer -= Time.deltaTime;
        //} else {
        //    var cheese = FindClosestCheeseObject(player.transform);
        //    if (cheese) {
        //        var cheesePos = cheese.position;
        //        agentState.SetState(pointOfInterest, cheesePos);
        //        if(!(currentAction is Investigate) && !(currentAction is Attack) && !(currentAction is Stalk))
        //            Replan();
        //    }
        //    stalkTimer = stalkPlayerTimer;
        //}
    }

    private void HandleSeeingThePlayer() {
        //var multi = visionSensor.CanWeSeeTarget(player);
        //canSeePlayer = multi > 0f;

        //if (multi > 0f) {
        //    agentState.SetState(pointOfInterest, topStim.transform.position);

        //    if (visualStim.awareness > thresholdForPlayerVisualReaction) {
        //        if (AddGoal(attackGoal, player, 1, false)) {
        //            //AddGoal(stalkGoal, null, 4, false);
        //            behaviourState = CatBehavior.Chase;
        //            Replan();
        //        }
        //    } else {
        //        SoundStimuli(visualStim, true, 1f);
        //    }
        //}

        ////if (multi > 0f && !soundStimuli.Contains(visualStim))
        ////    soundStimuli.Add(visualStim);

        //if (multi > 0f) {
        //    agentState.SetState(visualOnPlayer, true);
        //    visualStim.AdjustStim(multi * Time.deltaTime);
        //}
        //else {
        //    agentState.RemoveState(visualOnPlayer);
        //}

        //if (visualStim.awareness > thresholdForPlayerVisualReaction) {
        //    if (AddGoal(attackGoal, player, 1, false)) {
        //        topStim = visualStim;
        //        agentState.SetState(pointOfInterest, topStim.transform.position);
        //        AddGoal(stalkGoal, null, 4, true);
        //        RemoveGoal(investigateGoal);
        //        Replan();
        //    }
        //} else if(visualStim.awareness > minThreshold) {
        //    if(topStim != visualStim)
        //        RemoveGoal(investigateGoal);

        //    if (AddGoal(investigateGoal, player, 3, true)) {
        //        topStim = visualStim;
        //        Replan();
        //    }
        //}
    }

    public void TouchedTheCat() {
        //sound stim, ignoredd
        //player is immediately hunted
        //If the player is on the back eject them from the back with anim
    }

    public void SoundStimuli(Stimulus stimulus, bool ignoreHearing = false, float boost = 0f) {
        //if (currentAction is Attack) { return; }
        if(topStim != null && topStim == stimulus) { return; }  //restimulated, ignore
        if (!hearingSensor.CanWeHear(stimulus.gameObject)) { return; }

        //if (topStim != null) Debug.Log("topStim="+topStim.name + ":" + topStim.awareness + "  " + stimulus.name + ":" + stimulus.awareness);
        //else Debug.Log("topStim=" + "Null" + "  " + stimulus.name + ":" + stimulus.awareness);

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

        //behaviourState = CatBehavior.Investigate;
        //AddGoal(investigateGoal, stimulus, 3, true);
        //topStim = stimulus;
        //Replan();


        //////if higher prio comes in, investigate that
        ////if(topStim != null && stimulus.priority > topStim.priority) {
        ////    RemoveGoal(investigateGoal);
        ////}

        //    ////if the current stim is really unaware, investigate the new stim
        //    //if (topStim != null && topStim.awareness <= 0f) {
        //    //    RemoveGoal(investigateGoal);
        //    //}

        //    //if (AddGoal(investigateGoal, stimulus, 3, true)) {
        //    //    topStim = stimulus;
        //    //    AddGoal(stalkGoal, null, 4, true);
        //    //    agentState.SetState(pointOfInterest, topStim.transform.position);
        //    //    Replan();
        //    //}
    }
}

