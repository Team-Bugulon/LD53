using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayer : MonoBehaviour
{
    [SerializeField] string sortingLayerName = "UI";
    [SerializeField] int sortingOrder = 0;

    // Start is called before the first frame update
    void Start()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.sortingLayerName = sortingLayerName;
        mr.sortingOrder = sortingOrder;
    }
}
