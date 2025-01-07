using UnityEngine;

public class setCheckpointID : MonoBehaviour
{
    public float row;
    public float column;

    private CheckpointsManager checksManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarAgent>(out CarAgent player))
        {
            checksManager.PlayerWentThroughCheck(this, other.transform);           
        }
    }

    public void setRoadCheckpoints(CheckpointsManager checksManager)
    {
        this.checksManager = checksManager;
    }
}
