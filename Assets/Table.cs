using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Customer leftCustomer;
    public Customer rightCustomer;

    public Transform plateContainerL;
    public Transform plateContainerR;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager.i.player.atRest)
        {
            if (leftCustomer != null)
            {
                leftCustomer.Deliver();
            }

            if (rightCustomer != null)
            {
                rightCustomer.Deliver();
            }
        }
    }
}
