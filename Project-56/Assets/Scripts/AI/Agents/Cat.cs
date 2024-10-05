using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cat : GAgent
{
    public const string patrolGoal = "patrolGoal";
    public const string investigateGoal = "investigateGoal";
    public const string attackGoal = "attackGoal";

    public const string visualOnPlayer = "visualOnPlayer";

    GameObject player;
    IVisionSensor visionSensor;
    IHearingSensor hearingSensor;

    List<Stimulus> soundStimuli = new List<Stimulus>();
    public Stimulus topStim;
    public Stimulus visualStim;

    [SerializeField] float visualAwareness;

    public enum CatBehavior {
        Patrol,
        Investigate,
        Chase,
        Hunt
    }

    public CatBehavior behaviourState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameManager.player.gameObject;
        AddGoal(patrolGoal, null, 5, false);
        visionSensor = GetComponent<IVisionSensor>();
        hearingSensor = GetComponent<IHearingSensor>();
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    var multi = visionSensor.CanWeSeeTarget(player);

    //    if(multi > 0f) {
    //        if (!soundStimuli.Contains(visualStim))
    //            soundStimuli.Add(visualStim);

    //        visualStim.awareness += multi * (Time.deltaTime+Time.deltaTime);
    //    }

    //    if(multi > 0f) {
    //        agentState.SetState(visualOnPlayer, true);
    //    } else {
    //        agentState.RemoveState(visualOnPlayer);
    //    }

    //    if (visualStim.awareness > 2f) {
    //        if(AddGoal(attackGoal, player, 3, true)) {
    //            Replan();
    //        }

    //        UpdateBestStim();
    //    } else if (visualStim.awareness > 1f) {
    //        UpdateBestStim();

    //        foreach (var i in goals.Keys) {
    //            if (i.sgoals.ContainsKey(investigateGoal)) {
    //                if (!i.sgoals.ContainsValue(player)) {
    //                    RemoveGoal(investigateGoal);
    //                    break;
    //                }
    //            }
    //        }

    //        if (AddGoal(investigateGoal, player, 4, true)) {
    //            topStim = visualStim;
    //            Debug.Log("Investigate the player visually ");
    //            Replan();
    //        }
    //    }

    //    for (int i = 0; i < soundStimuli.Count; i++) {
    //        if (soundStimuli[i].awareness <= 0)
    //            soundStimuli.RemoveAt(i);

    //        if (soundStimuli[i] == visualStim)
    //            RemoveGoal(attackGoal);
    //    }
    //}

    //void UpdateBestStim() {
    //    var bestStim = GetMaxPriorityMostAwareStimulus();

    //    if(bestStim != topStim) {
    //        topStim = bestStim;
    //        Debug.Log("We should investigate " + topStim.gameObject.name);

    //        RemoveGoal(investigateGoal);
    //        AddGoal(investigateGoal, bestStim, 4, true);

    //        Replan();
    //    }
    //}

    //public Stimulus GetMaxPriorityMostAwareStimulus() {
    //    var maxPriorityMostAwareStimulus = soundStimuli.OrderByDescending(s => s.priority)
    //                                                 .ThenByDescending(s => s.awareness)
    //                                                 .First();

    //    return maxPriorityMostAwareStimulus;
    //}

    //public void SoundStimuli(Stimulus stimulus) {
    //    var heard = hearingSensor.CanWeHear(stimulus.gameObject);
    //    if (!heard) return;

    //    if (!soundStimuli.Contains(stimulus)) {
    //        //Debug.Log("New sound heard " + stimulus.gameObject.name);
    //        soundStimuli.Add(stimulus);
    //        UpdateBestStim();
    //    }

    //    UpdateBestStim();

    //}

    private void Update() {
        var multi = visionSensor.CanWeSeeTarget(player);

        if (multi > 0f && !soundStimuli.Contains(visualStim))
            soundStimuli.Add(visualStim);

        if (multi > 0f)
            visualStim.awareness += multi * (Time.deltaTime + Time.deltaTime);

        if(visualStim.awareness > 1.5f) {
            if(AddGoal(attackGoal, player, 1, false)) {
                topStim = visualStim;
                Replan();
            }
        }
    }

    public void SoundStimuli(Stimulus stimulus) {
        if(HasGoal(attackGoal)) { return; }

        if (AddGoal(investigateGoal, stimulus, 2, true)) {
            topStim = stimulus;
            Replan();
        }
    }
}

