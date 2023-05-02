using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class splash : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("bugulon_logo").GetComponent<SpriteRenderer>().DOColor(Color.black, 3.5f).SetEase(Ease.InExpo).OnComplete(() => UnityEngine.SceneManagement.SceneManager.LoadScene(1));
    }

    
}
