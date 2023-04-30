using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlateState
{
    Ready,
    PickedUp,
    OnTable
}

public class Plate : MonoBehaviour
{
    [Header("Gameplay")]
    public int dishType = 0;
    [SerializeField] PlateState state = PlateState.Ready;

    [Header("References")]
    [SerializeField] SpriteRenderer plateRenderer;
    [SerializeField] SpriteRenderer mealRenderer;
    [SerializeField] ParticleSystem ps;

    GameObject magnetizeTarget;
    Vector2 magnetizeOffset;
    float magnetizeTimer;
    float magnetizeDuration = .25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == PlateState.Ready && collision.CompareTag("Player"))
        {
            state = PlateState.PickedUp;
            //GameManager.i.player.PickUpPlate(this);

            Vector2 offset = GameManager.i.player.GetPlateOffset(this);
            Magnetize(GameManager.i.player.gameObject, offset, .25f);
            Invoke("PlayerPickUpPlate", .25f);
        }   
    }

    private void Update()
    {
        if (magnetizeTarget != null)
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)magnetizeTarget.transform.position + magnetizeOffset, magnetizeTimer / magnetizeDuration);
            magnetizeTimer += Time.deltaTime;
        }
    }

    public void Magnetize(GameObject target, Vector2 offset, float duration = .25f)
    {
        magnetizeTarget = target;
        magnetizeOffset = offset;
        magnetizeDuration = duration;
        magnetizeTimer = 0f;
        Invoke("UnMagnetize", duration);
    }

    void UnMagnetize()
    {
        magnetizeTarget = null;
    }
    
    void PlayerPickUpPlate()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        GameManager.i.player.PickUpPlate(this);
    }

    public void SetRenderingOrder(int order)
    {
        plateRenderer.sortingOrder = order * 2;
        mealRenderer.sortingOrder = order * 2 + 1;
    }

    public void SetDish(int dishType)
    {
        this.dishType = dishType;
        mealRenderer.sprite = GameManager.i.dishSprites[dishType];
    }

    public void Drop()
    {
        float offset = transform.localPosition.y;
        if (offset == 0)
        {
            Break(false);
        }
        else
        {
            Break(true);
        }
        GetComponent<CircleCollider2D>().enabled = false;
        transform.parent = transform.parent.parent.parent.parent;
        transform.DOMoveY(-2 - offset, .4f).SetRelative(true).SetEase(Ease.InQuart);
        //Invoke("Break", .4f);
    }

    void Break(bool hard = false)
    {
        if (hard)
        {
            Invoke("BreakHard", .4f);
        } else
        {
            Invoke("BreakSoft", .4f);
        }
    }

    void BreakHard()
    {
        plateRenderer.enabled = false;
        ps.Play();
        GameManager.i.BreakPlate(dishType);

        mealRenderer.enabled = false;
        Destroy(gameObject, 1f);
    }

    void BreakSoft()
    {
        plateRenderer.enabled = false;
        ps.Play();
        GameManager.i.BreakPlate(dishType);

        mealRenderer.sprite = GameManager.i.sludgeSprites[Random.Range(0, GameManager.i.sludgeSprites.Count)];
        mealRenderer.sortingLayerName = "Entities";
        mealRenderer.sortingOrder = -1;
        this.enabled = false;
    }
}
