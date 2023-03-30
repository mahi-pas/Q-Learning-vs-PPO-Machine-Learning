/*
    Q Learning Agents Class
    https://github.com/mahi-pas/Q-Learning-Package-Unity
    
    How to use:
    - Make your script inherit from QAgent
    - Overload your own definitions for:
        - GetNextState()
        - GetReward()

    Look at example/ for examples

 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAgent : MonoBehaviour
{

    [Header("Training Settings")]
    [SerializeField] private int numEpisodes = 10000;
    [SerializeField] private int maxIterEpisode = 100;
    [SerializeField] public float explorationDecay = 0.001f;
    [SerializeField] public float explorationProbability = 1f;
    [SerializeField] public float minExplorationProbability = 0.01f;
    [SerializeField] private float alpha = 0.01f;
    [SerializeField] private float gamma = 0.99f;

    [Header("Brain Settings")]
    [SerializeField] private int numStates = 5;    
    [SerializeField] private int numActions = 4;  
    public float[,] QTable;
    [SerializeField] private float secondsBetweenTurns = 0.1f;
    private float time = 0f;

    [Header("Current Episode Settings")]
    [SerializeField] private int currentIteration = 0;
    [SerializeField] private int currentEpisode = 0; 
    [SerializeField] private int currentState = 0; 
    [SerializeField] private int currentAction = 0;
    [SerializeField] private float totalEpisodeReward = 0f;
    [SerializeField] public bool done = false;

    

    public virtual void Start(){
        InitializeQTable();
        currentIteration = 0;
        currentEpisode = 0;
        time = 0f;
    }

    public virtual void Update(){
        time += Time.deltaTime;
        if(time < secondsBetweenTurns){
            return;
        }
        else{
            time -= secondsBetweenTurns;
        }
        if (currentEpisode < numEpisodes){
            if(done || currentIteration >= maxIterEpisode){
                //Episode over
                OnEpisodeEnd();
            }
            else if(currentIteration == 0 && !done){
                //First round of new episode
                OnEpisodeBegin();
                Iterate();
            }
            else if(!done){
                //Normal round of episode
                Iterate();
            }
        }
    }

    public virtual void Iterate(){
        Vector2 oldPos = transform.localPosition;
        currentAction = ChooseAction(currentState);
        //Debug.Log("Action: "+currentAction);
        int nextState = GetNextState(currentState, currentAction);
        Vector2 newPos = transform.localPosition;
        float reward = GetReward(oldPos, newPos, nextState); 
        float maxQ = GetMaxQ(nextState);
        QTable[currentState, currentAction] = (1 - alpha) * QTable[currentState, currentAction] + alpha * (reward + gamma * maxQ);
        
        // Update the current state and action
        currentState = nextState;
        totalEpisodeReward += reward;

        currentIteration++;
    }

    //Overwrite this in child classes
    public virtual int GetNextState(int currentState, int currentAction){
        return 0;
    }

    //Overwrite this in child classes
    public virtual float GetReward(Vector2 oldPosition, Vector2 newPosition, int nextState){
        return 0f;
    }

    public virtual void OnEpisodeEnd(){
        //Debug.Log("OnEpisodeEnd()");
        PrintQTable();
        currentEpisode += 1;
        currentIteration = 0;
        explorationProbability = Mathf.Max(minExplorationProbability, Mathf.Exp(-explorationDecay * currentEpisode));
        done = false;
    }

    public virtual void OnEpisodeBegin(){
        //Debug.Log("OnEpisodeBegin()");
        currentState = 0;
        currentAction = GetRandomAction();
    }

    private float GetMaxQ(int state)
    {
        float maxQ = float.MinValue;
        for (int i = 0; i < numActions; i++)
        {
            float qValue = QTable[state, i];
            if (qValue > maxQ)
            {
                maxQ = qValue;
            }
        }
        return maxQ;
    }

    private int ChooseAction(int state)
    {
        // Choose a random action
        if (Random.value < explorationProbability)
        {
            return GetRandomAction();
        }
        // Otherwise, choose the action with the highest Q-value
        float maxQ = float.MinValue;
        int bestAction = 0;
        for (int i = 0; i < numActions; i++)
        {
            float qValue = QTable[state, i];
            if (qValue > maxQ)
            {
                maxQ = qValue;
                bestAction = i;
            }
        }
        return bestAction;
    }

    private int GetRandomAction(){
        return Random.Range(0, numActions);
    }

    private void InitializeQTable(){
        QTable = new float[numStates, numActions];
    }

    public void PrintQTable(){
        string output = "";
        for (int i = 0; i < numStates; i++)
        {
            output += "(";
            for (int j = 0; j<numActions; j++){
                if(j == numActions -1){
                    output += string.Format("{0}",QTable[i,j]);
                }
                else{
                    output += string.Format("{0},",QTable[i,j]);
                }
            }
            
            output += ")\n";
        }
        Debug.Log(output);
    }
}
