using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCustom : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        sr.sprite = sprites[0];
    }

    private void OnMouseEnter()
    {
        sr.sprite = sprites[1];
        SoundManager.i.Play("MenuMove", .15f, .8f);
    }

    private void OnMouseExit()
    {
        sr.sprite = sprites[0];
    }

    private void OnMouseDown()
    {
        sr.sprite = sprites[2];
        onClick.Invoke();
        Invoke("ResetHover", .1f);
    }

    void ResetHover()
    {
        sr.sprite = sprites[1];
    }

    public UnityEvent onClick;
}
