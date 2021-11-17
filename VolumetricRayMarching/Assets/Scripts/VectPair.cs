using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectPair
{
    public Vector3 pos;
    public Vector3 color;
    public VectPair()
    {
        pos = new Vector3(0, 0, 0);
        color = new Vector3(0, 0, 0);

    }
    public VectPair(Vector3 startpos, Vector3 startcolor)
    {
        pos = startpos;
        color = startcolor;
    }
}
