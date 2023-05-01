using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SaveManager : MonoBehaviour
{
    //Save system
    private static SaveManager _i;

    public static SaveManager i
    {
        get
        {
            if (_i != null)
            {
                return _i;
            }
            else
            {
                return _i = new GameObject("SaveManager").AddComponent<SaveManager>();
            }
        }
    }

    private void Awake()
    {
        if (_i == null && _i != this)
        {
            _i = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class Save_Data
    {
        //public int highscore;
        public List<float> times;
        float musicVolume;
        float sfxVolume;
    }

    public Save_Data data;
    public int level = 0;

    public void Save()
    {
        //data = new Save_Data();
        //data.highscore = TransitionManager.i.BestLevel;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("save", json);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            string json = PlayerPrefs.GetString("save");
            data = JsonUtility.FromJson<Save_Data>(json);
            //TransitionManager.i.BestLevel = data.highscore;
            if (data.times.Count < 20)
            {
                for (int i = data.times.Count; i < 20; i++)
                {
                    data.times.Add(0);
                }
            }
        }
        else
        {
            Debug.Log("No save data found");
            data = new Save_Data();
            data.times = new List<float>();
            for (int i = 0; i < 20; i++)
            {
                data.times.Add(0);
            }
        }
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey("save");
    }

    public int GetScore(int levelID)
    {
        string jsonPath = "LDtk/Levels/simplified/level_" + levelID.ToString() + "/data";

        LevelsJSON level = LevelsJSON.CreateFromJSON(jsonPath);

        float time = level.customFields.time / 10f;

        int score = 0;

        if (data.times[levelID] <= time)
        {
            score = 3;
        }
        else if (data.times[levelID] - 10 <= time)
        {
            score = 2;
        }
        else
        {
            score = 1;
        }

        return score;
    }

    public void NewScore(int levelID, float time)
    {
        if (data.times[levelID] == 0 || time < data.times[levelID])
        {
            data.times[levelID] = time;
            Save();
        }
    }

    public bool IsLevelUnlocked(int levelID)
    {
        if (levelID == 0)
        {
            return true;
        } else
        {
            if (data.times[levelID - 1] > 0)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
