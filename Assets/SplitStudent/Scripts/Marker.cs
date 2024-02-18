using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public int row;
    public int column;
    public int group;
    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return column;
    }

    public int GetGroup()
    {
        return group;
    }
}
