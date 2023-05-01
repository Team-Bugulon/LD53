using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public int wave = 0;
    [SerializeField] Transform customerContainer;
    [SerializeField] Customer customerPrefab;

    public List<Sprite> sprites;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[GameManager.i.world.skin];
    }
    
    public void Init()
    {
        if (wave != 0)
        {
            Customer customer = Instantiate(customerPrefab);
            customer.transform.parent = customerContainer;
            customer.transform.localPosition = Vector3.zero;
            customer.wave = wave;
            customer.Init();
        }

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

        var assignedTable = tables[minIndex];

        //is he on the left or right side of the table
        if (transform.position.x > assignedTable.transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
