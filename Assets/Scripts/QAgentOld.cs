using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAgentOld : MonoBehaviour
{

    [Header("Training Settings")]
    [SerializeField] private int numEpisodes = 10000;
    [SerializeField] private int maxIterEpisode = 100;
    [SerializeField] private float explorationDecay = 0.001f;
    [SerializeField] private float explorationProbability = 1f;
    [SerializeField] private float minExplorationProbability = 0.01f;
    [SerializeField] private float gamma = 0.99f;
    [SerializeField] private float learningRate = 0.01f;

    [Header("Brain Settings")]
    private Dictionary<string, float> QTable;
    [SerializeField] private int numObservations = 5;
    [SerializeField] private int numActions = 4;

    [Header("Current Episode Settings")]
    [SerializeField] private bool done = false;
    [SerializeField] private float totalEpisodeReward = 0f;
    [SerializeField] private int currentIteration = 0;
    [SerializeField] private int currentEpisode = 0;
    [SerializeField] private string currentState;
    [SerializeField] private List<float> rewardsPerEpisode;


    public void Start() {
        QTable = new Dictionary<string, float>();
        rewardsPerEpisode = new List<float>();
        done = false;

        OnEpisodeBegin();
    }

    public virtual void OnEpisodeBegin(){
        ResetEnvironment();
        done = false;
        totalEpisodeReward = 0f;
        currentIteration = 0;
        currentState = ActionToState(GetCurrentPosition(), GetCurrentPosition());
    }

    public virtual void ResetEnvironment(){

    }

    private void Update() {
        if (currentEpisode < numEpisodes){
            Iteration();
        }
    }

    public void Iteration(){

        Vector2 action;
        //see if random action will be taken
        if(Random.Range(0f,1f) < explorationProbability){
            action = SelectRandomMove();
        }
        else{
            action = SelectOptimalMove();
        }

        float reward;
        string nextState;

        TakeStep(action, out nextState, out reward, out done);

        //calculate Q value
        float temp;
        if (!QTable.TryGetValue(currentState, out temp)) QTable[currentState] = 0f;
        if (!QTable.TryGetValue(nextState, out temp)) QTable[nextState] = 0f;
        float actionOptimalMove;
        SelectOptimalMove(action,out actionOptimalMove);
        QTable[currentState] = (1-learningRate) * QTable[currentState] + (learningRate * gamma * (actionOptimalMove));

        totalEpisodeReward += reward;

        currentIteration++;

        if (done || currentIteration >= maxIterEpisode){
            EndEpisode();
        }

        currentState = nextState;
        

    }

    public void EndEpisode(){
        rewardsPerEpisode.Add(totalEpisodeReward);
        explorationProbability = Mathf.Min(minExplorationProbability, Mathf.Exp(-explorationDecay * currentEpisode));

        currentEpisode++;
        OnEpisodeBegin();
    }

    public void TakeStep(Vector2 action, out string nextState, out float reward, out bool done){
        Vector2 globalPosition = transform.TransformPoint(action);
        float radius = 0.3f;
        Collider2D collision = Physics2D.OverlapCircle(globalPosition, radius);

        nextState = ActionToState(GetCurrentPosition(), action);

        if(collision == null){
            done = false;
            reward = 0f;
            transform.localPosition = action;
        }
        else if( collision.gameObject.tag == "Wall"){
            reward = -1f;
            done = true;
            Debug.Log("HitWall");
        }
        else if( collision.gameObject.tag == "Goal"){
            reward = 1f;
            done = true;
            transform.localPosition = action;
        }
        else{
            done = false;
            reward = 0f;
            transform.localPosition = action;
        }
    }

    public string ActionToState(Vector2 currentPos, Vector2 nextPos){
        return "("+string.Join(", ", currentPos)+")->("+string.Join(", ", nextPos)+")";
    }

    public Vector2 SelectOptimalMove(){
        float highest = 0f;
        Vector2 highestLoc = new Vector2();
        Vector2 currentPos = GetCurrentPosition();
        foreach (Vector2 cur in GetPossibleMoves())
        {
            float currentVal = 0f;
            QTable.TryGetValue(ActionToState(currentPos, cur), out currentVal);

            if(currentVal >= highest){
                highestLoc = cur;
                highest = currentVal;
            }
        }

        return highestLoc;
    }

    public Vector2 SelectOptimalMove(Vector2 position, out float highest){
        highest = 0f;
        Vector2 highestLoc = new Vector2();
        foreach (Vector2 cur in GetPossibleMoves(position))
        {
            float currentVal = 0f;
            QTable.TryGetValue(ActionToState(position, cur), out currentVal);

            if(currentVal >= highest){
                highestLoc = cur;
                highest = currentVal;
            }
        }
        return highestLoc;
    }

    public Vector2 SelectRandomMove(){
        int randIndex = Random.Range(0,4);
        return GetPossibleMoves()[randIndex];
    }

    public List<Vector2> GetPossibleMoves(){
        Vector2 position = GetCurrentPosition();
        List<Vector2> result = new List<Vector2>();
        result.Add(position + new Vector2(0,1));
        result.Add(position + new Vector2(0,-1));
        result.Add(position + new Vector2(1,0));
        result.Add(position + new Vector2(-1,0));
        return result;
    }

    public List<Vector2> GetPossibleMoves(Vector2 position){
        List<Vector2> result = new List<Vector2>();
        result.Add(position + new Vector2(0,1));
        result.Add(position + new Vector2(0,-1));
        result.Add(position + new Vector2(1,0));
        result.Add(position + new Vector2(-1,0));
        return result;
    }

    public Vector2 GetCurrentPosition(){
        return new Vector2(Mathf.Floor(transform.localPosition.x), Mathf.Floor(transform.localPosition.y));
    }

}

