using DG.Tweening;
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
}
