using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public GameObject redLight;    
    public GameObject yellowLight; 
    public GameObject greenLight;  
    public Renderer cubeRenderer;  

    public Material redMaterial;   
    public Material yellowMaterial; 
    public Material greenMaterial;  

    private float timer;           
    private int currentState;     

    
    private float redDuration = 10.0f;
    private float greenDuration = 8.0f;
    private float yellowDuration = 2.0f;

    public int initialState = 0;

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

    public void InitializaState(int state)
    {
        timer = 0.0f;
        currentState = state;
        UpdateTrafficLight();
    }

    void Start()
    {
        timer = 0.0f;
        currentState = initialState; // Inizia con la luce rossa
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

        switch (currentState)
        {
            case 0: 
                cubeRenderer.material = redMaterial;
                break;
            case 1: 
                cubeRenderer.material = greenMaterial;
                break;
            case 2: 
                cubeRenderer.material = yellowMaterial;
                break;
        }
    }
}
