using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;

public class Backrooms : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TextMeshPro starsText;
    public Transform BG;
    void Start()
    {
        int totalScore = 0;
        for (int i = 0; i < 20; i++)
        {
            int score = SaveManager.i.GetScore(i);
            totalScore += score;
        }

        starsText.text = "<size=.5>" + totalScore.ToString() + "/60";

        BG.DOScale(1.2f, 10f).SetEase(Ease.OutQuad);

        

        TransitionManager.i.TransiOut(.5f);

        Invoke("woo", 1.5f);

        UIOverseer.i.InitUI();
    }

    void woo()
    {
        SoundManager.i.Play("snd_debbieWOO", 0, .8f);
    }

 }
