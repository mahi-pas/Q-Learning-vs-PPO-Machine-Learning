using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PPOSteerToTarget : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float closestDistance;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private SpriteRenderer floor;

    public override void OnEpisodeBegin()
    {
        transform.rotation = Quaternion.identity;
        transform.localPosition = new Vector3(Random.Range(-8f,8f), Random.Range(3.5f,-3.5f), 0);
        target.transform.localPosition = new Vector3(Random.Range(-8f,8f), Random.Range(3.5f,-3.5f), 0);

        closestDistance = Vector2.Distance(transform.position,target.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.up);
        sensor.AddObservation(target.transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        transform.position += transform.up * Time.deltaTime * moveSpeed * moveX;
        transform.Rotate(0, 0, moveY * rotationSpeed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance < closestDistance){
            AddReward(1f);
            closestDistance = distance;
        }
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
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = -Input.GetAxisRaw("Horizontal");
    }



}
