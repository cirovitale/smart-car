using System.Collections.Generic;
using UnityEngine;

public class CheckpointsListHandler : MonoBehaviour
{

    public void initObj(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public int row;
    public int column;
}