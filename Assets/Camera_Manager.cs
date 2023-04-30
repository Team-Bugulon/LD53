using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    private static Camera_Manager _i;

    public static Camera_Manager i
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Gameplay")]
    public float cameraDistancePx = 0f;
    public Vector2 ortho;
    public Vector2 orthoCropped;
    public int margin = 16;

    [Header("References")]
    public Camera mainCamera;

    const float pixelSize = 1f/32f;

    private void Start()
    {
        ortho = new Vector2(Camera_Manager.i.mainCamera.orthographicSize * 16f / 9f, Camera_Manager.i.mainCamera.orthographicSize);
        orthoCropped = new Vector2(ortho.x - margin * pixelSize, ortho.y - margin * pixelSize);
    }

    private void LateUpdate()
    {
        Vector2 offset = GameManager.i.player.rb.velocity.normalized * cameraDistancePx * pixelSize;
        Vector2 tempPos = (Vector2)GameManager.i.player.transform.position + offset;
        mainCamera.transform.position = new Vector3(
            Mathf.RoundToInt(tempPos.x / pixelSize) * pixelSize, 
            Mathf.RoundToInt(tempPos.y / pixelSize) * pixelSize, 
            mainCamera.transform.position.z);
    }
}
