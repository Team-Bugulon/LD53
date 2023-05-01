using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : MonoBehaviour
{
    [SerializeField] float speedModifier = 2f;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameManager.i.puddleSprites[Random.Range(0, GameManager.i.puddleSprites.Count)];
        sr.color = GameManager.i.puddleColors[Random.Range(0, 3) + GameManager.i.world.skin * 3];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.i.player.Slide(speedModifier, GetComponent<SpriteRenderer>().color);
        }
    }
}
