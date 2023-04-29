using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float turnSpeed = 1f;
    float direction = 0;
    bool controlsLocked = false;
    Vector2 movementInput = Vector2.zero;
    CircleCollider2D cc;
    Rigidbody2D rb;
    
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !controlsLocked)
        {
            movementInput = ctx.ReadValue<Vector2>();
        }
        else if (ctx.canceled)
        {
            movementInput = Vector2.zero;
            direction = direction % 360;
        }
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    private void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            //get vector angle from movementInput
            var newDirection = Vector2.SignedAngle(Vector2.right, movementInput);

            direction = Mathf.LerpAngle(direction, newDirection, turnSpeed);

            rb.velocity = DegreeToVector2(direction) * moveSpeed;
            Debug.Log("angle " + direction);
        }
    }


    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, (Vector3)rb.velocity.normalized + transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector3)movementInput.normalized + transform.position);
        }
    }
}
