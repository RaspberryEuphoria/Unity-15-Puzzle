using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Borders
{
    public BoxCollider top;
    public BoxCollider right;
    public BoxCollider left;
    public BoxCollider bottom;
}

public class BordersColliders : MonoBehaviour
{
    public Borders borders;
}
