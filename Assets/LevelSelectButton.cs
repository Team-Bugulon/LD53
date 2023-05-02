using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshPro text;
    [SerializeField] GameObject on;
    [SerializeField] GameObject off;
    [SerializeField] SpriteRenderer stars;
    [SerializeField] List<Sprite> starsSprite;

    public int levelID;
    public int score;
    public int skin;
    public string levelName;
    public float time;
    public float goalTime;
    bool unlocked = false;

    // Start is called before the first frame update
    void Start()
    {
        string jsonPath = "LDtk/Levels/simplified/level_" + levelID.ToString() + "/data";

        LevelsJSON level = LevelsJSON.CreateFromJSON(jsonPath);

        goalTime = level.customFields.time / 10f;
        levelName = level.customFields.name;
        skin = level.customFields.skin;

        time = SaveManager.i.data.times[levelID];
        
        if (time == 0)
        {
            score = 0;
        }
        else if (time <= goalTime)
        {
            score = 3;
        }
        else if (time - 10 <= goalTime)
        {
            score = 2;
        }
        else if (time > 0)
        {
            score = 1;
        }

        unlocked = SaveManager.i.IsLevelUnlocked(levelID);

        if (unlocked)
        {
            on.SetActive(true);
            off.SetActive(false);
        } else
        {
            on.SetActive(false);
            off.SetActive(true);
        }

        text.text = "<size=.5>"+(levelID+1).ToString();
        stars.sprite = starsSprite[score];

        CustomButton cb = GetComponentInChildren<CustomButton>();
        if (cb != null) cb.levelID = levelID;
    }

    public void OnHover()
    {
        Titlescreen.i.RefreshPaper(this);
    }
}
