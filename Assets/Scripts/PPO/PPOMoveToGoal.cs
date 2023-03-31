using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PPOMoveToGoal : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private SpriteRenderer floor;
    

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(0f,8f), Random.Range(3.5f,-3.5f), 0);
        target.transform.localPosition = new Vector3(Random.Range(-1f,-8f), Random.Range(3.5f,-3.5f), 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        AddReward(-0.01f);
        
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if( other.gameObject.tag == "Wall"){
            AddReward(-100f);
            floor.color = loseColor;
            EndEpisode();
        }
        else if( other.gameObject.tag == "Goal"){
            AddReward(100f);
            floor.color = winColor;
            EndEpisode();
        }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

}
