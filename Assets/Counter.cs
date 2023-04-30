using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public List<Transform> plateAnchors;

    public Transform GetFreeSpot()
    {
        int platesQty = GetComponentsInChildren<Plate>().Length;
        return plateAnchors[Mathf.Min(platesQty, plateAnchors.Count-1)];
    }

}
