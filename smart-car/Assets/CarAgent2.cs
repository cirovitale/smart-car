// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Sensors;
// using Unity.MLAgents.Actuators;

// [RequireComponent(typeof(Rigidbody))]
// public class CarAgent2 : Agent
// {
//      private Rigidbody carRigidbody;
//     public bool isManualControl = false;

//     // private float lastCheckpointTime; 
//     //  private float penaltyInterval = 5f; 

//     private float crossingStartTime;

//     // Parametri di guida
//      public float maxForwardSpeed = 20f;
//      public float maxBackwardSpeed = 10f;
//      public float acceleration = 7f;
//      public float brakingDeceleration = 7f;
//      public float idleDeceleration = 7f;
//      public float turnSpeed = 90f;

//     // Reward/Penalty definiti come variabili
//     private float rewardWallEnter         = -4f;   
//     private float rewardWallExit          = 1.1f;  
//     private float rewardCarEnter          = -1.5f; 
//     private float rewardCarExit           = 1.6f;  
//     private float rewardRoadLineEnter     = -2f; 
//     private float rewardRoadLineStay      = -0.7f; 
//     private float rewardRoadLineExit      = 1f;    
//     private float rewardCrossingExitSlow  = 3f;    
//     private float rewardCrossingExitFast  = -5f;   
//     private float rewardWallEndEnter      = -20f;
//     private float penaltyEndMap = -30f;
//     // private float rewardCheckpointSlow    = 2f;    
//     // private float rewardCheckpointFast    = 4f;    

//     private float penaltyVelocity         = -2f;   // Da usare in caso di velocità eccessiva

//      private float rewardRightCheckpoint = 1f;
//      private float rewardWrongCheckpoint = -5f;

//     // Soglie per velocità (in 5 m/s)
//     //  private float lowSpeedThreshold   = 10f;
//     //  private float highSpeedThreshold  = 45f;
//     //  private float rewardLowSpeed      = +0.01f; 
//     private float minMovingSpeedThreshold = 1f; 

//     private float currentSpeed = 0f;  
//     private float steeringInput;  
//     private bool accelerateInput;     
//     private bool brakeInput;          

//     private Vector3 spawnPosition;
//     private Quaternion spawnRotation;

//     // Variabili per i checkpoint ciclici
//     private int nextCheckpointIndex = 0; // Da 0 a 166
//     private int totalCheckpoints = 148;  // Se hai Checkpoint_1 (0) fino a (147)


    

    

//     private void Awake()
//     {
//         carRigidbody = GetComponent<Rigidbody>();
//         carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
//     }

//     private void Start()
//     {
//         spawnPosition = transform.position;
//         spawnRotation = transform.rotation;
//     }

//     private void Update()
//     {
//         // if (Time.time - lastCheckpointTime > penaltyInterval)
//         // {
//         //     AddReward(-50f); 
//         //     Debug.Log("[NO Checkpoint] -50");
//         //     lastCheckpointTime = Time.time;

//         //     if (GetCumulativeReward() < -3000f) EndEpisode();
//         // }

//         if (transform.position.y < -1f)
//         {
//             AddReward(penaltyEndMap);
//             Debug.Log("Caduto fuori dal mondo: " + penaltyEndMap);
//             EndEpisode();
//         }
//     }

//     public override void OnEpisodeBegin()
//     {
//         // lastCheckpointTime = Time.time;

//         // Reset fisica
//         carRigidbody.linearVelocity = Vector3.zero;
//         carRigidbody.angularVelocity = Vector3.zero;

//         // Reset posizione e rotazione
//         transform.position = spawnPosition;
//         transform.rotation = spawnRotation;

//         // Reset controlli
//         currentSpeed = 0f;
//         steeringInput = 0f;
//         accelerateInput = false;
//         brakeInput = false;

//         // Reset checkpoint
//         nextCheckpointIndex = 0;
//     }

//     public override void CollectObservations(VectorSensor sensor)
//     {
//         sensor.AddObservation(transform.forward);
//         sensor.AddObservation(currentSpeed);
//         sensor.AddObservation(steeringInput);
//         sensor.AddObservation(nextCheckpointIndex);
//     }

//     public override void OnActionReceived(ActionBuffers actions)
//     {
//         if (!isManualControl)
//         {
//             // Branch 0: steering (3 valori: -1, 0, +1)
//             steeringInput = actions.DiscreteActions[0] - 1f;

//             // Branch 1: accelerate or brake (2 valori)
//             if (actions.DiscreteActions[1] == 0)
//             {
//                 accelerateInput = false;
//                 brakeInput = true;
//             }
//             else
//             {
//                 accelerateInput = true;
//                 brakeInput = false;
//             }
//         }
//         else
//         {
//             // Controlli da tastiera
//             steeringInput = Input.GetAxis("Horizontal");
//             accelerateInput = Input.GetKey(KeyCode.W);
//             brakeInput      = Input.GetKey(KeyCode.S);
//         }

//         // ApplyMovement();
//         // CheckSpeedReward();
//     }

//     private void CheckSpeedReward()
//     {
//         float speedMagnitude = Mathf.Abs(currentSpeed);

//         // se stai sotto una certa soglia (bassa velocità).
//         // if (speedMagnitude < lowSpeedThreshold)
//         // {
//         //     AddReward(rewardLowSpeed);
//         //     Debug.Log("[Low Speed Reward] " + rewardLowSpeed);
//         // }

//         // // se la velocità è troppo alta
//         // if (speedMagnitude > highSpeedThreshold)
//         // {
//         //     AddReward(penaltyVelocity);
//         //     Debug.Log("[High Speed Penalty] " + penaltyVelocity);
//         // }

//          if (speedMagnitude < minMovingSpeedThreshold)
//         {
//             AddReward(penaltyVelocity);
//             Debug.Log("[Low Speed Penalty] " + penaltyVelocity);
//         }
//     }

//     private void FixedUpdate()
// {
//     ApplyMovement();
//     CheckSpeedReward();
// }


//     private void ApplyMovement()
//     {
//         // Accelera / Frena / Rallenta
//         if (accelerateInput)
//         {
//             currentSpeed += acceleration * Time.fixedDeltaTime;
//         }
//         else if (brakeInput)
//         {
//             currentSpeed -= brakingDeceleration * Time.fixedDeltaTime;
//         }
//         else
//         {
//             // Se non accelero né freno, rallento gradualmente
//             if (currentSpeed > 0)
//             {
//                 currentSpeed -= idleDeceleration * Time.fixedDeltaTime;
//                 if (currentSpeed < 0) currentSpeed = 0f; 
//             }
//             else if (currentSpeed < 0)
//             {
//                 currentSpeed += idleDeceleration * Time.fixedDeltaTime;
//                 if (currentSpeed > 0) currentSpeed = 0f; 
//             }
//         }

//         currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardSpeed, maxForwardSpeed);

//         float turnAngle = steeringInput * turnSpeed * Time.fixedDeltaTime;
//         Quaternion turnOffset = Quaternion.Euler(0f, turnAngle, 0f);

//         carRigidbody.MoveRotation(carRigidbody.rotation * turnOffset);

//         Vector3 move = carRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime;
//         carRigidbody.MovePosition(carRigidbody.position + move);


//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("Wall"))
//         {
//             AddReward(rewardWallEnter);
//             Debug.Log("[Wall Enter] " + rewardWallEnter);
//         }
//         if (collision.gameObject.CompareTag("Car"))
//         {
//             AddReward(rewardCarEnter);
//             Debug.Log("[Car Enter] " + rewardCarEnter);
//         }


//         if (collision.gameObject.CompareTag("WallEnd"))
//         {
//             AddReward(rewardWallEndEnter);
//             Debug.Log("[Wall End Enter] " + rewardWallEndEnter);
//             EndEpisode();
//         }
//     }

//     private void OnCollisionExit(Collision collision)
//     {
//         if (collision.gameObject.CompareTag("Wall"))
//         {
//             AddReward(rewardWallExit);
//             Debug.Log("[Wall Exit] " + rewardWallExit);
//         }
//         if (collision.gameObject.CompareTag("Car"))
//         {
//             AddReward(rewardCarExit);
//             Debug.Log("[Car Exit] " + rewardCarExit);
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         // -----------------------
//         // Checkpoint
//         // -----------------------
//         if (other.CompareTag("Checkpoint"))
//         {
//             int index = ParseCheckpointIndex(other.gameObject.name);

//             if (index == nextCheckpointIndex)
//             {
//                 // Reward
//                 AddReward(rewardRightCheckpoint);
//                 Debug.Log("[Checkpoint] Corretto => +" + rewardRightCheckpoint);
//                 // Passa al checkpoint successivo in modo ciclico
//                 nextCheckpointIndex = (nextCheckpointIndex + 1) % totalCheckpoints;
//             }
//             else
//             {
//                 AddReward(rewardWrongCheckpoint);
//                 Debug.Log("[Checkpoint] Sbagliato + " + rewardWrongCheckpoint);
//             }
//         }

//         // -----------------------
//         // RoadLine
//         // -----------------------
//         if (other.CompareTag("RoadLine"))
//         {
//             AddReward(rewardRoadLineEnter);
//             Debug.Log("[RoadLine Enter] " + rewardRoadLineEnter);
//         }

//         // -----------------------
//         // Crossing
//         // -----------------------
//         if (other.CompareTag("Crossing"))
//         {
//             crossingStartTime = Time.time;
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Crossing"))
//         {
//             float crossingEndTime = Time.time;
//             float crossingDuration = crossingEndTime - crossingStartTime;

//             if (crossingDuration < 2.0f)
//             {
//                 AddReward(rewardCrossingExitFast);
//                 Debug.Log("[Crossing Exit] " + rewardCrossingExitFast);
//             }
//             else
//             {
//                 AddReward(rewardCrossingExitSlow);
//                 Debug.Log("[Crossing Exit] " + rewardCrossingExitSlow);
//             }
//         }

//         if (other.CompareTag("RoadLine"))
//         {
//             AddReward(rewardRoadLineExit);
//             Debug.Log("[RoadLine Exit] " + rewardRoadLineExit);
//         }

//         // if (other.CompareTag("Map"))
//         // {
//         //     AddReward(-3000f);
//         //     Debug.Log("[Map Exit] -3000");
//         //     EndEpisode();
//         // }
//     }

//     private void OnTriggerStay(Collider other)
//     {
//         if (other.CompareTag("RoadLine"))
//         {
//             AddReward(rewardRoadLineStay);
//             Debug.Log("[RoadLine Stay] " + rewardRoadLineStay);
//         }
//     }

//     private int ParseCheckpointIndex(string checkpointName)
//     {
//         int openParenIndex = checkpointName.IndexOf('(');
//         int closeParenIndex = checkpointName.IndexOf(')');

//         if (openParenIndex < 0 || closeParenIndex < 0 || closeParenIndex <= openParenIndex)
//         {
//             return -1;
//         }

//         string numberString = checkpointName.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
        
//         int parsedIndex;
//         if (int.TryParse(numberString, out parsedIndex))
//         {
//             return parsedIndex;
//         }
        
//         return -1; // se fallisce
//     }
// }




using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic; // Per usare Dictionary<(int,int),List<(int,int)>>

[RequireComponent(typeof(Rigidbody))]
public class CarAgent2 : Agent
{
    private Rigidbody carRigidbody;
    public bool isManualControl = false;

    // private float lastCheckpointTime; 
    // private float penaltyInterval = 5f; 

    private float crossingStartTime;

    // Parametri di guida
    public float maxForwardSpeed = 20f;
    public float maxBackwardSpeed = 10f;
    public float acceleration = 7f;
    public float brakingDeceleration = 7f;
    public float idleDeceleration = 7f;
    public float turnSpeed = 90f;

    // Reward/Penalty definiti come variabili
    private float rewardWallEnter         = -4f;   
    private float rewardWallExit          = 1.1f;  
    private float rewardCarEnter          = -1.5f; 
    private float rewardCarExit           = 1.6f;  
    private float rewardRoadLineEnter     = -2f; 
    private float rewardRoadLineStay      = -0.7f; 
    private float rewardRoadLineExit      = 1f;    
    private float rewardCrossingExitSlow  = 3f;    
    private float rewardCrossingExitFast  = -5f;   
    private float rewardWallEndEnter      = -20f;
    private float penaltyEndMap           = -30f;
    // private float rewardCheckpointSlow  = 2f;   
    // private float rewardCheckpointFast  = 4f;   

    private float penaltyVelocity         = -2f;   // Da usare in caso di velocità eccessiva

    private float rewardRightCheckpoint   = 1f;
    private float rewardWrongCheckpoint   = -5f;

    private float minMovingSpeedThreshold = 1f; // Soglia per penalità se troppo lento

    private float currentSpeed = 0f;  
    private float steeringInput;  
    private bool accelerateInput;     
    private bool brakeInput;          

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private int currentCheckpointGroup;
    private int currentCheckpointIndex;

    // Dizionario delle transizioni valide:
    private Dictionary<(int group, int index), List<(int group, int index)>> validTransitions
        = new Dictionary<(int group, int index), List<(int group, int index)>>
    {
        // ----------------------
        // RAMO 0 (da 0 a 8)
        // ----------------------
        [(0,0)] = new List<(int,int)>{(0,0), (0,1)},
        [(0,1)] = new List<(int,int)>{(0,2)},
        [(0,2)] = new List<(int,int)>{(0,3)},
        [(0,3)] = new List<(int,int)>{(0,4)},
        [(0,4)] = new List<(int,int)>{(0,5)},
        [(0,5)] = new List<(int,int)>{(0,6)},
        [(0,6)] = new List<(int,int)>{(0,7)},
        [(0,7)] = new List<(int,int)>{(0,8)},

        // Incrocio: da 0(8) posso andare a 1(0) OPPURE 2(0)
        [(0,8)] = new List<(int,int)>{(1,0), (2,0)},

        // ----------------------
        // RAMO 1 (da 0 a 22)
        // ----------------------
        [(1,0)]  = new List<(int,int)>{(1,1)},
        [(1,1)]  = new List<(int,int)>{(1,2)},
        [(1,2)]  = new List<(int,int)>{(1,3)},
        [(1,3)]  = new List<(int,int)>{(1,4)},
        [(1,4)]  = new List<(int,int)>{(1,5)},
        [(1,5)]  = new List<(int,int)>{(1,6)},
        [(1,6)]  = new List<(int,int)>{(1,7)},
        [(1,7)]  = new List<(int,int)>{(1,8)},
        [(1,8)]  = new List<(int,int)>{(1,9)},
        [(1,9)]  = new List<(int,int)>{(1,10)},
        [(1,10)] = new List<(int,int)>{(1,11)},
        [(1,11)] = new List<(int,int)>{(1,12)},
        [(1,12)] = new List<(int,int)>{(1,13)},
        [(1,13)] = new List<(int,int)>{(1,14)},
        [(1,14)] = new List<(int,int)>{(1,15)},
        [(1,15)] = new List<(int,int)>{(1,16)},
        [(1,16)] = new List<(int,int)>{(1,17)},
        [(1,17)] = new List<(int,int)>{(1,18)},
        [(1,18)] = new List<(int,int)>{(1,19)},
        [(1,19)] = new List<(int,int)>{(1,20)},
        [(1,20)] = new List<(int,int)>{(1,21)},
        [(1,21)] = new List<(int,int)>{(1,22)},

        // Da 1(22) si ricongiunge a 4(0)
        [(1,22)] = new List<(int,int)>{(4,0)},

        // ----------------------
        // RAMO 2 (da 0 a 20)
        // ----------------------
        [(2,0)]  = new List<(int,int)>{(2,1)},
        [(2,1)]  = new List<(int,int)>{(2,2)},
        [(2,2)]  = new List<(int,int)>{(2,3)},
        [(2,3)]  = new List<(int,int)>{(2,4)},
        [(2,4)]  = new List<(int,int)>{(2,5)},
        [(2,5)]  = new List<(int,int)>{(2,6)},
        [(2,6)]  = new List<(int,int)>{(2,7)},
        [(2,7)]  = new List<(int,int)>{(2,8)},
        [(2,8)]  = new List<(int,int)>{(2,9)},
        [(2,9)]  = new List<(int,int)>{(2,10)},
        [(2,10)] = new List<(int,int)>{(2,11)},
        [(2,11)] = new List<(int,int)>{(2,12)},
        [(2,12)] = new List<(int,int)>{(2,13)},
        [(2,13)] = new List<(int,int)>{(2,14)},
        [(2,14)] = new List<(int,int)>{(2,15)},
        [(2,15)] = new List<(int,int)>{(2,16)},
        [(2,16)] = new List<(int,int)>{(2,17)},
        [(2,17)] = new List<(int,int)>{(2,18)},
        [(2,18)] = new List<(int,int)>{(2,19)},
        [(2,19)] = new List<(int,int)>{(2,20)},

        // Da 2(20) si ricongiunge a 4(0)
        [(2,20)] = new List<(int,int)>{(4,0)},

        // ----------------------
        // RAMO 4 (da 0 a 2)
        // ----------------------
        [(4,0)] = new List<(int,int)>{(4,1)},
        [(4,1)] = new List<(int,int)>{(4,2)},

        // Da 4(2) torna a 0(0)
        [(4,2)] = new List<(int,int)>{(0,0)},
    };

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
        // if (Time.time - lastCheckpointTime > penaltyInterval)
        // {
        //     AddReward(-50f); 
        //     Debug.Log("[NO Checkpoint] -50");
        //     lastCheckpointTime = Time.time;

        //     if (GetCumulativeReward() < -3000f) EndEpisode();
        // }

        // Se la macchina cade fuori dal mondo
        if (transform.position.y < -1f)
        {
            AddReward(penaltyEndMap);
            Debug.Log("Caduto fuori dal mondo: " + penaltyEndMap);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        // lastCheckpointTime = Time.time;

        // Reset fisica
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        // Reset posizione e rotazione
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        // Reset controlli
        currentSpeed = 0f;
        steeringInput = 0f;
        accelerateInput = false;
        brakeInput = false;

        currentCheckpointGroup = 0;
        currentCheckpointIndex = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(currentSpeed);
        sensor.AddObservation(steeringInput);

        sensor.AddObservation(currentCheckpointGroup);
        sensor.AddObservation(currentCheckpointIndex);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isManualControl)
        {
            // Branch 0: steering (3 valori: -1, 0, +1)
            steeringInput = actions.DiscreteActions[0] - 1f;

            // Branch 1: accelerate (1) o brake (0)
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
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSpeedReward();
    }

    private void CheckSpeedReward()
    {
        float speedMagnitude = Mathf.Abs(currentSpeed);

        if (speedMagnitude < minMovingSpeedThreshold)
        {
            AddReward(penaltyVelocity);
            Debug.Log("[Low Speed Penalty] " + penaltyVelocity);
        }
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
            // Se non accelero né freno, rallento gradualmente
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

        // Limita la velocità in avanti e indietro
        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardSpeed, maxForwardSpeed);

        // Sterzata
        float turnAngle = steeringInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnOffset = Quaternion.Euler(0f, turnAngle, 0f);
        carRigidbody.MoveRotation(carRigidbody.rotation * turnOffset);

        // Spostamento
        Vector3 move = carRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime;
        carRigidbody.MovePosition(carRigidbody.position + move);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(rewardWallEnter);
            Debug.Log("[Wall Enter] " + rewardWallEnter);
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(rewardCarEnter);
            Debug.Log("[Car Enter] " + rewardCarEnter);
        }

        if (collision.gameObject.CompareTag("WallEnd"))
        {
            AddReward(rewardWallEndEnter);
            Debug.Log("[Wall End Enter] " + rewardWallEndEnter);
            EndEpisode();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(rewardWallExit);
            Debug.Log("[Wall Exit] " + rewardWallExit);
        }
        if (collision.gameObject.CompareTag("Car"))
        {
            AddReward(rewardCarExit);
            Debug.Log("[Car Exit] " + rewardCarExit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // -----------------------
        // Checkpoint con branching
        // -----------------------
        if (other.CompareTag("Checkpoint"))
        {
            var (newGroup, newIndex) = ParseCheckpointName(other.gameObject.name);
            if (newGroup == -1 || newIndex == -1)
            {
                // Parsing fallito => penalizza o ignora
                AddReward(rewardWrongCheckpoint);
                Debug.Log("[Checkpoint] Parsing fallito => " + rewardWrongCheckpoint);
                return;
            }

            // Verifichiamo se (newGroup,newIndex) è tra i validi successori dell'attuale (currentCheckpointGroup, currentCheckpointIndex)
            if (validTransitions.TryGetValue((currentCheckpointGroup, currentCheckpointIndex), out List<(int,int)> possibleNext))
            {
                if (possibleNext.Contains((newGroup, newIndex)))
                {
                    // Correct checkpoint
                    AddReward(rewardRightCheckpoint);
                    Debug.Log($"[Checkpoint] CORRETTO: da ({currentCheckpointGroup},{currentCheckpointIndex}) a ({newGroup},{newIndex}) => +{rewardRightCheckpoint}");

                    // Aggiorna lo stato attuale
                    currentCheckpointGroup = newGroup;
                    currentCheckpointIndex = newIndex;
                }
                else
                {
                    // Non è tra i possibili => checkpoint sbagliato
                    AddReward(rewardWrongCheckpoint);
                    Debug.Log($"[Checkpoint] SBAGLIATO: ({newGroup},{newIndex}) non valido da ({currentCheckpointGroup},{currentCheckpointIndex}) => {rewardWrongCheckpoint}");
                }
            }
            else
            {
                // Non ci sono transizioni definite per (currentCheckpointGroup, currentCheckpointIndex)
                AddReward(rewardWrongCheckpoint);
                Debug.Log($"[Checkpoint] Nessuna transizione definita per ({currentCheckpointGroup},{currentCheckpointIndex}) => {rewardWrongCheckpoint}");
            }
        }

        // -----------------------
        // RoadLine
        // -----------------------
        if (other.CompareTag("RoadLine"))
        {
            AddReward(rewardRoadLineEnter);
            Debug.Log("[RoadLine Enter] " + rewardRoadLineEnter);
        }

        // -----------------------
        // Crossing
        // -----------------------
        if (other.CompareTag("Crossing"))
        {
            crossingStartTime = Time.time;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Crossing"))
        {
            float crossingEndTime = Time.time;
            float crossingDuration = crossingEndTime - crossingStartTime;

            if (crossingDuration < 2.0f)
            {
                AddReward(rewardCrossingExitFast);
                Debug.Log("[Crossing Exit] " + rewardCrossingExitFast);
            }
            else
            {
                AddReward(rewardCrossingExitSlow);
                Debug.Log("[Crossing Exit] " + rewardCrossingExitSlow);
            }
        }

        if (other.CompareTag("RoadLine"))
        {
            AddReward(rewardRoadLineExit);
            Debug.Log("[RoadLine Exit] " + rewardRoadLineExit);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RoadLine"))
        {
            AddReward(rewardRoadLineStay);
            Debug.Log("[RoadLine Stay] " + rewardRoadLineStay);
        }
    }
    
    private (int group, int index) ParseCheckpointName(string checkpointName)
    {
        string[] parts = checkpointName.Split(' ');
        if (parts.Length < 2) return (-1, -1);

        string groupPart = parts[0].Replace("Checkpoint_", "");

        string indexPart = parts[1].Replace("(", "").Replace(")", ""); 

        if (!int.TryParse(groupPart, out int g)) g = -1;
        if (!int.TryParse(indexPart, out int i)) i = -1;

        return (g, i);
    }
}
