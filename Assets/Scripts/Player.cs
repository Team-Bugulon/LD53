using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float kitchenSpeed = 1f;
    [SerializeField] float turnSpeed = 1f;
    [SerializeField] float brakeSpeed = 1f;
    [SerializeField] float accelerateSpeed = 1f;
    [SerializeField] float collisionForce = 10;

    [SerializeField] float stunTime = 1f;
    [SerializeField] float freezeTime = 1f;
    [SerializeField] float cooldownTime = 2f;
    public bool stunned = false;
    public bool canBeHurt = true;
    
    public bool inKitchen = false;

    bool actionAfterPause = false;

    float direction = 0;
    float realSpeed;
    bool controlsLocked = false;
    bool isBraking = false;
    public bool atRest = false;
    Vector2 movementInput = Vector2.zero;
    //CircleCollider2D cc;
    BoxCollider2D cc;
    Animator ac;
    SpriteRenderer sr;
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
            actionAfterPause = true;
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
        //cc = GetComponent<CircleCollider2D>();
        cc = GetComponent<BoxCollider2D>();

        rb = GetComponent<Rigidbody2D>();
        ac = GetComponent<Animator>();
        platesHeld = new List<Plate>();
        sr = GetComponent<SpriteRenderer>();
        sr.material = new Material(sr.material);
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

        foreach(Table table in FindObjectsOfType<Table>())
        {
            table.OnPlayerPickupFood(plate.dishType);
        }
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
                plate.SetRenderingOrder(-1);

                UpdatePlatesPos();

                //return true
                return true;
            }
        }
        return false;
    }

    void UpdatePlatesPos()
    {
        //get all plates in plateContainerL
        List<Plate> plateL = plateContainerL.GetComponentsInChildren<Plate>().ToList();
        //sort plates by local position y
        //int absoluteY = plateL.Count;
        int absoluteY = 0;
        foreach (Plate plate in plateL)
        {
            plate.SetRenderingOrder(absoluteY);
            float localX = (absoluteY % 2) * 4 * pixelSize;
            float localY = absoluteY * 8 * pixelSize;
            Vector2 localPos = new Vector2(localX, localY);
            plate.transform.localPosition = localPos;
            absoluteY++;
        }

        //get all plates in plateContainerL
        List<Plate> plateR = plateContainerR.GetComponentsInChildren<Plate>().ToList();
        //sort plates by local position y
        //absoluteY = plateR.Count;
        absoluteY = 0;
        foreach (Plate plate in plateR)
        {
            plate.SetRenderingOrder(absoluteY);
            float localX = (absoluteY % 2) * 4 * pixelSize;
            float localY = absoluteY * 8 * pixelSize;
            Vector2 localPos = new Vector2(localX, localY);
            plate.transform.localPosition = localPos;
            absoluteY++;
        }
    }


    void FixedUpdate()
    {
        if (!stunned)
        {
            if (movementInput != Vector2.zero && !isBraking)
            {
                //get vector angle from movementInput
                var newDirection = Vector2.SignedAngle(Vector2.right, movementInput);

                direction = Mathf.LerpAngle(direction, newDirection, turnSpeed);
            }

            if (isBraking || !actionAfterPause)
            {
                realSpeed = Mathf.Lerp(realSpeed, 0, brakeSpeed);
            }
            else
            {
                if (!inKitchen)
                {
                    realSpeed = Mathf.Lerp(realSpeed, moveSpeed, accelerateSpeed);
                } else
                {
                    realSpeed = Mathf.Lerp(realSpeed, kitchenSpeed, accelerateSpeed);
                }
                
            }

            if (realSpeed <= 3f)
            {
                atRest = true;
                //movementInput = Vector2.zero;
                if (movementInput == Vector2.zero) actionAfterPause = false;
            }
            else
            {
                atRest = false;
            }

            rb.velocity = DegreeToVector2(direction) * realSpeed;
        }

    }

    private void Update()
    {
        Animate();
    }

    void Animate()
    {
        if (stunned)
        {
            PlayAnimation("player_hurt");
        }
        else if (atRest)
        {
            PlayAnimation("player_idle");
        }
        else if (isBraking)
        {
            string suffix = "front";
            if (rb.velocity.y > 0f)
            {
                suffix = "back";
            }
            if (rb.velocity.x < -.1f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            } else if (rb.velocity.x > .1f)
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
            if (rb.velocity.x < -.1f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (rb.velocity.x > .1f)
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!atRest && !inKitchen && !collision.gameObject.CompareTag("Counter"))
        {
            Vector2 collisionDirection = DegreeToVector2(direction).normalized;

            Hurt(collisionDirection);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!atRest && !inKitchen && !collision.gameObject.CompareTag("Counter"))
        {
            Vector2 collisionDirection = DegreeToVector2(direction).normalized;

            Hurt(collisionDirection);
        }
    }

    public void Hurt(Vector2 collisionDirection)
    {
        if (canBeHurt)
        {
            canBeHurt = false;
            realSpeed = 0;
            stunned = true;
            rb.velocity = Vector2.zero;

            //rb.AddForce(-collisionDirection * collisionForce);
            //GameManager.i.ShakeScreen();
            //SoundManager.i.Play("Damage2", .1f, .8f);
            StartCoroutine(StunTimer(-collisionDirection * collisionForce));
            Camera_Manager.i.ShakeScreen(duration:.65f);
            //StartCoroutine(CooldownTimer());

            foreach (Plate plate in platesHeld)
            {
                plate.Drop();
            }

            platesHeld = new List<Plate>();

            foreach (Table table in FindObjectsOfType<Table>())
            {
                table.OnPlayerDropFood();
            }
        }
    }

    IEnumerator StunTimer(Vector2 force)
    {
        sr.material.SetColor("_TintAdd", Color.white);
        yield return new WaitForSeconds(.05f);
        sr.material.SetColor("_TintAdd", Color.black);
        yield return new WaitForSeconds(freezeTime - .05f);
        rb.AddForce(force);
        yield return new WaitForSeconds(stunTime);
        realSpeed = 0;
        stunned = false;
        StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer()
    {
        float flickerSpeed = .1f;
        int tickQty = Mathf.RoundToInt(cooldownTime / flickerSpeed);
        int flip = 0;
        for (int i = 0; i < tickQty; i++)
        {
            if (flip == 0)
            {
                sr.material.SetColor("_TintAdd", Color.white);
            }
            else
            {
                sr.material.SetColor("_TintAdd", Color.black);
            }

            flip = (flip + 1) % 2;

            yield return new WaitForSeconds(flickerSpeed);
        }
        sr.material.SetColor("_TintAdd", Color.black);

        canBeHurt = true;
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
