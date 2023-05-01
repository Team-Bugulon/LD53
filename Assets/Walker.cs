using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Walker : MonoBehaviour
{
    public List<Vector2> path;
    [SerializeField] float speed = 1f;
    [SerializeField] float waitTime = 1f;

    bool stunned = false;
    [SerializeField] float stunnedDuration = 1f;

    private void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            foreach (Vector2 point in path)
            {
                yield return StartCoroutine(MoveTo(point));
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    IEnumerator MoveTo(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            if (!stunned)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                //flip direction
                if (transform.position.x - target.x > 0)
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                } else
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
            }
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!stunned)
            {
                stunned = true;
                GetComponent<Animator>().Play("walker_bumped");
                Invoke("Unstun", stunnedDuration);
                transform.DOComplete();
                transform.DOShakeScale(.4f, .5f, 20);
            }
        }
    }

    void Unstun()
    {
        stunned = false;
        GetComponent<Animator>().Play("walker_idle");
        transform.DOComplete();
        transform.DOShakeScale(.4f, .5f, 20);
    }
}
