using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class CarAgent : Agent
{
    [SerializeField] private Rigidbody carRigidbody; 
    private float crossingStartTime;

    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Vector3 spawnForward;

    public CheckpointsManager checksManager;

    // Variabili di controllo
    private float m_Steering;
    private bool m_Accelerate;
    private bool m_Brake;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        spawnPosition = carRigidbody.transform.position;
        spawnForward = carRigidbody.transform.forward;
    }

    // Chiamato a inizio episodio
    public override void OnEpisodeBegin()
    {
        checksManager.ResetCheckpoints();
        // 1. Azzeramento velocità
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        // 2. Reset posizione e rotazione
        carRigidbody.transform.position = spawnPosition;
        carRigidbody.transform.rotation = Quaternion.LookRotation(spawnForward);

        // 3. Reset variabili di controllo
        m_Steering = 0f;
        m_Accelerate = false;
        m_Brake = false;
    }

    // Raccolta osservazioni
    public override void CollectObservations(VectorSensor sensor)
    {
        // TODO da implementare
    }

    // Viene chiamato ogni volta che l’agente riceve una decisione
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Branch 0: 3 possibili valori (0, 1, 2) -> -1, 0, +1
        m_Steering = actions.DiscreteActions[0] - 1f;

        // Branch 1: 2 possibili valori (0, 1) -> frena, accelera
        if (actions.DiscreteActions[1] == 0)
        {
            m_Accelerate = false;
            m_Brake = true;
        }
        else
        {
            m_Accelerate = true;
            m_Brake = false;
        }

        // Applica il movimento in base agli input
        ApplyMovement();
    }

    // Applica le forze di movimento
    private void ApplyMovement()
    {
        float moveForce = 0f;
        if (m_Accelerate) moveForce = 500f;   // spinta in avanti
        if (m_Brake)      moveForce = -300f; // spinta all’indietro (o freno)

        // Sterzata
        float turnTorque = m_Steering * 100f;

        // Aggiungi forza in avanti
        carRigidbody.AddForce(transform.forward * moveForce);

        // Aggiungi torque per ruotare
        carRigidbody.AddTorque(Vector3.up * turnTorque);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-2f);
            Debug.Log("[Wall Enter] -2");
            // EndEpisode();
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(-2f);
            Debug.Log("[Car Enter] -2");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.25f);
            Debug.Log("[Wall Stay] -0.25");
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(-0.25f);
            Debug.Log("[Car Stay] -0.25");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(+3f);
            Debug.Log("[Wall Exit] +3");
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(+3f);
            Debug.Log("[Car Exit] +3");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoadLine"))
        {
            AddReward(-1f);
            Debug.Log("[RoadLine Enter] -1");
        }

        if (other.CompareTag("Crossing"))
        {
            crossingStartTime = Time.time;
            // Debug.Log("[Trigger Enter - Crossing] at time: " + crossingStartTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Crossing"))
        {
            float crossingEndTime = Time.time;
            float crossingDuration = crossingEndTime - crossingStartTime;
            // Debug.Log("[Trigger Exit - Crossing] at time: " + crossingEndTime);
            // Debug.Log("[Trigger Exit - Crossing] crossing in " + crossingDuration + " seconds");
            if (crossingDuration < 2.0f)
            {
                AddReward(-1f);
                Debug.Log("[Crossing Exit] -1");
            }
            else
            {
                AddReward(+2f);
                Debug.Log("[Crossing Exit] +2");
            }
        }

        if (other.CompareTag("RoadLine"))
        {
            AddReward(+1f);
            Debug.Log("[RoadLine Exit] +1");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RoadLine"))
        {
            AddReward(-0.1f);
            Debug.Log("[RoadLine Stay] -0.1");
        }
    }
}
