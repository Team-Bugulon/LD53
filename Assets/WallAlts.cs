using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallAlts : MonoBehaviour
{
    [SerializeField] string wallSpritesheetPath;
    // Start is called before the first frame update
    void Start()
    {
        var objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(wallSpritesheetPath);
        var wallAlts = objects.Where(q => q is Sprite).Cast<Sprite>().ToList();
        //order them by name
        wallAlts = wallAlts.OrderBy(q => q.name).ToList();

        if (Random.Range(0,5) > 0){
            GetComponentsInChildren<SpriteRenderer>()[1].sprite = wallAlts[1];
        } else
        {
            GetComponentsInChildren<SpriteRenderer>()[1].sprite = wallAlts[Random.Range(2,5)];
        }
    }
}
