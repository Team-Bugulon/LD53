using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float dir = collision.transform.position.y - transform.position.y;
            GetComponent<Animator>().Play("door_idle");
            if (dir > 0)
            {
                GetComponent<Animator>().Play("door_out", 0, 0f);
            }
            else
            {
                GetComponent<Animator>().Play("door_in", 0, 0f);
            }
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        float dir = collision.transform.position.y - transform.position.y;
    //        GetComponent<Animator>().Play("door_idle");
    //        if (dir > 0)
    //        {
    //            GetComponent<Animator>().Play("door_out", 0, 0f);
    //        }
    //        else
    //        {
    //            GetComponent<Animator>().Play("door_in", 0, 0f);
    //        }
    //    }
    //}
}
