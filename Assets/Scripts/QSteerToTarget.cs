using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSteerToTarget : QAgent
{
    /*
    [4 States, 3 Actions]
    Actions:
        0: Move Forward
        1: Turn Left (also moves forward a bit)
        2: Turn Right (also moves forward a bit)
    States:
        0: Target is in front (utilizes cone check)
        1: Target is to the left
        2: Target is to the right
        3: Hit Target
     */

     /*
     Best Table:
    (257.5778,720.5385,255.8222)
    (276.9714,719.3155,291.2688)
    (388.7945,351.0731,714.1392)
    (0,0,0)

      */

    [Header("MoveToTarget")]
    public Transform target;
    public float moveSpeed = 3f;
    public float rotationSpeed = 100f;
    public float goalRadius = 0.3f;
    public float distanceDeltaCutoff = 0.1f;
    public bool loadBestTable = false;
    public float coneAngle = 10f;
    public float lastAngle;

    [Header("Visualization")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private SpriteRenderer floor;

    public override void Start(){
        base.Start();

        if (loadBestTable) LoadBestTable();

        lastAngle = SteerAngle(transform.position, target.position);
    }

    public override void OnEpisodeBegin(){
        base.OnEpisodeBegin();
        transform.rotation = Quaternion.identity;
        transform.localPosition = new Vector3(Random.Range(-8f,8f), Random.Range(3.5f,-3.5f), 0);
        target.transform.localPosition = new Vector3(Random.Range(-8f,8f), Random.Range(3.5f,-3.5f), 0);
    }

    public override int GetNextState(int currentState, int currentAction){
        ExecuteAction(currentAction);
        return GetState(transform.localPosition, target.localPosition);
    }

    public int GetState(Vector2 position, Vector2 target){
        if (Vector2.Distance(position, target) < goalRadius){
            return 3;
        }
        else if (InCone(position, target)){
            return 0;
        }
        else if(IsRight(position, target)){
            return 2;
        }
        else{
            return 1;
        }
    }

    public override float GetReward(Vector2 oldPosition, Vector2 newPosition, int nextState){
        if (nextState == 3){
            floor.color = winColor;
            Debug.Log("Done!");
            done = true;
            return 1000f;
        }
        float reward = 0f;
        float targetDistanceDelta = Vector2.Distance(newPosition, target.localPosition) - Vector2.Distance(oldPosition, target.localPosition);
        if (targetDistanceDelta < distanceDeltaCutoff) targetDistanceDelta = 0;
        if (targetDistanceDelta > 0){
            //further from target, punish
            reward += -1f;
        }
        else if(targetDistanceDelta < 0){
            //closer to target, reward
            reward += 1f;
        }
        return reward;
    }

    public void ExecuteAction(int action){
        switch (action)
        {
            case 0:
                transform.position += transform.up * Time.deltaTime * moveSpeed;
                break;
            case 1:
                transform.position += transform.up * Time.deltaTime * (moveSpeed/2);
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                break;
            case 2:
                transform.position += transform.up * Time.deltaTime * (moveSpeed/2);
                transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
                break;
            default:
                Debug.LogError(string.Format("Invalid action {0}",action));
                break;
        }
    }

    private bool IsRight(Vector2 position, Vector2 target){
        return Vector3.Cross(transform.up, target-position).z < 0f;
    }  

    public float SteerAngle(Vector2 position, Vector2 target){
        return Vector2.Angle(transform.up, target-position);
    }

    private bool InCone(Vector2 position, Vector2 target){
        return Vector2.Angle(transform.up, target-position) < coneAngle;
    }  

    private bool InBehindCone(Vector2 position, Vector2 target){
        return Vector2.Angle(-transform.up, target-position) < coneAngle;
    }  



    public void LoadBestTable(){
        explorationDecay = 1f;
        explorationProbability = 0f;
        minExplorationProbability = 0f;

        QTable = new float[,] {
        {257.5778f,720.5385f,255.8222f},
        {276.9714f,719.3155f,291.2688f},
        {388.7945f,351.0731f,714.1392f},
        {0,0,0}
        };

    }

    //used for show, not used in training (It doesn't provide any punishment)
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Wall"){
            floor.color = loseColor;
            done = true;
        }
    }


}
