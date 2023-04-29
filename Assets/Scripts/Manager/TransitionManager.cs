using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager _i;

    public static TransitionManager i
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [InspectorButton("NextLevel")]
    public bool nextlevel;

    [SerializeField] Transform transi;

    private void Start()
    {
        //SaveManager.i.Load();
    }

    public void TransiIn()
    {
        SoundManager.i.MusicOut();
        transi.DOKill();
        transi.position = new Vector2(40, Camera.main.transform.position.y);
        transi.DOMoveX(0, 1.5f).SetEase(Ease.InOutCubic).SetUpdate(true);
    }

    public void TransiOut(float wait = 0f)
    {
        Time.timeScale = 1;
        transi.DOKill();
        transi.position = new Vector2(0, Camera.main.transform.position.y);
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(wait);
        sequence.Append(transi.DOMove(Vector3.zero, 0f));
        sequence.Append(transi.DOMoveX(-40, 1.5f).SetEase(Ease.InOutCirc).SetUpdate(true));
    }

    public void NextLevel()
    {
    }


    public void LoadLevel(int level = 0)
    {
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        TransiIn();
        yield return new WaitForSecondsRealtime(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        StartCoroutine(MainMenuCoroutine());
    }

    IEnumerator MainMenuCoroutine()
    {
        TransiIn();
        yield return new WaitForSecondsRealtime(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        TransiOut(1f);
    }
}
