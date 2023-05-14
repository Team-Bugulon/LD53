using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class SplashScreen : MonoBehaviour
{
    
    void Start()
    {
        //UnityEditor.PackageManager.Client.Embed("com.unity.render-pipelines.universal");
        GameObject.Find("bugulon_logo").GetComponent<SpriteRenderer>().DOColor(Color.black, 3.5f).SetEase(Ease.InExpo).OnComplete(() => UnityEngine.SceneManagement.SceneManager.LoadScene(1));
    }
}
