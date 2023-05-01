using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _i;

    public static UIManager i
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
    [SerializeField] TMPro.TextMeshPro timeText;

    [SerializeField] Transform customerBubble;
    [SerializeField] TMPro.TextMeshPro customerText;

    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject winScreenPlate;
    [SerializeField] TMPro.TextMeshPro winScreenText;
    [SerializeField] List<GameObject> stars;

    public void UpdateTime(float time)
    {
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);
        string format = string.Format("<size=.666666>{0:D2}:{1:D2}<size=.5>.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt(timeSpan.Milliseconds/10));
        timeText.text = format;
    }

    public void UpdateCustomerBubble(int totalS, int total, bool shake = true)
    {
        customerText.text = "<size=.5>" + totalS.ToString() + "/" + total.ToString();
        if (shake)
        {
            customerBubble.DOComplete();
            customerBubble.DOShakeScale(.4f, .5f, 20).OnComplete(() => customerBubble.localScale = Vector3.one);
        }
    }

    public void LevelWinScreen()
    {
        StartCoroutine(WinScreenCoroutine());
    }

    IEnumerator WinScreenCoroutine()
    {
        winScreen.transform.parent.gameObject.SetActive(true);
        winScreen.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0, 0, 0, 0);
        winScreen.GetComponent<SpriteRenderer>().DOFade(.4f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        winScreenPlate.SetActive(true);
        UIOverseer.i.InitUI();
        winScreenText.text = "";
        winScreenPlate.transform.DOShakeScale(.4f, .5f, 20);
        yield return new WaitForSecondsRealtime(1f);
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(GameManager.i.timer);
        string format = string.Format("<size=.5>TIME {0:D2}:{1:D2}<size=.333333>.{2:D2}\n ", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt(timeSpan.Milliseconds / 10));
        winScreenText.text = format;
        yield return new WaitForSecondsRealtime(.75f);
        format = string.Format("<size=.5>TIME {0:D2}:{1:D2}<size=.333333>.{2:D2}\n", timeSpan.Minutes, timeSpan.Seconds, Mathf.FloorToInt(timeSpan.Milliseconds / 10));
        System.TimeSpan timeSpan2 = System.TimeSpan.FromSeconds(GameManager.i.world.time);
        string format2 = string.Format("<size=.5>GOAL {0:D2}:{1:D2}<size=.333333>.{2:D2}", timeSpan2.Minutes, timeSpan2.Seconds, Mathf.FloorToInt(timeSpan2.Milliseconds / 10));
        winScreenText.text = format + format2;
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i<GameManager.i.score; i++)
        {
            stars[i].SetActive(true);
            yield return new WaitForSecondsRealtime(.35f);
        }
    }
}
