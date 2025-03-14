using System.Collections.Generic;
using UnityEngine;

//TODO capire errore cannot create new with monoBehaviour 

public class CheckpointsManager : MonoBehaviour
{
    [SerializeField] private setCheckpointID[,] mappaCheckpoints = new setCheckpointID[15, 27];
    private List<List<CheckpointsListHandler>> nextPossibleCheckpoints;

    [SerializeField] private List<Transform> carTransformList;
    private List<setCheckpointID> lastCheckpointsList;

    public void ResetCheckpoints()
    {
        lastCheckpointsList = new List<setCheckpointID>();
        nextPossibleCheckpoints = new List<List<CheckpointsListHandler>>();

        //inizializza lista per ogni macchina da gestire e anche last check
        foreach (Transform car in carTransformList)
        {
            List<CheckpointsListHandler> tempList = new List<CheckpointsListHandler>();
            CheckpointsListHandler temp = new CheckpointsListHandler();
            temp.initObj(-1, -1);
            tempList.Add(temp);
            nextPossibleCheckpoints.Add(tempList);

            //last sempre vuoto tanto poi lo setta la macchina
            setCheckpointID tempLast = new setCheckpointID();
            lastCheckpointsList.Add(tempLast);
        }

        Transform checkpointsTransform = transform.Find("Checkpoints");

        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");


        foreach (GameObject checkpoint in checkpoints)
        {
            setCheckpointID check = checkpoint.GetComponent<setCheckpointID>();
            float currentIterationRow = check.row;
            float currentIterationColumn = check.column;

            check.setRoadCheckpoints(this);

            mappaCheckpoints[(int)currentIterationRow, (int)currentIterationColumn] = check;
        }


    }


    public void PlayerWentThroughCheck(setCheckpointID checkpoint, Transform carTransform)
    {

        //preleviamo la lista da gestire tra le n liste delle n macchine
        int nextChecksIndex = carTransformList.IndexOf(carTransform);
        List<CheckpointsListHandler> actualCarPossibleChecks = nextPossibleCheckpoints[nextChecksIndex];
        setCheckpointID lastCheckpointCar = lastCheckpointsList[nextChecksIndex];

        /*Debug.Log("Lista check possibili");
        for (int i = 0; i < actualCarPossibleChecks.Count; i++)
        {
            Debug.Log(actualCarPossibleChecks[i].row);
            Debug.Log(actualCarPossibleChecks[i].column);
        }
        Debug.Log("checkpoint attuale");
        Debug.Log(checkpoint.row);
        Debug.Log(checkpoint.column);
        Debug.Log("last checkpoint");
        Debug.Log(lastCheckpointCar.row);
        Debug.Log(lastCheckpointCar.column);*/



        //preleviamo valori check
        CheckpointsListHandler checkRaggiunto = new CheckpointsListHandler();
        checkRaggiunto.initObj((int)checkpoint.row, (int)checkpoint.column);
        bool isCheckTrovato = false;
        bool isTempTrovato = false;
        //caso base = -1 -1 (inizio simulazione, qualsiasi check va bene)
        //CheckpointsListHandler temp = new CheckpointsListHandler();
        //temp.initObj(-1, -1);
        if (actualCarPossibleChecks[0].row == -1)
        {
            //penso si debba comunque assegnare il reward TODO
            // Debug.Log("caso base trovato: temp!!");
            isTempTrovato = true;
            isCheckTrovato = true;
        }
        else
        {

            //controllare se il checkpoint attuale si trova nell'elenco (agli incroci avremo una lista)
            for (int i = 0; i < actualCarPossibleChecks.Count; i++)
            {
                if ((actualCarPossibleChecks[i].row == checkRaggiunto.row) && (actualCarPossibleChecks[i].column == checkRaggiunto.column))
                {
                    //checkpoint trovato!
                    // carTransform.GetComponent<CarAgent>().AddReward(100f);
                    // carTransform.GetComponent<CarAgent>().UpdateCheckpointTime();
                    Debug.Log("[Checkpoint OK!] +100");
                    isCheckTrovato = true;
                    continue;
                }
            }
            if (!isCheckTrovato)
            {
                // carTransform.GetComponent<CarAgent>().AddReward(-10f);
                // carTransform.GetComponent<CarAgent>().UpdateCheckpointTime();

                Debug.Log("[Checkpoint Wrong] -10");
            }
        }

        //aggiungere i prossimi checkpoint giusti solo se è stato superato il check precedente giusto
        if (isCheckTrovato)
        {
            //rimuovi i check precedenti
            actualCarPossibleChecks.Clear();


            if (isTempTrovato)
            {
                // Debug.Log("Primo checkpoint superato");
                // fare un if dove se la riga o colonna equivale ad una delle corsie destre allora ci mettiamo quello dopo, se no verso sinistra, mettere anche le intersez.
                if (checkpoint.row == 2 || checkpoint.row == 8 || checkpoint.row == 14)
                {
                    //+1 colonna e incroci
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)checkpoint.row, (int)checkpoint.column + 1);
                    actualCarPossibleChecks.Add(temp2);
                    CheckpointsListHandler temp3 = new CheckpointsListHandler();
                    temp3.initObj((int)checkpoint.row - 2, (int)checkpoint.column + 2);
                    actualCarPossibleChecks.Add(temp3);
                    CheckpointsListHandler temp4 = new CheckpointsListHandler();
                    temp4.initObj((int)checkpoint.row + 0, (int)checkpoint.column + 3);
                    actualCarPossibleChecks.Add(temp4);
                    CheckpointsListHandler temp5 = new CheckpointsListHandler();
                    temp5.initObj((int)checkpoint.row + 1, (int)checkpoint.column + 1);
                    actualCarPossibleChecks.Add(temp5);
                }
                else if (checkpoint.row == 1 || checkpoint.row == 7 || checkpoint.row == 13)
                {
                    //-1 colonna e incroci
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)checkpoint.row, (int)checkpoint.column - 1);
                    actualCarPossibleChecks.Add(temp2);
                    CheckpointsListHandler temp3 = new CheckpointsListHandler();
                    temp3.initObj((int)checkpoint.row + 2, (int)checkpoint.column - 2);
                    actualCarPossibleChecks.Add(temp3);
                    CheckpointsListHandler temp4 = new CheckpointsListHandler();
                    temp4.initObj((int)checkpoint.row + 0, (int)checkpoint.column - 3);
                    actualCarPossibleChecks.Add(temp4);
                    CheckpointsListHandler temp5 = new CheckpointsListHandler();
                    temp5.initObj((int)checkpoint.row - 1, (int)checkpoint.column - 1);
                    actualCarPossibleChecks.Add(temp5);
                }
                else if (checkpoint.column == 2 || checkpoint.column == 8 || checkpoint.column == 14 || checkpoint.column == 20 || checkpoint.column == 26)
                {
                    //-1 riga e incroci
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)checkpoint.row - 1, (int)checkpoint.column);
                    actualCarPossibleChecks.Add(temp2);
                    CheckpointsListHandler temp3 = new CheckpointsListHandler();
                    temp3.initObj((int)checkpoint.row - 2, (int)checkpoint.column - 2);
                    actualCarPossibleChecks.Add(temp3);
                    CheckpointsListHandler temp4 = new CheckpointsListHandler();
                    temp4.initObj((int)checkpoint.row - 3, (int)checkpoint.column + 0);
                    actualCarPossibleChecks.Add(temp4);
                    CheckpointsListHandler temp5 = new CheckpointsListHandler();
                    temp5.initObj((int)checkpoint.row - 1, (int)checkpoint.column + 1);
                    actualCarPossibleChecks.Add(temp5);
                }
                else
                {
                    //+1 riga e incroci
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)checkpoint.row + 1, (int)checkpoint.column);
                    actualCarPossibleChecks.Add(temp2);
                    CheckpointsListHandler temp3 = new CheckpointsListHandler();
                    temp3.initObj((int)checkpoint.row + 2, (int)checkpoint.column + 2);
                    actualCarPossibleChecks.Add(temp3);
                    CheckpointsListHandler temp4 = new CheckpointsListHandler();
                    temp4.initObj((int)checkpoint.row + 3, (int)checkpoint.column + 0);
                    actualCarPossibleChecks.Add(temp4);
                    CheckpointsListHandler temp5 = new CheckpointsListHandler();
                    temp5.initObj((int)checkpoint.row - 1, (int)checkpoint.column - 1);
                    actualCarPossibleChecks.Add(temp5);
                }

                lastCheckpointCar.row = checkpoint.row;
                lastCheckpointCar.column = checkpoint.column;

            }
            else if (((int)checkpoint.row + 1 <= 14) && (checkpoint.column == lastCheckpointCar.column) && (checkpoint.row == (lastCheckpointCar.row + 1)))
            { //attuale = precedente con colonna uguale e +1 riga (stiamo scendendo sulla verticale, corsia sinistra)
                if (mappaCheckpoints[(int)checkpoint.row + 1, (int)checkpoint.column] != null)
                {
                    // Debug.Log("esiste un check successivo; non è un incrocio verticale sx");
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)mappaCheckpoints[(int)checkpoint.row + 1, (int)checkpoint.column].row, (int)mappaCheckpoints[(int)checkpoint.row + 1, (int)checkpoint.column].column);
                    actualCarPossibleChecks.Add(temp2);
                }
                else
                {
                    // Debug.Log("ci troviamo ad un incrocio verticale sx");
                    //ci troviamo ad un incrocio: inserire le 3 possibili zone in cui controllare, poi iterarci e controllare se si può (o se c'è un muro)
                    CheckpointsListHandler[] zoneInCuiControllare = new CheckpointsListHandler[3];
                    zoneInCuiControllare[0] = new CheckpointsListHandler();
                    zoneInCuiControllare[0].initObj(3, 0);
                    zoneInCuiControllare[1] = new CheckpointsListHandler();
                    zoneInCuiControllare[1].initObj(1, -1);
                    zoneInCuiControllare[2] = new CheckpointsListHandler();
                    zoneInCuiControllare[2].initObj(2, 2);
                    foreach (CheckpointsListHandler z in zoneInCuiControllare)
                    {
                        int xTemp = (int)checkpoint.row + z.row;
                        int yTemp = (int)checkpoint.column + z.column;
                        if ((xTemp > 0) && (xTemp <= 14) && (yTemp > 0) && (yTemp <= 26))
                        {
                            CheckpointsListHandler temp2 = new CheckpointsListHandler();
                            temp2.initObj((int)checkpoint.row + z.row, (int)checkpoint.column + z.column);
                            actualCarPossibleChecks.Add(temp2);
                        }
                        else
                        {
                            Debug.Log("fuori range checkpoints");
                        }
                    }
                }
            }
            else if (((int)checkpoint.row - 1 > 0) && (checkpoint.column == lastCheckpointCar.column) && (checkpoint.row == (lastCheckpointCar.row - 1)))
            { //attuale = precedente con colonna uguale e -1 riga (stiamo salendo sulla verticale, corsia destra)
                if (mappaCheckpoints[(int)checkpoint.row - 1, (int)checkpoint.column] != null)
                {
                    // Debug.Log("esiste un check successivo; non è un incrocio verticale dx");
                    //esiste un check successivo; non è un incrocio
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)mappaCheckpoints[(int)checkpoint.row - 1, (int)checkpoint.column].row, (int)mappaCheckpoints[(int)checkpoint.row - 1, (int)checkpoint.column].column);
                    actualCarPossibleChecks.Add(temp2);
                }
                else
                {
                    // Debug.Log("ci troviamo ad un incrocio verticale dx");
                    //ci troviamo ad un incrocio: inserire le 3 possibili zone in cui controllare, poi iterarci e controllare se si può (o se c'è un muro)
                    CheckpointsListHandler[] zoneInCuiControllare = new CheckpointsListHandler[3];
                    zoneInCuiControllare[0] = new CheckpointsListHandler();
                    zoneInCuiControllare[0].initObj(-2, -2);
                    zoneInCuiControllare[1] = new CheckpointsListHandler();
                    zoneInCuiControllare[1].initObj(-3, 0);
                    zoneInCuiControllare[2] = new CheckpointsListHandler();
                    zoneInCuiControllare[2].initObj(-1, 1);
                    foreach (CheckpointsListHandler z in zoneInCuiControllare)
                    {
                        int xTemp = (int)checkpoint.row + z.row;
                        int yTemp = (int)checkpoint.column + z.column;
                        if ((xTemp > 0) && (xTemp <= 14) && (yTemp > 0) && (yTemp <= 26))
                        {
                            CheckpointsListHandler temp2 = new CheckpointsListHandler();
                            temp2.initObj((int)checkpoint.row + z.row, (int)checkpoint.column + z.column);
                            actualCarPossibleChecks.Add(temp2);
                        }
                        else
                        {
                            Debug.Log("fuori range checkpoints");
                        }
                    }
                }
            }
            else if (((int)checkpoint.column - 1 > 0) && (checkpoint.row == lastCheckpointCar.row) && (checkpoint.column == (lastCheckpointCar.column - 1)))
            { //attuale = precedente con riga uguale e -1 colonna (stiamo andando a sinistra sull'orizzontale)
                if (mappaCheckpoints[(int)checkpoint.row, (int)checkpoint.column - 1] != null)
                {
                    // Debug.Log("esiste un check successivo; non è un incrocio orizzontale sx");
                    //esiste un check successivo; non è un incrocio
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)mappaCheckpoints[(int)checkpoint.row, (int)checkpoint.column - 1].row, (int)mappaCheckpoints[(int)checkpoint.row, (int)checkpoint.column - 1].column);
                    actualCarPossibleChecks.Add(temp2);
                }
                else
                {
                    // Debug.Log("ci troviamo ad un incrocio orizzontale sx");
                    //ci troviamo ad un incrocio: inserire le 3 possibili zone in cui controllare, poi iterarci e controllare se si può (o se c'è un muro)
                    CheckpointsListHandler[] zoneInCuiControllare = new CheckpointsListHandler[3];
                    zoneInCuiControllare[0] = new CheckpointsListHandler();
                    zoneInCuiControllare[0].initObj(+2, -2);
                    zoneInCuiControllare[1] = new CheckpointsListHandler();
                    zoneInCuiControllare[1].initObj(0, -3);
                    zoneInCuiControllare[2] = new CheckpointsListHandler();
                    zoneInCuiControllare[2].initObj(-1, -1);
                    foreach (CheckpointsListHandler z in zoneInCuiControllare)
                    {
                        int xTemp = (int)checkpoint.row + z.row;
                        int yTemp = (int)checkpoint.column + z.column;
                        if ((xTemp > 0) && (xTemp <= 14) && (yTemp > 0) && (yTemp <= 26))
                        {
                            CheckpointsListHandler temp2 = new CheckpointsListHandler();
                            temp2.initObj((int)checkpoint.row + z.row, (int)checkpoint.column + z.column);
                            actualCarPossibleChecks.Add(temp2);
                        }
                        else
                        {
                            Debug.Log("fuori range checkpoints");
                        }
                    }
                }
            }
            else if (((int)checkpoint.column + 1 <= 26) && (checkpoint.row == lastCheckpointCar.row) && (checkpoint.column == (lastCheckpointCar.column + 1)))
            { //attuale = precedente con riga uguale e +1 colonna (stiamo andando a destra sull'orizzontale)
                if (mappaCheckpoints[(int)checkpoint.row, (int)checkpoint.column + 1] != null)
                {
                    // Debug.Log("esiste un check successivo; non è un incrocio orizzontale dx");
                    //esiste un check successivo; non è un incrocio
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    temp2.initObj((int)mappaCheckpoints[(int)checkpoint.row, (int)checkpoint.column + 1].row, (int)mappaCheckpoints[(int)checkpoint.row, (int)checkpoint.column + 1].column);
                    actualCarPossibleChecks.Add(temp2);
                }
                else
                {
                    // Debug.Log("ci troviamo ad un incrocio orizzontale dx");
                    //ci troviamo ad un incrocio: inserire le 3 possibili zone in cui controllare, poi iterarci e controllare se si può (o se c'è un muro)
                    CheckpointsListHandler[] zoneInCuiControllare = new CheckpointsListHandler[3];
                    zoneInCuiControllare[0] = new CheckpointsListHandler();
                    zoneInCuiControllare[0].initObj(-2, +2);
                    zoneInCuiControllare[1] = new CheckpointsListHandler();
                    zoneInCuiControllare[1].initObj(0, +3);
                    zoneInCuiControllare[2] = new CheckpointsListHandler();
                    zoneInCuiControllare[2].initObj(+1, +1);
                    foreach (CheckpointsListHandler z in zoneInCuiControllare)
                    {
                        int xTemp = (int)checkpoint.row + z.row;
                        int yTemp = (int)checkpoint.column + z.column;
                        if ((xTemp > 0) && (xTemp <= 14) && (yTemp > 0) && (yTemp <= 26))
                        {
                            CheckpointsListHandler temp2 = new CheckpointsListHandler();
                            temp2.initObj((int)checkpoint.row + z.row, (int)checkpoint.column + z.column);
                            actualCarPossibleChecks.Add(temp2);
                        }
                        else
                        {
                            Debug.Log("fuori range checkpoints");
                        }
                    }
                }
            }
            else
            {
                // Debug.Log("Eravamo ad un incrocio");
                //nessuno dei casi precedenti = eravamo ad un incrocio, capire come scegliere il prossimo check
                CheckpointsListHandler[] zoneInCuiControllare = new CheckpointsListHandler[12];
                //caso +1,0, farà uscire i / 3 = 0
                zoneInCuiControllare[0] = new CheckpointsListHandler();
                zoneInCuiControllare[0].initObj(3, 0);
                zoneInCuiControllare[1] = new CheckpointsListHandler();
                zoneInCuiControllare[1].initObj(1, 1);
                zoneInCuiControllare[2] = new CheckpointsListHandler();
                zoneInCuiControllare[2].initObj(2, -2);
                //caso 0,+1, farà uscire i / 3 = 1
                zoneInCuiControllare[3] = new CheckpointsListHandler();
                zoneInCuiControllare[3].initObj(2, 2);
                zoneInCuiControllare[4] = new CheckpointsListHandler();
                zoneInCuiControllare[4].initObj(-1, 1);
                zoneInCuiControllare[5] = new CheckpointsListHandler();
                zoneInCuiControllare[5].initObj(0, +3);
                //caso -1,0, farà uscire i / 3 = 2
                zoneInCuiControllare[6] = new CheckpointsListHandler();
                zoneInCuiControllare[6].initObj(-2, 2);
                zoneInCuiControllare[7] = new CheckpointsListHandler();
                zoneInCuiControllare[7].initObj(-3, 0);
                zoneInCuiControllare[8] = new CheckpointsListHandler();
                zoneInCuiControllare[8].initObj(-1, -1);
                //caso 0,-1, farà uscire i / 3 = 3
                zoneInCuiControllare[9] = new CheckpointsListHandler();
                zoneInCuiControllare[9].initObj(1, -1);
                zoneInCuiControllare[10] = new CheckpointsListHandler();
                zoneInCuiControllare[10].initObj(-2, -2);
                zoneInCuiControllare[11] = new CheckpointsListHandler();
                zoneInCuiControllare[11].initObj(0, -3);

                int i = 0;
                int trovatoCheckIncrocio = -1;
                foreach (CheckpointsListHandler z in zoneInCuiControllare)
                {

                    int xTemp = (int)lastCheckpointCar.row + z.row;
                    int yTemp = (int)lastCheckpointCar.column + z.column;
                    if ((checkpoint.row == xTemp) && (yTemp == checkpoint.column))
                    {
                        // Debug.Log("trovato inters.");
                        // Debug.Log(i / 3);
                        trovatoCheckIncrocio = i / 3;
                        continue;
                    }
                    i++;
                }

                if (trovatoCheckIncrocio != -1)
                {
                    CheckpointsListHandler temp2 = new CheckpointsListHandler();
                    switch (trovatoCheckIncrocio)
                    {
                        case 0:
                            temp2.initObj((int)checkpoint.row + 1, (int)checkpoint.column);
                            actualCarPossibleChecks.Add(temp2);
                            break;
                        case 1:
                            temp2.initObj((int)checkpoint.row, (int)checkpoint.column + 1);
                            actualCarPossibleChecks.Add(temp2);
                            break;
                        case 2:
                            temp2.initObj((int)checkpoint.row - 1, (int)checkpoint.column);
                            actualCarPossibleChecks.Add(temp2);
                            break;
                        case 3:
                            temp2.initObj((int)checkpoint.row, (int)checkpoint.column - 1);
                            actualCarPossibleChecks.Add(temp2);
                            break;
                    }
                }
                else
                {
                    Debug.Log("Impossibile trovare incrocio");
                }



            }


        }

        //assegna check precedente
        lastCheckpointCar.row = checkpoint.row;
        lastCheckpointCar.column = checkpoint.column;
    }
}
