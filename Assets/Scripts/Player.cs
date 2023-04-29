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
        }
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
            //var newDirection = Vector2.Angle(Vector2.right, movementInput);

            //direction = Mathf.Lerp(direction, newDirection, turnSpeed);

            rb.velocity = movementInput.normalized * moveSpeed;
        }

        //rb.velocity = (Quaternion.AngleAxis(direction, Vector3.forward) * Vector3.right).normalized * moveSpeed;
        //rb.velocity = (Vector2)(Quaternion.Euler(0, 0, direction) * Vector2.right).normalized * moveSpeed;
    }

}
