using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentCarController : Agent
{
    // [SerializeField] private Transform target;

    //onEpisodeBegin() da fare, praticamente spawna la macchina

    public override void CollectObservations(VectorSensor sensor)
    {
        //penso sia da implementare meglio, deve puntare sempre al checkpoint successivo perchè così impara ad andare dritto TODO
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //float move = actions.ContinuousActions[0];
        float moveSpeed = 2f;
        float forwardAmount = 0f;
        float turnAmount = 0f;

        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +1f; break;
            case 2: forwardAmount = -1f; break;
        }
        switch (actions.DiscreteActions[0])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +1f; break;
            case 2: turnAmount = -1f; break;
        }
        //settare questi due valori nella macchina TODO

        transform.localPosition += new Vector3(forwardAmount, 0f) * Time.deltaTime * moveSpeed;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.5f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.1f);
        }
    }*/
}
