using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMoveToTarget : QAgent
{
    /*
    [10 States, 4 Actions]
    Actions:
        0: Move Up
        1: Move Right
        2: Move Down
        3: Move Left
    States:
        0: Target is north
        1: Target is north-east
        2: Target is east
        3: Target is south-east
        4: Target is south
        5: Target is south-west
        6: Target is west
        7: Target is north-west 
        8: Hit Target
        9: Hit Wall             ----Currently hit wall is not implemented, agent will not recognize walls
     */

     /* Best Tables so far
    (7.945521,6.826207,6.994746,44.03877)
    (0,0,0,0)
    (0,0,0,0)
    (-0.06179959,0.846605,0.1397007,-0.06930903)
    (6.115326,9.443285,14.43366,6.892287)
    (69.86707,68.88033,86.95436,71.18029)
    (91.74245,92.64734,92.14013,95.86502)
    (88.68631,73.29393,74.51134,76.27456)
    (0,0,0,0)
     
     
      */

    [Header("MoveToTarget")]
    public Transform target;
    public float moveSpeed = 3f;
    public float directionDeadZone = 0.1f;
    public bool loadBestTable = false;

    [Header("Visualization")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private SpriteRenderer floor;

    public override void Start(){
        base.Start();

        if (loadBestTable) LoadBestTable();
    }

    
    public override void OnEpisodeBegin(){
        base.OnEpisodeBegin();
        transform.localPosition = new Vector3(Random.Range(0f,8f), Random.Range(3.5f,-3.5f), 0);
        target.transform.localPosition = new Vector3(Random.Range(-1f,-8f), Random.Range(3.5f,-3.5f), 0);
    }

    public override int GetNextState(int currentState, int currentAction){
        ExecuteAction(currentAction);
        return GetState(transform.localPosition, target.localPosition);
    }

    public override float GetReward(Vector2 oldPosition, Vector2 newPosition, int nextState){
        if (nextState == 8){
            floor.color = winColor;
            Debug.Log("Done!");
            done = true;
            return 100f;
        }
        else if (nextState == 9){
            floor.color = loseColor;
            done = true;
            return -100f;
        }
        float targetDistanceDelta = Vector2.Distance(newPosition, target.localPosition) - Vector2.Distance(oldPosition, target.localPosition);
        if (targetDistanceDelta > 0){
            //further from target, punish
            return -1f;
        }
        else if(targetDistanceDelta < 0){
            //closer to target, reward
            return 1f;
        }
        return 0f;
    }

    public int GetState(Vector2 position, Vector2 target){
        Vector2 diff = target - position;
        if (Mathf.Abs(diff.x) < directionDeadZone) diff.x = 0f;
        if (Mathf.Abs(diff.y) < directionDeadZone) diff.y = 0f;

        if(diff.x == 0 && diff.y > 0 ){ //North
            return 0;
        }
        else if(diff.x > 0 && diff.y > 0 ){ //North-east
            return 1;
        }
        else if(diff.x > 0 && diff.y == 0 ){ //East
            return 2;
        }
        else if(diff.x > 0 && diff.y < 0 ){ //South-east
            return 3;
        }
        else if(diff.x == 0 && diff.y < 0 ){ //South
            return 4;
        }
        else if(diff.x < 0 && diff.y < 0 ){ //South-west
            return 5;
        }
        else if(diff.x < 0 && diff.y == 0 ){ //West
            return 6;
        }
        else if(diff.x < 0 && diff.y > 0 ){ //North-west
            return 7;
        }
        else if(diff.x == 0 && diff.y == 0){ //goal
            return 8;
        }
        else{
            Debug.LogError(string.Format("Get State couldn't find a state: ({0}-{1}={3})",target,position,diff));
            done = true;
            return 9;
        }        
    }

    public void ExecuteAction(int action){
        switch (action)
        {
            case 0:
                transform.position += new Vector3(0,1,0) * Time.deltaTime * moveSpeed;
                break;
            case 1:
                transform.position += new Vector3(1,0,0) * Time.deltaTime * moveSpeed;
                break;
            case 2:
                transform.position += new Vector3(0,-1,0) * Time.deltaTime * moveSpeed;
                break;
            case 3:
                transform.position += new Vector3(-1,0,0) * Time.deltaTime * moveSpeed;
                break;
            default:
                Debug.LogError(string.Format("Invalid action {0}",action));
                break;
        }
    }

    public void LoadBestTable(){
        explorationDecay = 1f;
        explorationProbability = 0f;
        minExplorationProbability = 0f;
        QTable = new float[,] {
        {7.945521f,6.826207f,6.994746f,44.03877f},
        {0,0,0,0},
        {0,0,0,0},
        {-0.06179959f,0.846605f,0.1397007f,-0.06930903f},
        {6.115326f,9.443285f,14.43366f,6.892287f},
        {69.86707f,68.88033f,86.95436f,71.18029f},
        {91.74245f,92.64734f,92.14013f,95.86502f},
        {88.68631f,73.29393f,74.51134f,76.27456f},
        {0,0,0,0},
        {0,0,0,0}
        };
    }


}
