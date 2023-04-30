using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager _i;

    public static GameManager i
    {
        get
        {
            return _i;
        }
    }

    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("References")]
    public Player player;

    [Header("Prefabs")]
    [SerializeField] Plate platePrefab;
    
    [Header("Libs")]
    public List<Sprite> dishSprites;
    public List<Sprite> sludgeSprites;
    public List<RuntimeAnimatorController> customersRat;

    int wave = 0;
    List<int> availableDish;

    // Start is called before the first frame update
    void Start()
    {
        var objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Gameplay/Dish/dish.png");
        dishSprites = objects.Where(q => q is Sprite).Cast<Sprite>().ToList();

        var objects2 = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Gameplay/sludge.png");
        sludgeSprites = objects2.Where(q => q is Sprite).Cast<Sprite>().ToList();

        UpdateWave();

        availableDish = new List<int>();
        for (int i = 0; i < dishSprites.Count; i++)
        {
            availableDish.Add(i);
        }
        
        //shuffle
        for (int i = 0; i < availableDish.Count; i++)
        {
            var temp = availableDish[i];
            int randomIndex = Random.Range(i, availableDish.Count);
            availableDish[i] = availableDish[randomIndex];
            availableDish[randomIndex] = temp;
        }
        //availableDish.Sort((a, b) => 1 - 2 * Random.Range(0, 1));
    }

    public int GetRandomDish()
    {
        //pop value at index zero
        int dishType = availableDish[0];
        availableDish.RemoveAt(0);
        if (availableDish.Count <= 0)
        {
            availableDish = new List<int>();
            for (int i = 0; i < dishSprites.Count; i++)
            {
                availableDish.Add(i);
            }
            //shuffle
            for (int i = 0; i < availableDish.Count; i++)
            {
                var temp = availableDish[i];
                int randomIndex = Random.Range(i, availableDish.Count);
                availableDish[i] = availableDish[randomIndex];
                availableDish[randomIndex] = temp;
            }
        }

        return dishType;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateWave()
    {
        wave++;
        Customer[] customers = GameObject.FindObjectsOfType<Customer>();
        foreach (var customer in customers)
        {
            customer.waveUpdate(wave);
        }
    }

    public void SpawnPlate(int dishType)
    {
        var plate = Instantiate(platePrefab);
        plate.SetDish(dishType);
    }

    public void BreakPlate(int dishType)
    {
        SpawnPlate(dishType);
    }
}
