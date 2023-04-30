using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public int wave = 0;
    [SerializeField] Transform customerContainer;
    [SerializeField] Customer customerPrefab;

    void Start()
    {
        if (wave != 0)
        {
            Customer customer = Instantiate(customerPrefab);
            customer.transform.parent = customerContainer;
            customer.transform.localPosition = Vector3.zero;
        }
    }
}
