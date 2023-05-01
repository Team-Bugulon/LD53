using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CustomerState
{
    Idle,
    Ordering,
    Waiting,
    Satisfied
}

public enum CustomerType
{
    Rat
}

public class Customer : MonoBehaviour
{
    public Table assignedTable;
    public Transform assignedPlateContainer;
    [SerializeField] Speech speech;
    public int wave = 1;
    public CustomerState state = CustomerState.Idle;
    public int dishType;

    CustomerType type = CustomerType.Rat;
    int customerSkin = 0;

    // Start is called before the first frame update
    public void Init()
    {
        if (!GameManager.i.customers.ContainsKey(wave))
        {
            GameManager.i.customers.Add(wave, new List<Customer>());
        }
        GameManager.i.customers[wave].Add(this);
        GameManager.i.CustomerTotalQty++;

        if (GameManager.i.world.skin == 0)
        {
            customerSkin = Random.Range(0, GameManager.i.customersRat.Count);
            GetComponent<Animator>().runtimeAnimatorController = GameManager.i.customersRat[customerSkin];
        } else
        {
            customerSkin = Random.Range(0, GameManager.i.customersFish.Count);
            GetComponent<Animator>().runtimeAnimatorController = GameManager.i.customersFish[customerSkin];
        }

        GetComponent<Animator>().Play("idle");


        Table[] tables = GameObject.FindObjectsOfType<Table>();
        //calculate distance to each table
        float[] distances = new float[tables.Length];
        for (int i = 0; i < tables.Length; i++)
        {
            distances[i] = Vector2.Distance(transform.position, tables[i].transform.position);
        }

        //find the closest table
        float minDistance = Mathf.Min(distances);
        int minIndex = System.Array.IndexOf(distances, minDistance);

        assignedTable = tables[minIndex];

        //is he on the left or right side of the table
        if (transform.position.x < assignedTable.transform.position.x)
        {
            assignedTable.leftCustomer = this;
            assignedPlateContainer = assignedTable.plateContainerL;
        }
        else
        {
            assignedTable.rightCustomer = this;
            assignedPlateContainer = assignedTable.plateContainerR;
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void waveUpdate(int wave)
    {
        if (this.wave == wave && state == CustomerState.Idle)
        {
            state = CustomerState.Ordering;
            speech.Ordering();
            assignedTable.EnableCircle();
        }
    }
    public void Deliver()
    {
        if (state == CustomerState.Ordering)
        {
            dishType = GameManager.i.GetRandomDish();
            state = CustomerState.Waiting;
            speech.SetDish(dishType);
            transform.DOComplete();
            transform.DOShakeScale(.4f, .5f, 20);
            GameManager.i.SpawnPlate(dishType);
        }
        else if (state == CustomerState.Waiting)
        {
            if (GameManager.i.player.DiscardPlate(dishType, this))
            {
                state = CustomerState.Satisfied;
                transform.DOComplete();
                transform.DOShakeScale(.4f, .5f, 20);
                speech.gameObject.SetActive(false);
                GetComponent<Animator>().Play("satisfied");
                GameManager.i.CustomerSatisfied();
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //            if (state == CustomerState.Ordering)
    //        {
    //            dishType = GameManager.i.GetRandomDish();
    //            state = CustomerState.Waiting;
    //            speech.SetDish(dishType);
    //            transform.DOComplete();
    //            transform.DOShakeScale(.4f, .5f, 20);
    //            GameManager.i.SpawnPlate(dishType);
    //        }
    //        else if (state == CustomerState.Waiting)
    //{
    //    if (GameManager.i.player.DiscardPlate(dishType, this))
    //    {
    //        state = CustomerState.Satisfied;
    //        transform.DOComplete();
    //        transform.DOShakeScale(.4f, .5f, 20);
    //        speech.gameObject.SetActive(false);
    //        GetComponent<Animator>().Play("satisfied");
    //    }
    //}
    //    }
    //}

}
