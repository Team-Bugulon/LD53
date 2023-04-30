using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Customer leftCustomer;
    public Customer rightCustomer;

    public Transform plateContainerL;
    public Transform plateContainerR;

    [SerializeField] GameObject circle;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager.i.player.atRest && !GameManager.i.player.stunned)
        {
            bool disableCircle = true;
            if (leftCustomer != null)
            {
                leftCustomer.Deliver();
                if (leftCustomer.state == CustomerState.Waiting || leftCustomer.state == CustomerState.Ordering)
                {
                    disableCircle = false;
                }
            }

            if (rightCustomer != null)
            {
                rightCustomer.Deliver();
                if (rightCustomer.state == CustomerState.Waiting || rightCustomer.state == CustomerState.Ordering)
                {
                    disableCircle = false;
                }
            }

            if (disableCircle)
            {
                circle.SetActive(false);
            }
        }
    }

    public void EnableCircle()
    {
        circle.SetActive(true);
    }
}
