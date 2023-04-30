using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speech : MonoBehaviour
{
    [SerializeField] SpriteRenderer Bubble;
    [SerializeField] SpriteRenderer Dish;

    private void Update()
    {
        //check if parent position is in screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.parent.position);
        if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
        {
            //place it on the side of the camera
            Bubble.transform.parent = Camera_Manager.i.mainCamera.transform;
            
            //calculate the position on the side of the camera towards the parent
            Vector3 dir = transform.parent.position - Camera_Manager.i.mainCamera.transform.position;
            Vector3 posClamped = new Vector3(Mathf.Clamp(dir.x, - Camera_Manager.i.orthoCropped.x, Camera_Manager.i.orthoCropped.x), Mathf.Clamp(dir.y, -Camera_Manager.i.orthoCropped.y, Camera_Manager.i.orthoCropped.y), 10);
            Bubble.transform.localPosition = posClamped;

            //rotate bubble towards parent
            Bubble.transform.rotation = Quaternion.LookRotation(Vector3.forward, -dir);
            Dish.transform.rotation = Quaternion.identity;
        }
        else
        {
            Bubble.transform.parent = transform;
            Bubble.transform.localPosition = Vector3.zero;
            Bubble.transform.localRotation = Quaternion.identity;
            Dish.transform.rotation = Quaternion.identity;
        }
    }

    public void Ordering()
    {
        this.gameObject.SetActive(true);
        Bubble.transform.DOComplete();
        Bubble.transform.DOPunchScale(Vector3.one * 1.05f, .25f, 10);
    }

    public void SetDish(int dishType)
    {
        Dish.sprite = GameManager.i.dishSprites[dishType];
        Bubble.transform.DOComplete();
        //Bubble.transform.DOPunchScale(Vector3.one * 1.05f, .25f, 10);
    }
}
