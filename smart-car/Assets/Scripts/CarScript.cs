using UnityEngine;

public class CarScript : MonoBehaviour
{
    private float crossingStartTime;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("[Collision Enter - Wall]");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("[Collision Exit - Wall]");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoadLine"))
        {
            Debug.Log("[Trigger Enter - RoadLine]");
        }

        if (other.CompareTag("Crossing"))
        {
            crossingStartTime = Time.time;
            Debug.Log("[Trigger Enter - Crossing] at time: " + crossingStartTime);
        }

        if (other.CompareTag("TrafficLight"))
        {
            TrafficLightController trafficLight = other.GetComponent<TrafficLightController>();
            if (trafficLight != null)
            {
                Debug.Log("Detected Traffic Light with color: " + trafficLight.GetCurrentColor());
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RoadLine"))
        {
            Debug.Log("[Trigger Stay - RoadLine]");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RoadLine"))
        {
            Debug.Log("[Trigger Exit - RoadLine]");
        }

        if (other.CompareTag("Crossing"))
        {
            float crossingEndTime = Time.time;
            float crossingDuration = crossingEndTime - crossingStartTime;
            Debug.Log("[Trigger Exit - Crossing] at time: " + crossingEndTime);
            Debug.Log("[Trigger Exit - Crossing] time spent: " + crossingDuration + " seconds");
        }
    }
}
