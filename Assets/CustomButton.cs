using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;
    Animator an;
    SpriteRenderer sr;

    [SerializeField] int buttonBehavior = 0;

    [SerializeField] bool singleUse = true;
    bool used = false;

    public bool usesAnimator = false;

    // Start is called before the first frame update
    void Start()
    {
        if (usesAnimator) an = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        used = false;
        if (usesAnimator)
        {
            an = GetComponent<Animator>();
            an.Play("default");
        } else
        {
            sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprites[0];
        }
    }

    public void Hover()
    {
        if (usesAnimator)
        {
            an.Play("hover");
        }
        else
        {
            sr.sprite = sprites[1];
        }
        //SoundManager.i.Play("MenuMove", .15f, .8f);
    }

    public void Default()
    {
        if (usesAnimator)
        {
            an.Play("default");
        }
        else
        {
            sr.sprite = sprites[0];
        }
    }

    public void Press()
    {
        if ((singleUse && !used) || !singleUse) {
            used = true;
            if (usesAnimator)
            {
                an.Play("press");
            }
            else
            {
                sr.sprite = sprites[2];
            }
            onPress.Invoke();
            ButtonBehavior();
            Invoke("ResetHover", .1f);
        }

        if (singleUse)
        {
            UIOverseer.i.isActive = false;
        }
    }

    void ResetHover()
    {
        if (usesAnimator)
        {
            an.Play("hover");
        }
        else
        {
            sr.sprite = sprites[1];
        }
    }

    void ButtonBehavior()
    {
        switch (buttonBehavior)
        {
            case 0:
                Debug.Log("GO BACK MENU");
                TransitionManager.i.MainMenu();
                break;
            case 1:
                Debug.Log("RESTART LEVEL");
                TransitionManager.i.LoadLevel();
                break;
            case 2:
                Debug.Log("NEXT LEVEL");
                TransitionManager.i.NextLevel();
                break;
        }
    }

    public UnityEvent onPress;
}
