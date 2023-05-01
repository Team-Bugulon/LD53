using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Drawing;

public class Titlescreen : MonoBehaviour
{
    private static Titlescreen _i;

    public static Titlescreen i
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


    public GameObject levelSelect;
    public GameObject mainMenu;
    public LevelSelectButton levelSelectButtonPrefab;
    public Transform levelSelectButtonContainer;

    public GameObject journal;
    public TMPro.TextMeshPro dayText;
    public TMPro.TextMeshPro titleText;
    public TMPro.TextMeshPro bestTimeText;
    public TMPro.TextMeshPro goalTimeText;
    public SpriteRenderer kyur;
    public SpriteRenderer stars;
    public List<Sprite> kyurs;
    public List<Sprite> starss;

    // Start is called before the first frame update
    void Start()
    {
        TransitionManager.i.TransiOut();

        //for (int i = 0; i < 14; i++)
        //{
        //    LevelSelectButton bobus = Instantiate(levelSelectButtonPrefab);
        //    bobus.levelID = i;
        //    bobus.transform.parent = levelSelectButtonContainer;
        //    bobus.transform.localPosition = new Vector2(i % 5 * 1.5f, - Mathf.FloorToInt(i / 5) * 1.5f);
        //}

        MainMenu();
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        levelSelect.SetActive(false);

        UIOverseer.i.InitUI();
        var bg = GameObject.Find("menu_bg");
        var logo = GameObject.Find("logo");

        bg.transform.position = new Vector3(3.5f, 0, 0);
        bg.transform.DOMoveX(0, 5).SetEase(Ease.OutQuart);

        logo.transform.position = new Vector3(logo.transform.position.x, 8.5f, 0);
        logo.transform.DOMoveY(2.5f, 3).SetEase(Ease.OutQuart);
    }

    public void LevelSelect()
    {
        mainMenu.SetActive(false);
        levelSelect.SetActive(true);

        foreach(Transform bobibou in levelSelectButtonContainer)
        {
            Destroy(bobibou.gameObject);
        }

        for (int i = 0; i < 14; i++)
        {
            LevelSelectButton bobus = Instantiate(levelSelectButtonPrefab);
            bobus.levelID = i;
            bobus.transform.parent = levelSelectButtonContainer;
            bobus.transform.localPosition = new Vector2(i % 5 * 1.5f, -Mathf.FloorToInt(i / 5) * 1.5f);
        }

        UIOverseer.i.InitUI();
    }

    public void RefreshPaper(LevelSelectButton cara)
    {
        journal.SetActive(true);
        journal.transform.DOComplete();
        journal.transform.DOShakeScale(.4f, .25f, 20).OnComplete(() => journal.transform.localScale = Vector3.one);
        dayText.text = "<size=.6666666>DAY"+(cara.levelID+1).ToString();
        titleText.text = "<size=.5>" +cara.levelName.ToUpper();
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(cara.time);
        string format = string.Format("<size=.25>Best Time\n{0:D2}:{1:D2}.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt(timeSpan.Milliseconds / 10));
        bestTimeText.text = format;
        System.TimeSpan timeSpan2 = System.TimeSpan.FromSeconds(cara.goalTime);
        string format2 = string.Format("<size=.25>Goal Time\n{0:D2}:{1:D2}.{2:D2}", timeSpan2.Minutes, timeSpan2.Seconds, Mathf.FloorToInt(timeSpan2.Milliseconds / 10));
        goalTimeText.text = format2;

        kyur.sprite = kyurs[cara.levelID % 4];

        stars.sprite = starss[cara.score];
    }
}
