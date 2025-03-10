using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class CarAgent : Agent
{
    [SerializeField] private Rigidbody carRigidbody;
    public bool isManualControl = false;

    public CheckpointsManager checksManager;

    private float lastCheckpointTime; // Tempo dell'ultimo checkpoint
    [SerializeField] private float penaltyInterval = 5f; // Intervallo per la penalità


    private float crossingStartTime;

    // Parametri di guida - la velocità è in m/s, l'accelerazione in m/s^2
    [SerializeField] private float maxForwardSpeed = 30f;
    [SerializeField] private float maxBackwardSpeed = 25f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float brakingDeceleration = 20f;
    [SerializeField] private float idleDeceleration = 10f;
    [SerializeField] private float turnSpeed = 180f;

    private float currentSpeed = 0f;  
    private float steeringInput;  
    private bool accelerateInput;     // Acceleratore
    private bool brakeInput;          // Freno

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    private void Update()
{
    // Controlla se è passato troppo tempo dall'ultimo checkpoint
    if (Time.time - lastCheckpointTime > penaltyInterval)
    {
        AddReward(-100f);
        Debug.Log("[NO Checkpoint] -100");

        lastCheckpointTime = Time.time;

        // Termina l'episodio dopo molte penalità
        if (GetCumulativeReward() < -2500f) EndEpisode();
    }
}

    public override void OnEpisodeBegin()
    {
        checksManager.ResetCheckpoints();
        lastCheckpointTime = Time.time;


        // Reset
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        currentSpeed = 0f;
        steeringInput = 0f;
        accelerateInput = false;
        brakeInput = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(currentSpeed);
        sensor.AddObservation(steeringInput);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isManualControl)
        {
            // Branch 0: steering (3 valori: -1, 0, +1)
            steeringInput = actions.DiscreteActions[0] - 1f;

            // Branch 1: accelerate or brake (2 valori)
            if (actions.DiscreteActions[1] == 0)
            {
                accelerateInput = false;
                brakeInput = true;
            }
            else
            {
                accelerateInput = true;
                brakeInput = false;
            }
        }
        else
        {
            // Controlli da tastiera
            steeringInput = Input.GetAxis("Horizontal");
            accelerateInput = Input.GetKey(KeyCode.W);
            brakeInput      = Input.GetKey(KeyCode.S);
        }

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        // Accelera / Frena / Rallenta
        if (accelerateInput)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if (brakeInput)
        {
            currentSpeed -= brakingDeceleration * Time.fixedDeltaTime;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= idleDeceleration * Time.fixedDeltaTime;
                if (currentSpeed < 0) currentSpeed = 0f; 
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += idleDeceleration * Time.fixedDeltaTime;
                if (currentSpeed > 0) currentSpeed = 0f; 
            }
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardSpeed, maxForwardSpeed);

        if (Mathf.Abs(currentSpeed) > 0)
        {
            // Ruota il veicolo in base allo steering
            float turnAngle = steeringInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(0f, turnAngle, 0f);
        }
        

        transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-50f);
            Debug.Log("[Wall Enter] -50");
            // EndEpisode();
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(-20f);
            Debug.Log("[Car Enter] -20");
            // EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-2f);
            Debug.Log("[Wall Stay] -2");
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(-1f);
            Debug.Log("[Car Stay] -1");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // if (collision.gameObject.CompareTag("Wall"))
        // {
        //     AddReward(+5f);
        //     Debug.Log("[Wall Exit] +5");
        // }
        // if (collision.gameObject.CompareTag("Car"))
        // {
        //     AddReward(+10f);
        //     Debug.Log("[Car Exit] +10");
        // }
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

    public void UpdateCheckpointTime()
    {
        lastCheckpointTime = Time.time;
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
                AddReward(-30f);
                Debug.Log("[Crossing Exit] -30");
            }
            else
            {
                AddReward(+10f);
                Debug.Log("[Crossing Exit] +10");
            }
        }

        if (other.CompareTag("RoadLine"))
        {
            AddReward(+1f);
            Debug.Log("[RoadLine Exit] +1");
        }

        if (other.CompareTag("Map"))
        {
            AddReward(-1000f);
            Debug.Log("[Map Exit] -1000");
            EndEpisode();
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
