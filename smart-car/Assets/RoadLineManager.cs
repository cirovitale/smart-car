using UnityEngine;
using System.Linq;  // Per usare .Where() e simili

public class RoadLineManager : MonoBehaviour
{
    [Header("Container di RoadLine nella Scena")]
    [Tooltip("Assegna qui il GameObject che contiene come figli tutti i segmenti di RoadLine.")]
    public Transform roadLinesParent;

    // memorizzo tutti i children
    private Transform[] allLines;

    private void Awake()
    {
        // Se l'oggetto parent è stato assegnato in Inspector,
        // recupera tutti i Transform figli (escludendo il parent stesso).
        if (roadLinesParent != null)
        {
            allLines = roadLinesParent.GetComponentsInChildren<Transform>()
                                      .Where(t => t != roadLinesParent)
                                      .ToArray();

            Debug.Log($"Trovate {allLines.Length} roadline(s) nel parent.");
        }
        else
        {
            Debug.LogWarning("roadLinesParent non assegnato. Assicurati di trascinare l'oggetto contenitore in scena.");
        }
    }

    public Transform GetClosestRoadLine(Vector3 position)
    {
        if (allLines == null || allLines.Length == 0)
        {
            return null; 
        }

        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var line in allLines)
        {
            float dist = Vector3.Distance(position, line.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = line;
            }
        }

        return closest;
    }
    
    public Transform[] GetAllRoadLines()
    {
        return allLines;
    }
}
