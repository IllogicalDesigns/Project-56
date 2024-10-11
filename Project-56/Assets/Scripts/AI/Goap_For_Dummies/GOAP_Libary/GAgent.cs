using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class GAgent : MonoBehaviour {
    public List<GAction> actions = new List<GAction>();
    [SerializeField] public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    private GPlanner planner;
    public Queue<GAction> actionQueue;
    public GAction currentAction;
    public SubGoal currentGoal;

    private Coroutine currentActionRoutine;

    public WorldStates agentState = new WorldStates();

    [HideInInspector] public NavMeshAgent agent;

    [SerializeField] private bool logDebug = true;

    // Start is called before the first frame update
    private void Awake() {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    public virtual void Start() {
        GAction[] acts = this.GetComponents<GAction>();
        foreach (var a in acts) {
            actions.Add(a);
        }
    }

    public void CompleteAction() {
        if (currentAction) {
            // InvokeAnyEffectFunctions();
            StopCoroutine(currentAction.performCoroutine);
            currentAction.performCoroutine = null;
            currentAction.running = false;
            currentAction.PostPerform();
        }

        void InvokeAnyEffectFunctions() {
            foreach (var effect in currentAction.effects) {
                if (effect.Value is Action action)
                    action.Invoke();
            }
        }
    }

    public void Replan(bool force = false) {
        if (currentAction == null) {
            Debug.Log("Current action is null");
            return;
        }
        else {
            Debug.Log("Replanning during action " + currentAction.GetType());
        }

        if (!force && !currentAction.isInterruptible) {
            Debug.Log(currentAction.name + " is not interuptable, preventing interupt during replan");
            return;
        }

        actionQueue = null;

        currentAction.Interruppted();
        StopCoroutine(currentActionRoutine);
        currentAction.running = false;
        currentAction = null;
    }

    public bool AddGoal(string str, object value, int priority, bool _removable) {
        if (HasGoal(str)) return false;

        SubGoal newSub = new SubGoal(str, value, priority, _removable);
        goals.Add(newSub, priority);

        return true;
    }

    public bool HasGoal(string str) {
        foreach (var i in goals.Keys) {
            if (i.sgoals.ContainsKey(str))
                return true;
        }

        return false;
    }

    public void RemoveGoal(string str) {
        foreach (var i in goals.Keys) {
            if (i.sgoals.ContainsKey(str))
                i.sgoals.Remove(str);
        }

        return;
    }

    // Update is called once per frame
    private void LateUpdate() {
        //We have a current running action, let it run
        if (currentAction != null && currentActionRoutine != null && currentAction.running) return;

        //agent has no plan
        if (planner == null || actionQueue == null) {
            planner = new GPlanner();

            //sory by decending priority our goals using Linq
            var sortedGoals = from entry in goals orderby entry.Value ascending select entry;

            foreach (var sg in sortedGoals) {
                actionQueue = planner.Plan(actions, sg.Key.sgoals, agentState, logDebug); //todo cdg is the null where we pass in our local state?
                if (actionQueue != null) {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        //if we run out of things todo, remove the goal and setup to get new plan
        if (actionQueue != null && actionQueue.Count == 0) {
            if (currentGoal.isRemoveable) {
                goals.Remove(currentGoal);
            }
            planner = null;
        }

        //We have a queue and no action running, start the next action.
        if (actionQueue != null && actionQueue.Count > 0) {
            currentAction = actionQueue.Dequeue();

            //Attempt to run the preperform, if fails, start a new plan
            if (currentAction.PrePerform()) {
                currentActionRoutine = currentAction.StartAction();
            }
            else {
                actionQueue = null; //force a replan as an action failed
            }
        }
    }

    //TODO this references the Influence libary and should be handled in a differnt way
    //public IEnumerator influencedGoto(InfluenceLayer layer, InfluenceLayer baseLayer, Vector3 point, float stoppingDist = 2f, float resampleDist = 2f, float rotationSpeed = 300) {
    //    agent.isStopped = false;
    //    agent.stoppingDistance = stoppingDist;
    //    agent.angularSpeed = rotationSpeed;

    //    NavMesh.SamplePosition(point, out var hit, resampleDist, NavMesh.AllAreas);

    //    var path = GoapNavagation.FindPath(transform.position, hit.position, layer, baseLayer);

    //    for (int i = 0; i < path.Length-1; i++) {
    //        Debug.DrawLine(layer.GridToWorld(path[i].gridPos), layer.GridToWorld(path[i+1].gridPos), Color.cyan, 10f);
    //    }
        
    //    // DrawDebugPath();
        
    //    foreach (var node in path)
    //    {
    //        var nodePosition = layer.GridToWorld(node.gridPos);
            
    //        do {
    //            //TODO fix this to see if we are at the agent destination as that is different from the point passed in
    //            agent.isStopped = false;
    //            agent.SetDestination(nodePosition);
    //            yield return new WaitForSeconds(0.1f);
    //        } while (Vector3.Distance(new Vector3(point.x, transform.position.y, point.z), transform.position) > stoppingDist + 1f);
    //    }
        
    //    yield return new WaitForEndOfFrame();
    //}
    
    public IEnumerator Goto(Vector3 point, float stoppingDist = 2f, float resampleDist = 2f, float rotationSpeed = 300) {
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDist - 0.2f;
        //agent.angularSpeed = rotationSpeed;
        //TODO fix this to see if we are at the agent destination as that is different from the point passed in
        var navPoint = NavMesh.SamplePosition(point, out var hit, resampleDist, NavMesh.AllAreas);
        
        do {
            agent.SetDestination(hit.position);
            yield return new WaitForSeconds(0.1f);
            //Debug.Log("realStopDist:" + (agent.stoppingDistance + stoppingDist) + " currentDist:" + Vector3.Distance(agent.transform.position, agent.destination));
        } while (Vector3.Distance(agent.transform.position, agent.destination) > agent.stoppingDistance + stoppingDist);
    }
    
    public IEnumerator Goto(Transform point, float stoppingDist = 2f, float resampleDist = 2f) {
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDist - 0.2f;
        //TODO fix this to see if we are at the agent destination as that is different from the point passed in

        do {
            var navPoint = NavMesh.SamplePosition(point.position, out var hit, resampleDist, NavMesh.AllAreas);
            
            agent.SetDestination(hit.position);
            yield return new WaitForSeconds(0.1f);
        } while (point != null && Vector3.Distance(agent.transform.position, agent.destination) > agent.stoppingDistance + stoppingDist);
    }

    //TODO this references the Awareness libary and should be handled in a differnt way
    public virtual void OnSuspicious(TrackedTarget target) {
    }

    //TODO this references the Awareness libary and should be handled in a differnt way
    public virtual void OnLostSuspicion(TrackedTarget target) {
    }

    //TODO this references the Awareness libary and should be handled in a differnt way
    public virtual void OnDetected(TrackedTarget target) {
    }

    //TODO this references the Awareness libary and should be handled in a differnt way
    public virtual void OnFullyDetected(TrackedTarget target) {
    }

    //TODO this references the Awareness libary and should be handled in a differnt way
    public virtual void OnLostDetection(TrackedTarget target) {
    }

    //TODO this references the Awareness libary and should be handled in a differnt way
    public virtual void OnFullyLost(TrackedTarget target) {
    }
}

public class SubGoal {
    public Dictionary<string, object> sgoals;
    public int priority;
    public bool isRemoveable;

    public SubGoal(string str, object value, int priority, bool _removable) {
        sgoals = new Dictionary<string, object>();
        sgoals.Add(str, priority);
        isRemoveable = _removable;
    }
}
