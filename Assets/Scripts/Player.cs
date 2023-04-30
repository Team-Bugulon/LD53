using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float turnSpeed = 1f;
    [SerializeField] float brakeSpeed = 1f;
    [SerializeField] float accelerateSpeed = 1f;
    float direction = 0;
    float realSpeed;
    bool controlsLocked = false;
    bool isBraking = false;
    public bool atRest = false;
    Vector2 movementInput = Vector2.zero;
    CircleCollider2D cc;
    Animator ac;
    public Rigidbody2D rb;
    List<Plate> platesHeld;
    

    [Header("References")]
    [SerializeField] Transform plateContainerL;
    [SerializeField] Transform plateContainerR;

    const float pixelSize = 1f / 32f;

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

    public void Brake(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !controlsLocked)
        {
            isBraking = true;
        }
        else if (ctx.canceled)
        {
            isBraking = false;
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
        ac = GetComponent<Animator>();
        platesHeld = new List<Plate>();
    }

    public void PickUpPlate(Plate plate)
    {
        int platesQty = platesHeld.Count;
        float localX;
        float localY;
        int absoluteY;

        string pos;
        if (platesQty % 2 == 0)
        {
            pos = "left";
            plate.transform.parent = plateContainerL;
        } else
        {
            pos = "right";
            plate.transform.parent = plateContainerR;
        }

        absoluteY = Mathf.FloorToInt(platesQty / 2);
        plate.SetRenderingOrder(absoluteY);

        localX = (absoluteY % 2) * 4 * pixelSize;
        localY = absoluteY * 8 * pixelSize;
        Vector2 localPos = new Vector2(localX, localY);

        plate.transform.localPosition = localPos;

        platesHeld.Add(plate);
    }

    public Vector2 GetPlateOffset(Plate plate)
    {
        int platesQty = platesHeld.Count;
        float localX;
        float localY;
        int absoluteY;

        Vector2 offset = Vector2.zero;

        string pos;
        if (platesQty % 2 == 0)
        {
            pos = "left";
            offset += (Vector2)plateContainerL.localPosition;
        }
        else
        {
            pos = "right";
            offset += (Vector2)plateContainerR.localPosition;
        }

        absoluteY = Mathf.FloorToInt(platesQty / 2);
        
        plate.SetRenderingOrder(absoluteY);

        localX = (absoluteY % 2) * 4 * pixelSize;
        localY = absoluteY * 8 * pixelSize;
        Vector2 localPos = new Vector2(localX, localY);

        offset += localPos;

        return offset;
    }

    public bool DiscardPlate(int dishType, Customer customer)
    {
        //if one of the plate is of dish
        foreach (Plate plate in platesHeld)
        {
            if (plate.dishType == dishType)
            {
                //remove plate from list
                platesHeld.Remove(plate);
                plate.transform.parent = customer.assignedPlateContainer;
                plate.Magnetize(customer.assignedPlateContainer.gameObject, Vector2.zero, .5f);
                //return true
                return true;
            }
        }
        return false;
    }


    void FixedUpdate()
    {
        if (movementInput != Vector2.zero && !isBraking)
        {
            //get vector angle from movementInput
            var newDirection = Vector2.SignedAngle(Vector2.right, movementInput);

            direction = Mathf.LerpAngle(direction, newDirection, turnSpeed);
        }

        if (isBraking)
        {
            realSpeed = Mathf.Lerp(realSpeed, 0, brakeSpeed);
        } else
        {
            realSpeed = Mathf.Lerp(realSpeed, moveSpeed, accelerateSpeed);
        }

        if (realSpeed <= 3f)
        {
            atRest = true;
        } else
        {
            atRest = false;
        }

        rb.velocity = DegreeToVector2(direction) * realSpeed;
    }

    private void Update()
    {
        Animate();
    }

    void Animate()
    {
        if (isBraking)
        {
            string suffix = "front";
            if (rb.velocity.y > 1f)
            {
                suffix = "back";
            }
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            } else
            {
                transform.localScale = Vector3.one;
            }
            PlayAnimation("player_brake_" + suffix);
        } else
        {
            string suffix = "front";
            if (rb.velocity.y > 1f)
            {
                suffix = "back";
            }
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
            PlayAnimation("player_run_" + suffix);
        }
    }
    void PlayAnimation(string animName)
    {
        if (!ac.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            ac.Play(animName);
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
