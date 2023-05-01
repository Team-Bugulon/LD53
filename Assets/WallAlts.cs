using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallAlts : MonoBehaviour
{
    [SerializeField] string wallSpritesheetPath;
    [SerializeField] string floorSpritesheetPath;
    // Start is called before the first frame update
    void Start()
    {
        if (wallSpritesheetPath != null && wallSpritesheetPath != "")
        {
            var objects = Resources.LoadAll<Sprite>(wallSpritesheetPath);

            var wallAlts = objects.Where(q => q is Sprite).Cast<Sprite>().ToList();
            //order them by name
            wallAlts = wallAlts.OrderBy(q => q.name).ToList();

            if (Random.Range(0, 5) > 0)
            {
                GetComponentsInChildren<SpriteRenderer>()[1].sprite = wallAlts[1];
            }
            else
            {
                if (wallSpritesheetPath == "Assets/WallAlts/gloups")
                {
                    GetComponentsInChildren<SpriteRenderer>()[1].sprite = wallAlts[Random.Range(2, 7)];
                } else
                {
                    GetComponentsInChildren<SpriteRenderer>()[1].sprite = wallAlts[Random.Range(2, 5)];
                }

            }
        }

        if (floorSpritesheetPath != null && floorSpritesheetPath != "")
        {
            var objects = Resources.LoadAll<Sprite>(floorSpritesheetPath);
            var floorAlts = objects.Where(q => q is Sprite).Cast<Sprite>().ToList();
            //order them by name
            floorAlts = floorAlts.OrderBy(q => q.name).ToList();

            if (Random.Range(0, 5) > 0)
            {
                GetComponentsInChildren<SpriteRenderer>()[0].sprite = floorAlts[0];
            }
            else
            {
                GetComponentsInChildren<SpriteRenderer>()[0].sprite = floorAlts[Random.Range(1, floorAlts.Count)];
            }
        }
        
        Destroy(this);
    }
}
