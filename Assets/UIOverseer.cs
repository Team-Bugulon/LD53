using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class UIOverseer : MonoBehaviour
{
    private static UIOverseer _i;

    public static UIOverseer i
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    Vector2 moveDir;
    bool firstPressed = false;

    public Vector2Int cursorPos;

    List<List<CustomButton>> internalLayout;

    bool isActive = false;

    bool movePerformed = false;

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !movePerformed)
        {
            movePerformed = true;
            Validity();
            if (isActive) Move(ctx.ReadValue<Vector2>());
        } else if (ctx.canceled)
        {
            movePerformed = false;
        }
    }

    public void Brake(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Validity();
            if (isActive) Press();
        }
    }

    public void InitUI()
    {
        isActive = true;
        firstPressed = false;
        cursorPos = Vector2Int.zero;
        internalLayout = new List<List<CustomButton>>();

        var cb = FindObjectsOfType<CustomButton>().ToList();

        //sort list by Y 
        cb = cb.OrderByDescending(x => x.transform.position.y).ToList();

        //group by Y
        var groups = new Dictionary<float, List<CustomButton>>();

        float tolerance = .5f;

        foreach (var item in cb)
        {
            float key = Mathf.FloorToInt(item.transform.position.y / tolerance) * tolerance;
            if (!groups.ContainsKey(key))
            {
                groups.Add(key, new List<CustomButton>());
            }
            groups[key].Add(item);
        }

        //sort each group by ascending X
        foreach (var key in groups.Keys)
        {
            internalLayout.Add(groups[key].OrderBy(x => x.transform.position.x).ToList());
        }

    }

    private void Move(Vector2 dir)
    {
        if (!firstPressed)
        {
            firstPressed = true;
        } else
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                moveDir = new Vector2(Mathf.Sign(dir.x), 0);
                cursorPos = new Vector2Int(
                    Mathf.Clamp(cursorPos.x + (int)moveDir.x, 0, internalLayout[cursorPos.y].Count - 1),
                    cursorPos.y
                    );
            }
            else
            {
                moveDir = new Vector2(0, Mathf.Sign(dir.y));
                cursorPos = new Vector2Int(
                    0,
                    Mathf.Clamp(cursorPos.y + (int)moveDir.y, 0, internalLayout.Count - 1)
                    );
            }
        }

        foreach(var ccb in internalLayout)
        {
            foreach(var ccbb in ccb)
            {
                ccbb.Default();
            }
        }

        internalLayout[cursorPos.y][cursorPos.x].Hover();
    }

    private void Press()
    {
        if (firstPressed)
        {
            internalLayout[cursorPos.y][cursorPos.x].Press();
        }
    }

    private void Validity()
    {
        var cb = FindObjectsOfType<CustomButton>().ToList();
        if (cb.Count == 0)
        {
            isActive = false;
        }
    }
}
