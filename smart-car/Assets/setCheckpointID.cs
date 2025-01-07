using UnityEngine;

public class setCheckpointID : MonoBehaviour
{
    public float row;
    public float column;

    private checkpointsManager checksManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AgentCarController>(out AgentCarController player))
        {
            checksManager.PlayerWentThroughCheck(this, other.transform);
        }
    }

    public void setRoadCheckpoints(checkpointsManager checksManager)
    {
        this.checksManager = checksManager;
    }
}
