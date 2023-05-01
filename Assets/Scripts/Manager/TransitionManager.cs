using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

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
        //SoundManager.i.MusicOut();
        transi.gameObject.SetActive(true);
        transi.DOKill();
        transi.position = new Vector2(40 + Camera.main.transform.position.x, Camera.main.transform.position.y);
        transi.DOMoveX(Camera.main.transform.position.x, 1.5f).SetEase(Ease.InOutCubic).SetUpdate(true);
    }

    public void TransiOut(float wait = 0f, float anchorX = 0, float anchorY = 0)
    {
        Time.timeScale = 1;
        transi.DOKill();
        transi.position = new Vector2(anchorX, anchorY);
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(wait);
        //sequence.Append(transi.DOMove(Vector3.zero, 0f));
        sequence.Append(transi.DOMoveX(-40 + anchorX, 1.5f).SetEase(Ease.InOutCirc).SetUpdate(true)).OnComplete(() => transi.gameObject.SetActive(false));
    }

    public void NextLevel()
    {
        SaveManager.i.level++;
        LoadLevel();
    }

    public void LoadLevel(int level = -1)
    {
        Debug.Log("DOUBOLON " + level);
        if (level > -1) SaveManager.i.level = level;
        StartCoroutine(LoadLevelCoroutine());
    }

    IEnumerator LoadLevelCoroutine()
    {
        TransiIn();
        yield return new WaitForSecondsRealtime(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void MainMenu()
    {
        StartCoroutine(MainMenuCoroutine());
    }

    IEnumerator MainMenuCoroutine()
    {
        TransiIn();
        yield return new WaitForSecondsRealtime(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        TransiOut(1f);
    }
}
