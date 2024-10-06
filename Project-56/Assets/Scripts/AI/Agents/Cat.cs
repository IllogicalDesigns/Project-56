using System.Collections.Generic;
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
    public const string pointOfInterest = "pointOfInterest";

    GameObject player;
    IVisionSensor visionSensor;
    IHearingSensor hearingSensor;  //TODO are we not hearing?

    List<Stimulus> soundStimuli = new List<Stimulus>();
    public Stimulus topStim;
    public Stimulus visualStim;

    public float thresholdForPlayerVisualReaction = 1.5f;
    public float minThreshold = 0.5f;

    public enum CatBehavior {
        Patrol,
        Investigate,
        Chase,
        Hunt,
        WantRest
    }

    public CatBehavior behaviourState;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.player.gameObject;
        AddGoal(patrolGoal, null, 6, true);
        AddGoal(restGoal, null, 7, false);
        visionSensor = GetComponent<IVisionSensor>();
        hearingSensor = GetComponent<IHearingSensor>();
    }

    private void Update() {
        HandleSeeingThePlayer();
    }

    private void HandleSeeingThePlayer() {
        var multi = visionSensor.CanWeSeeTarget(player);

        if (multi > 0f && !soundStimuli.Contains(visualStim))
            soundStimuli.Add(visualStim);

        if (multi > 0f) {
            agentState.SetState(visualOnPlayer, true);
            visualStim.AdjustStim(multi * Time.deltaTime);
        } else {
            agentState.RemoveState(visualOnPlayer);
        }

        if (visualStim.awareness > thresholdForPlayerVisualReaction) {
            if (AddGoal(attackGoal, player, 1, false)) {
                topStim = visualStim;
                agentState.SetState(pointOfInterest, topStim.transform.position);
                AddGoal(stalkGoal, null, 4, true);
                RemoveGoal(investigateGoal);
                Replan();
            }
        } else if(visualStim.awareness > minThreshold) {
            if(topStim != visualStim)
                RemoveGoal(investigateGoal);

            if (AddGoal(investigateGoal, player, 3, true)) {
                topStim = visualStim;
                Replan();
            }
        }
    }

    public void TouchedTheCat() {
        //sound stim, ignoredd
        //player is immediately hunted
        //If the player is on the back eject them from the back with anim
    }

    public void SoundStimuli(Stimulus stimulus) {
        if(HasGoal(attackGoal)) { return; }
        if (!hearingSensor.CanWeHear(stimulus.gameObject)) { return; }

        //if higher prio comes in, investigate that
        if(topStim != null && stimulus.priority > topStim.priority) {
            RemoveGoal(investigateGoal);
        }

        //if the current stim is really unaware, investigate the new stim
        if (topStim != null && topStim.awareness <= 0f) {
            RemoveGoal(investigateGoal);
        }

        if (AddGoal(investigateGoal, stimulus, 3, true)) {
            topStim = stimulus;
            AddGoal(stalkGoal, null, 4, true);
            agentState.SetState(pointOfInterest, topStim.transform.position);
            Replan();
        }
    }
}

