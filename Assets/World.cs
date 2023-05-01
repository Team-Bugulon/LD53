using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Newtonsoft.Json;
using System;

public class CounterEntity : Entity { };

public class DoorEntity : Entity { };
public class ChairEntity : Entity { public CustomFieldsChair customFields; };
public class CustomFieldsChair
{
    public int wave = 0;
}

public class WalkerEntity : Entity { public CustomFieldsWalker customFields; };
public class CustomFieldsWalker
{
    public List<Point> path;
}

public class Point
{
    public int cx = 0;
    public int cy = 0;
}

public class TableEntity : Entity { };

public class PuddleEntity : Entity { };


public class Entity
{
    public string id = null;
    public int x = 0;
    public int y = 0;
}

public class EntitiesContainer
{
    public List<CounterEntity> counter;
    public List<ChairEntity> chair;
    public List<TableEntity> table;
    public List<DoorEntity> door;
    public List<PuddleEntity> puddle;
    public List<WalkerEntity> walker;
}

public class CustomFields
{
    public int time = 0;
    public int skin = 0;
    public string name = null;
}

public class LevelsJSON
{
    public int width = 0;
    public int height = 0;
    public CustomFields customFields;
    public EntitiesContainer entities;
    public static LevelsJSON CreateFromJSON(string jsonString)
    {
        string targetFile = Resources.Load<TextAsset>(jsonString).ToString();

        LevelsJSON lvl = JsonConvert.DeserializeObject<LevelsJSON>(targetFile);

        return lvl;
    }
}

public class World : MonoBehaviour
{
    //[HideInInspector] public int levelID;
    //[HideInInspector] public int skin;
    //[HideInInspector] public int time;
    //[HideInInspector] public string levelName;

    public int levelID;
    public int skin;
    public float time;
    public string levelName;

    [Header("References")]
    public Tilemap tilemap;
    public Transform entitiesContainer;

    [Header("Prefabs")]
    public Chair chair;
    public Counter counter;
    public Table table;
    public KitchenArea kitchenArea;
    public Door door;
    public Puddle puddle;
    public Walker walker;
    public Walker puffer;
    public List<TileBase> tiles;

    public void Generate(int worldID)
    {
        levelID = worldID;
        string jsonPath = "LDtk/Levels/simplified/level_" + worldID.ToString() + "/data";
        string csvPath = "LDtk/Levels/simplified/level_" + worldID.ToString() + "/intgrid";

        LevelsJSON level = LevelsJSON.CreateFromJSON(jsonPath);

        skin = level.customFields.skin;
        time = level.customFields.time/10f;
        levelName = level.customFields.name;

        //place tiles
        //clean tiles
        tilemap.ClearAllTiles();
        List<int> intGrid = GetIntGrid(csvPath);

        Vector2Int mapSize = new Vector2Int(level.width / 32, level.height / 32);

        Vector2Int minKitchen = Vector2Int.one * 100000;
        Vector2Int maxKitchen = Vector2Int.zero;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                int index = x + y * mapSize.x;
                int tileID = intGrid[index];

                if (tileID == 2)
                {
                    if (x < minKitchen.x) minKitchen.x = x;
                    if (y < minKitchen.y) minKitchen.y = y;
                    if (x > maxKitchen.x) maxKitchen.x = x;
                    if (y > maxKitchen.y) maxKitchen.y = y;
                }

                if (tileID != 0)
                {
                    TileBase tile = tiles[tileID - 1];
                    if (tileID - 1 == 0 && skin == 1)
                    {
                        tile = tiles[tileID - 1 + 2];
                    }
                    tilemap.SetTile(new Vector3Int(x, mapSize.y - y - 1, 0), tile);
                }
            }
        }

        //place entities
        //clean entitiesContainer
        foreach (Transform child in entitiesContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        //kitchen area
        KitchenArea ka = Instantiate(kitchenArea);
        ka.transform.parent = entitiesContainer;
        ka.transform.localScale = (Vector2)(maxKitchen - minKitchen) + Vector2.one;
        var kaPos = new Vector2(
             (minKitchen.x + maxKitchen.x + 1) / 2f,
             (level.height / 32f) - ((minKitchen.y + maxKitchen.y + 1) / 2f)
            );
        ka.transform.position = kaPos;

        if (level.entities.counter != null)
        {
            Counter c = Instantiate(counter);
            c.transform.parent = entitiesContainer;
            Vector2 offset = new Vector2(3, 1);
            c.transform.position = new Vector2(level.entities.counter[0].x / 32f + offset.x, (level.height - level.entities.counter[0].y) / 32f - offset.y);
        }

        if (level.entities.table != null)
        {
            foreach (var ent in level.entities.table)
            {
                Table c = Instantiate(table);
                c.transform.parent = entitiesContainer;
                Vector2 offset = new Vector2(1, 1);
                c.transform.position = new Vector2(ent.x/32f + offset.x, (level.height - ent.y)/32f - offset.y);
            }
        }

        if (level.entities.chair != null)
        {
            foreach(var ent in level.entities.chair)
            {
                Chair c = Instantiate(chair);
                c.transform.parent = entitiesContainer;
                c.wave = ent.customFields.wave;
                Vector2 offset = new Vector2(.5f, .5f);
                c.transform.position = new Vector2(ent.x / 32f + offset.x, (level.height - ent.y) / 32f - offset.y);
                c.Init();
            }
        }

        if (level.entities.door != null)
        {
            foreach (var ent in level.entities.door)
            {
                Door c = Instantiate(door);
                c.transform.parent = entitiesContainer;
                Vector2 offset = new Vector2(1.5f, .875f);
                c.transform.position = new Vector2(ent.x / 32f + offset.x, (level.height - ent.y) / 32f - offset.y);
            }
        }

        if (level.entities.puddle != null)
        {
            foreach (var ent in level.entities.puddle)
            {
                Puddle c = Instantiate(puddle);
                c.transform.parent = entitiesContainer;
                Vector2 offset = new Vector2(1, 1);
                c.transform.position = new Vector2(ent.x / 32f + offset.x, (level.height - ent.y) / 32f - offset.y);
            }
        }

        if (level.entities.walker != null)
        {
            foreach (var ent in level.entities.walker)
            {
                Walker c;
                if (skin == 0)
                {
                    c = Instantiate(walker);
                } else
                {
                    c = Instantiate(puffer);
                }
                c.transform.parent = entitiesContainer;
                Vector2 offset = new Vector2(1, 1);
                c.transform.position = new Vector2(ent.x / 32f + offset.x, (level.height - ent.y) / 32f - offset.y);

                c.path = new List<Vector2>();
                c.path.Add(c.transform.position);
                foreach (var p in ent.customFields.path)
                {
                    c.path.Add(new Vector2(p.cx + offset.x, (level.height / 32f - p.cy)  - offset.y));
                }
            }
        }


    }

    List<int> GetIntGrid(string path)
    {
        string fileData = Resources.Load<TextAsset>(path).ToString();
        fileData = fileData.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        var lines = fileData.Split(',');
        List<int> retList = new List<int>();
        foreach (var line in lines)
        {
            retList.Add(int.Parse(line));
        }
        return retList;
    }

}
