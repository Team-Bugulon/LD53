using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}