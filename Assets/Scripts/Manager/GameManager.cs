using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

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

    [InspectorButton("GenerateWorld")]
    public bool generate;
    public int level;
    
    [Header("References")]
    public Player player;
    public World world;

    [Header("Prefabs")]
    [SerializeField] Plate platePrefab;
    
    [Header("Libs")]
    public List<Sprite> dishSprites;
    public List<Sprite> sludgeSprites;
    public List<Sprite> puddleSprites;
    public List<Color> puddleColors;
    public List<RuntimeAnimatorController> customersRat;

    public float timer = 0;
    public bool win = false;
    int wave = 0;
    List<int> availableDish;
    Counter counter;
    public int score = 0;

    public Dictionary<int, List<Customer>> customers;
    public int CustomerTotalQty = 0;
    public int CustomerTotalSatisfied = 0;

    private void GenerateWorld()
    {
        CustomerTotalQty = 0;
        CustomerTotalSatisfied = 0;
        customers = new Dictionary<int, List<Customer>>();
        if (level < 0) level = SaveManager.i.level;
        world.Generate(level);
        player = GameObject.FindObjectOfType<Player>();
        counter = GameObject.FindObjectOfType<Counter>();
        timer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        var objects = Resources.LoadAll<Sprite>("Assets/dish");
        dishSprites = objects.Where(q => q is Sprite).Cast<Sprite>().ToList();

        var objects2 = Resources.LoadAll<Sprite>("Assets/sludge");
        sludgeSprites = objects2.Where(q => q is Sprite).Cast<Sprite>().ToList();

        var objects3 = Resources.LoadAll<Sprite>("Assets/puddle");
        puddleSprites = objects3.Where(q => q is Sprite).Cast<Sprite>().ToList();

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

        GenerateWorld();
        UpdateWave();
        UIManager.i.UpdateCustomerBubble(0, CustomerTotalQty, false);
        TransitionManager.i.TransiOut(1, player.transform.position.x, player.transform.position.y);
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

    private void Update()
    {
        if (!player.inKitchen && !win)
        {
            timer += Time.deltaTime;
            UIManager.i.UpdateTime(timer);
        }
    }

    void UpdateWave()
    {
        wave++;
        if (CustomerTotalQty == CustomerTotalSatisfied)
        {
            Win();
        } else
        {
            foreach (var customer in customers[wave])
            {
                customer.waveUpdate(wave);
            }
        }
    }

    void Win()
    {
        if (!win)
        {
            win = true;
            if (timer <= world.time)
            {
                score = 3;
            } else if (timer - 10 <= world.time)
            {
                score = 2;
            } else
            {
                score = 1;
            }
            SaveManager.i.NewScore(level, timer);
            Debug.Log("GAME WIN !");
            StartCoroutine(WinCoroutine());
        }

    }
    
    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.i.LevelWinScreen();
    }

    public void SpawnPlate(int dishType)
    {
        var plate = Instantiate(platePrefab);
        plate.transform.parent = counter.GetFreeSpot();
        plate.transform.localPosition = Vector3.zero;
        plate.SetDish(dishType);
    }

    public void BreakPlate(int dishType)
    {
        SpawnPlate(dishType);
    }

    public void CustomerSatisfied()
    {
        GameManager.i.CustomerTotalSatisfied++;

        UIManager.i.UpdateCustomerBubble(CustomerTotalSatisfied, CustomerTotalQty);
            
        bool everySatisfied = true;
        foreach (var customer in customers[wave])
        {
            if (customer.state != CustomerState.Satisfied)
            {
                everySatisfied = false;
                break;
            }
        }
        if (everySatisfied) UpdateWave();
    }
}
