using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class CarAgent2 : Agent
{
    public int startCheckpointGroup;
    public int startCheckpointIndex;

    private Rigidbody carRigidbody;
    public bool isManualControl = false;

    public bool isSimpleTrack = false;

    public TrafficLightController trafficLightController;
    public int simpleTrackTrafficLightState = 0;


    // private float lastCheckpointTime; 
    // private float penaltyInterval = 5f; 

    private float crossingStartTime;

    

    // Parametri di guida
    private float maxForwardSpeed = 10f;
    private float maxBackwardSpeed = 10f;
    private float acceleration = 7f;
    private float brakingDeceleration = 7f;
    private float idleDeceleration = 7f;
    private float turnSpeed = 90f;

    // Reward/Penalty definiti come variabili
    private float rewardWallEnter         = -1f;
    private float rewardWallEnterLightZone = -7f;  
    private float rewardWallEnterLightZoneStay = -1f;    
    private float rewardWallExit          = 0f;  
    // private float rewardCarEnter          = -0.4f; 
    // private float rewardCarExit           = 0.2f;  
    private float rewardCarEnter          = -5f; 
    private float rewardCarExit           = 0f;  
    private float rewardRoadLineEnter     = -0.01f; 
    private float rewardRoadLineStay      = -0.002f; 
    // private float rewardRoadLineExit      = 0.1f;    
    private float rewardRoadLineExit      = 0f;    
    private float rewardCrossingExitSlow  = 15f;    
    private float rewardCrossingExitFast  = -15f;   
    private float rewardWallEndEnter      = -10f;
    private float penaltyEndMap           = -3f;
    // private float rewardCheckpointSlow  = 2f;   
    // private float rewardCheckpointFast  = 4f;   

    private float penaltyVelocity         = -0.05f;

    private float rewardFinish = 1.0f;
    private float rewardWrongFinish = -15.0f;

    private float rewardRightCheckpoint   = 3f;

    private float rewardBonusBranchCheckpoint   = 25f;
    private float rewardWrongCheckpoint   = -50f;

    // Riferimento all'ultimo semaforo rilevato
    private TrafficLightController currentTrafficLight = null;
    private string currentColor = "Green";
    // per on stay, considerando circa 50 frame al secondo, dare reward minimo
    private float rewardStopOnRed = 0.5f;

    private float rewardCarStay = -0.05f;

    private float rewardWallStay = -0.05f;

    private float penaltyStopOnGreenYellow = -0.1f;
    private float rewardCrossOnGreenYellow = 5f;
    private float penaltyCrossOnRed = -15f;

    private float penaltyGoOnRed = -0.5f;
    private float rewardGoOnGreenYellow = 0.2f;

    private float minMovingSpeedThreshold = 4f;

    private float stopSpeedThreshold = 2f;
    private float goSpeedThreshold = 4f;

    private float currentSpeed = 0f;  
    private float steeringInput;  
    private bool accelerateInput;     
    private bool brakeInput;     
    private bool isInRedZone;     

    private bool favorBranchOne = true;


    // private bool toggleDirection = false;


    // private float greenSpeedTimer = 0f;


    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private int currentCheckpointGroup;
    private int currentCheckpointIndex;

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        // Controllo della sterzata
        if (Input.GetKey(KeyCode.A))
        {
            steeringInput = -1f; 
            discreteActions[0] = 0; 
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steeringInput = 1f;
            discreteActions[0] = 2; 
        }
        else
        {
            steeringInput = 0f;
            discreteActions[0] = 1;
        }

        // Controllo accelerazione e frenata
        if (Input.GetKey(KeyCode.W))
        {
            accelerateInput = true;  
            brakeInput = false;
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            accelerateInput = false;
            brakeInput = true; 
            discreteActions[1] = 0;
        }
        else
        {
            accelerateInput = false;
            brakeInput = false;  // Nessun tasto premuto -> Auto ferma
            discreteActions[1] = 2; // Neutro 
        }
    }


    // Dizionario transizioni valide
    private Dictionary<(int group, int index), List<(int group, int index)>> validTransitions
        = new Dictionary<(int group, int index), List<(int group, int index)>>
    {
        // ----------------------
        // RAMO 0 (da 0 a 8)
        // ----------------------
        [(0,0)] = new List<(int,int)>{(0,0),(0,1)},
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
        if (GetCumulativeReward() < -1000f) EndEpisode();
        // if (Time.time - lastCheckpointTime > penaltyInterval)
        // {
        //     AddReward(-50f); 
        //     // Debug.Log("[NO Checkpoint] -50");
        //     lastCheckpointTime = Time.time;

        //     if (GetCumulativeReward() < -3000f) EndEpisode();
        // }

        // Se la macchina cade fuori dal mondo
        if (transform.position.y < -1f)
        {
            AddReward(penaltyEndMap);
            // Debug.Log("Caduto fuori dal mondo: " + penaltyEndMap);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        // lastCheckpointTime = Time.time;

        // Reset
        carRigidbody.linearVelocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        currentSpeed = 0f;
        steeringInput = 0f;
        accelerateInput = false;
        brakeInput = false;

        currentCheckpointGroup = startCheckpointGroup;
        currentCheckpointIndex = startCheckpointIndex;

        if (isSimpleTrack)
        {
            trafficLightController.InitializaState(simpleTrackTrafficLightState);
        }

        // toggleDirection = !toggleDirection;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Stato interno dell'auto: velocità e sterzo
        sensor.AddObservation(currentSpeed); 
        sensor.AddObservation(steeringInput);  

        // checkpoint attuale (group,index)
        sensor.AddObservation(currentCheckpointGroup); 
        sensor.AddObservation(currentCheckpointIndex); 

        // Direzione e distanza dal PROSSIMO checkpoint
        // Vector3 nextCheckpointPos = GetNextCheckpointPosition();
        // Vector3 toNextCheckpoint = nextCheckpointPos - transform.position;

        // Vector3 localDir = transform.InverseTransformDirection(toNextCheckpoint.normalized);
        // sensor.AddObservation(localDir.x);  
        // sensor.AddObservation(localDir.z);

        // Incrocio
        List<Vector3> cpPositions = GetNextCheckpointPositions(); 
        // Checkpoint 1
        if (cpPositions.Count >= 1)
        {
            Vector3 cp1 = cpPositions[0];
            Vector3 dir1 = cp1 - transform.position;
            Vector3 localDir1 = transform.InverseTransformDirection(dir1.normalized);

            sensor.AddObservation(localDir1.x);
            sensor.AddObservation(localDir1.z);
        }
        // Checkpoint 2
        if (cpPositions.Count == 2)
        {
            Vector3 cp2 = cpPositions[1];
            Vector3 dir2 = cp2 - transform.position;
            Vector3 localDir2 = transform.InverseTransformDirection(dir2.normalized);

            sensor.AddObservation(localDir2.x);
            sensor.AddObservation(localDir2.z);
        } else {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        // Semaforo
        // int colorCode = 2; // Default = Green
        // if (currentTrafficLight != null)
        // {
        //     string color = currentTrafficLight.GetCurrentColor();
        //     if (color == "Red") colorCode = 0;
        //     if (color == "Green") colorCode = 1;
        //     if (color == "Yellow") colorCode = 2; 
        // } else {
        //     colorCode = -1;
        // }

        // sensor.AddObservation(colorCode);


        int isRed = 0, isGreen = 0, isYellow = 0;
        float distanceToTL = 100f; // Valore di default se non c'è il semaforo
        if (currentTrafficLight != null)
        {
            string color = currentTrafficLight.GetCurrentColor();
            if (color == "Red") 
                isRed = 1;
            else if (color == "Green")
                isGreen = 1;
            else if (color == "Yellow")
                isYellow = 1;

            // Calcola la distanza dal semaforo
            distanceToTL = Vector3.Distance(transform.position, currentTrafficLight.transform.position);
        }
        sensor.AddObservation(isRed);
        sensor.AddObservation(isGreen);
        sensor.AddObservation(isYellow);
        sensor.AddObservation(distanceToTL);

    }

    private List<Vector3> GetNextCheckpointPositions()
    {
        var positions = new List<Vector3>();

        if (validTransitions.TryGetValue((currentCheckpointGroup, currentCheckpointIndex), out List<(int,int)> nextList))
        {
            foreach(var (g, i) in nextList)
            {
                string cpName = $"Checkpoint_{g} ({i})";
                GameObject cpObj = GameObject.Find(cpName);
                if (cpObj != null)
                {
                    positions.Add(cpObj.transform.position);
                }
            }
        }
        
        return positions;
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

        // bool isInRedZone = false;

        // if (currentTrafficLight != null && currentTrafficLight.GetCurrentColor() == "Red")
        // {
        //     isInRedZone = true;
        // }

        isInRedZone = (currentTrafficLight != null && currentTrafficLight.GetCurrentColor() == "Red") || false;


        if (speedMagnitude < minMovingSpeedThreshold && !isInRedZone)
        {
            AddReward(penaltyVelocity);
            // Debug.Log("[Low Speed Penalty] " + penaltyVelocity);
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

        // Limita velocità in avanti e indietro
        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackwardSpeed, maxForwardSpeed);

        // Sterzata e spostamento
        float turnAngle = steeringInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnOffset = Quaternion.Euler(0f, turnAngle, 0f);
        carRigidbody.MoveRotation(carRigidbody.rotation * turnOffset);

        Vector3 move = carRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime;
        carRigidbody.MovePosition(carRigidbody.position + move);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if(currentTrafficLight != null)
            {
                AddReward(rewardWallEnterLightZone);
                // Debug.Log("[Wall Enter Light Zone] " + rewardWallEnterLightZone);
            } else {
                AddReward(rewardWallEnter);
                // Debug.Log("[Wall Enter] " + rewardWallEnter);
            }
        }
        if (collision.gameObject.CompareTag("Car") && !isSimpleTrack)
        {
            AddReward(rewardCarEnter);
            // Debug.Log("[Car Enter] " + rewardCarEnter);
        }

        if (collision.gameObject.CompareTag("WallEnd"))
        {
            AddReward(rewardWallEndEnter);
            // Debug.Log("[Wall End Enter] " + rewardWallEndEnter);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if(currentTrafficLight != null)
            {
                AddReward(rewardWallEnterLightZoneStay);
                // Debug.Log("[Wall Stay Light Zone] " + rewardWallEnterLightZoneStay);
            } else {
                AddReward(rewardWallStay);
            // Debug.Log("[Wall Stay] " + rewardWallStay);
            }
        }
        if (collision.gameObject.CompareTag("Car") && !isSimpleTrack)
        {
            AddReward(rewardCarStay);
            // Debug.Log("[Car Stay] " + rewardCarStay);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(rewardWallExit);
            // Debug.Log("[Wall Exit] " + rewardWallExit);
        }
        if (collision.gameObject.CompareTag("Car") && !isSimpleTrack)
        {
            AddReward(rewardCarExit);
            // Debug.Log("[Car Exit] " + rewardCarExit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            var (newGroup, newIndex) = ParseCheckpointName(other.gameObject.name);
            if (newGroup == -1 || newIndex == -1)
            {
                AddReward(rewardWrongCheckpoint);
                EndEpisode();
                // Debug.Log("[Checkpoint] Parsing fallito => " + rewardWrongCheckpoint);
                return;
            }

            // Controllo dell'incrocio: se siamo al checkpoint (0,8) 
            if (currentCheckpointGroup == 0 && currentCheckpointIndex == 8)
            {
                // se il flag favorisce il ramo 1,0
                if (favorBranchOne)
                {
                    if (newGroup == 1 && newIndex == 0)
                    {
                        AddReward(rewardBonusBranchCheckpoint); 
                        // Debug.Log("[Checkpoint Incrocio]: " + rewardBonusBranchCheckpoint);
                        favorBranchOne = false; 
                    }
                }
                else
                {
                    if (newGroup == 2 && newIndex == 0)
                    {
                        AddReward(rewardBonusBranchCheckpoint); // bonusCheckpointIntersection
                        // Debug.Log("[Checkpoint Incrocio]: " + rewardBonusBranchCheckpoint);
                        favorBranchOne = true; 
                    }
                }
            }

            // Verifichiamo se (newGroup,newIndex) è tra i validi successori dell'attuale (currentCheckpointGroup, currentCheckpointIndex)
            if (validTransitions.TryGetValue((currentCheckpointGroup, currentCheckpointIndex), out List<(int,int)> possibleNext))
            {
                if (possibleNext.Contains((newGroup, newIndex)))
                {
                    if (newGroup == 0 && newIndex == 0)
                    {
                        AddReward(0);
                        // Debug.Log("[Checkpoint] Raggiunto (0, 0) => Ricompensa: 0");
                    } else {
                        AddReward(rewardRightCheckpoint);
                        // Debug.Log($"[Checkpoint] CORRETTO: da ({currentCheckpointGroup},{currentCheckpointIndex}) a ({newGroup},{newIndex}) => +{rewardRightCheckpoint}");
                    }

                    // Aggiorna lo stato attuale
                    currentCheckpointGroup = newGroup;
                    currentCheckpointIndex = newIndex;
                }
                else
                {
                    AddReward(rewardWrongCheckpoint);
                    EndEpisode();
                    // Debug.Log($"[Checkpoint] SBAGLIATO: ({newGroup},{newIndex}) non valido da ({currentCheckpointGroup},{currentCheckpointIndex}) => {rewardWrongCheckpoint}");
                }
            }
            else
            {
                AddReward(rewardWrongCheckpoint);
                EndEpisode();
                // Debug.Log($"[Checkpoint] Nessuna transizione definita per ({currentCheckpointGroup},{currentCheckpointIndex}) => {rewardWrongCheckpoint}");
            }
        }

        if (other.CompareTag("RoadLine") && !isSimpleTrack)
        {
            AddReward(rewardRoadLineEnter);
            // Debug.Log("[RoadLine Enter] " + rewardRoadLineEnter);
        }

        if (other.CompareTag("Crossing") && !isSimpleTrack)
        {
            crossingStartTime = Time.time;
        }
        
        // traffic light zone
        if (other.CompareTag("TrafficLightZone"))
        {
            // Prendiamo il TrafficLightController presente nel parent (o oggetto associato)
            TrafficLightController tl = other.GetComponentInParent<TrafficLightController>();
            if (tl != null)
            {
                currentTrafficLight = tl;
                currentColor = currentTrafficLight.GetCurrentColor();
                // Debug.Log($"[TrafficLightZone] Enter => {currentColor}");
            }
        }
        // STOP line
        else if (other.CompareTag("Stop"))
        {
            if (currentColor == "Red")
            {
                // Penalty se attraversa il rosso
                AddReward(penaltyCrossOnRed);
                // Debug.Log($"[StopLine] Crossed on Red => {penaltyCrossOnRed}");
            }
            else if (currentColor == "Green" || currentColor == "Yellow")
            {
                // Reward se attraversa con verde o giallo
                AddReward(rewardCrossOnGreenYellow);
                // Debug.Log($"[StopLine] Crossed on {currentColor} => {rewardCrossOnGreenYellow}");
            }

            currentTrafficLight = null;
            currentColor = "Green";
        }
        


        if (other.CompareTag("Finish"))
        {
            // Piccola reward per finire correttamente
            AddReward(rewardFinish);
            EndEpisode();
            
        }
        if (other.CompareTag("FinishWrong"))
        {
            // Piccola penalty per finire 
            AddReward(rewardWrongFinish);
            EndEpisode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Crossing") && !isSimpleTrack)
        {
            float crossingEndTime = Time.time;
            float crossingDuration = crossingEndTime - crossingStartTime;

            if (crossingDuration < 1.0f)
            {
                AddReward(rewardCrossingExitFast);
                // Debug.Log("[Crossing Exit] " + rewardCrossingExitFast);
            }
            else
            {
                AddReward(rewardCrossingExitSlow);
                // Debug.Log("[Crossing Exit] " + rewardCrossingExitSlow);
            }
        }

        if (other.CompareTag("RoadLine") && !isSimpleTrack)
        {
            AddReward(rewardRoadLineExit);
            // Debug.Log("[RoadLine Exit] " + rewardRoadLineExit);
        }

        if (other.CompareTag("Stop"))
        {
            // Uscito dalla stop line
            // currentTrafficLight = null;
            // currentColor = "Green";
            // greenSpeedTimer = 0f;
        }

        if (other.CompareTag("TrafficLightZone"))
        {
            // greenSpeedTimer = 0f;
            currentTrafficLight = null;
            currentColor = "Green"; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RoadLine") && !isSimpleTrack)
        {
            AddReward(rewardRoadLineStay);
            // Debug.Log("[RoadLine Stay] " + rewardRoadLineStay);
        }

        if (other.CompareTag("TrafficLightZone"))
        {
            if (currentTrafficLight != null)
            {
                currentColor = currentTrafficLight.GetCurrentColor();
            }

            // Debug.Log($"[TrafficLightZone] Stay => {currentColor}");



            // fermo su
            float speedAbs = Mathf.Abs(currentSpeed);
            if (currentColor == "Red")
            {
                if (speedAbs < stopSpeedThreshold)
                {
                    AddReward(rewardStopOnRed);
                }
                else
                {
                    AddReward(penaltyGoOnRed);
                }
            }
            else if (currentColor == "Yellow")
            {
                if (speedAbs < goSpeedThreshold)
                {
                    AddReward(penaltyStopOnGreenYellow);
                }
                else
                {
                    AddReward(rewardGoOnGreenYellow);
                }
            }
            else if (currentColor == "Green")
            {
                if (speedAbs < goSpeedThreshold)
                {
                    AddReward(penaltyStopOnGreenYellow);
                    // greenSpeedTimer = 0f;
                }
                else
                {
                    // greenSpeedTimer += Time.deltaTime;
                    // if (greenSpeedTimer > 4.0f)
                    // {
                    //     AddReward(-50f);
                    //     // Debug.Log("[Penalità bug semaforo] -50");
                    //     EndEpisode();
                    // }
                    AddReward(rewardGoOnGreenYellow);
                }
            }
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
