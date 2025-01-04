using UnityEngine;

public class checkpointsManager : MonoBehaviour
{
    [SerializeField] private float[,] mappaCheckpoints = new float[15, 27];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (float i in mappaCheckpoints)
        {
            print(i);
        }

        //NOTA IMPORTANTE: i checkpoint non vengono presi in ordine, forse conviene salvarli di volta in volta nella matrice usando le coordinare riga/colonna ottenute
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (GameObject checkpoint in checkpoints)
        {
            float currentIterationRow = checkpoint.GetComponent<setCheckpointID>().row;
            float currentIterationColumn = checkpoint.GetComponent<setCheckpointID>().column;
            print(currentIterationRow);
            print(currentIterationColumn);

            mappaCheckpoints[(int)currentIterationRow, (int)currentIterationColumn] = 1;
        }

        foreach (float i in mappaCheckpoints)
        {
            print(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
