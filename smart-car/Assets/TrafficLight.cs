using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public GameObject redLight;    // Riferimento alla luce rossa
    public GameObject yellowLight; // Riferimento alla luce gialla
    public GameObject greenLight;  // Riferimento alla luce verde
    public Renderer cubeRenderer;  // Riferimento al renderer del cubo

    public Material redMaterial;   // Materiale rosso
    public Material yellowMaterial; // Materiale giallo
    public Material greenMaterial;  // Materiale verde

    private float timer;           // Timer per gestire le transizioni
    private int currentState;      // 0 = rosso, 1 = verde, 2 = giallo

    // Durate dei diversi stati del semaforo (in secondi)
    public float redDuration = 5.0f;
    public float greenDuration = 5.0f;
    public float yellowDuration = 2.0f;

    public string GetCurrentColor()
    {
        switch(currentState)
        {
            case 0:
                return "Red";
            case 1:
                return "Green";
            case 2:
                return "Yellow";
            default:
                return "Unknown";
        }
    }

    void Start()
    {
        timer = 0.0f;
        currentState = 0; // Inizia con la luce rossa
        UpdateTrafficLight();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case 0: // Rosso
                if (timer >= redDuration)
                {
                    currentState = 1; // Passa al verde
                    timer = 0.0f;
                    UpdateTrafficLight();
                }
                break;

            case 1: // Verde
                if (timer >= greenDuration)
                {
                    currentState = 2; // Passa al giallo
                    timer = 0.0f;
                    UpdateTrafficLight();
                }
                break;

            case 2: // Giallo
                if (timer >= yellowDuration)
                {
                    currentState = 0; // Torna al rosso
                    timer = 0.0f;
                    UpdateTrafficLight();
                }
                break;
        }
    }

    void UpdateTrafficLight()
    {
        // Attiva/disattiva le luci in base allo stato corrente
        redLight.SetActive(currentState == 0);
        greenLight.SetActive(currentState == 1);
        yellowLight.SetActive(currentState == 2);

        // Cambia il materiale del cubo
        switch (currentState)
        {
            case 0: // Rosso
                cubeRenderer.material = redMaterial;
                break;
            case 1: // Verde
                cubeRenderer.material = greenMaterial;
                break;
            case 2: // Giallo
                cubeRenderer.material = yellowMaterial;
                break;
        }
    }
}
