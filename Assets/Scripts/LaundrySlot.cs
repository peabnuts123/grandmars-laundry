using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaundrySlot : MonoBehaviour
{
    public GameObject unoHighlight;

    public bool isHighlighted
    {
        get
        {
            return this.animator.GetBool("isHighlighted");
        }
        set
        {
            this.animator.SetBool("isHighlighted", value);
        }
    }

    public bool isSelected
    {
        get
        {
            return this.animator.GetBool("isSelected");
        }
        set
        {
            this.animator.SetBool("isSelected", value);
        }
    }

    public bool isUno
    {
        get
        {
            return this.animator.GetBool("isUno");
        }
        set
        {
            this.unoHighlight.SetActive(value);
            this.animator.SetBool("isUno", value);
        }
    }

    private Rect worldMouseRect;
    private Animator animator;
    private GameObject laundryItem;
    private bool _canUno;
    private SpriteRenderer spriteRenderer;

    public bool canUno
    {
        get
        {
            return this._canUno;
        }
        set
        {
            this._canUno = value;
        }
    }


    void Start()
    {
        // Convert box collider to a world rect
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        worldMouseRect = new Rect(this.transform.position.x + boxCollider.offset.x - boxCollider.size.x * 0.5f, this.transform.position.y + boxCollider.offset.y - boxCollider.size.y * 0.5f, boxCollider.size.x, boxCollider.size.y);

        this.animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var topLeft = new Vector2(worldMouseRect.xMin, worldMouseRect.yMin);
        var topRight = new Vector2(worldMouseRect.xMax, worldMouseRect.yMin);
        var bottomRight = new Vector2(worldMouseRect.xMax, worldMouseRect.yMax);
        var bottomLeft = new Vector2(worldMouseRect.xMin, worldMouseRect.yMax);

        Debug.DrawLine(topLeft, topRight, Color.green);
        Debug.DrawLine(topRight, bottomRight, Color.green);
        Debug.DrawLine(bottomRight, bottomLeft, Color.green);
        Debug.DrawLine(bottomLeft, topLeft, Color.green);
    }

    public void GiveItem(GameObject laundryItem)
    {
        this.laundryItem = laundryItem;
        this.spriteRenderer.enabled = false;

        // MINE!!!
        laundryItem.transform.parent = this.transform;
        laundryItem.transform.position = this.transform.position;
        laundryItem.transform.rotation = this.transform.rotation;
    }

    public GameObject TakeItem()
    {
        GameObject item = this.laundryItem;
        this.laundryItem = null;
        this.spriteRenderer.enabled = true;
        return item;
    }

    public GameObject GetItem()
    {
        return this.laundryItem;
    }

    public bool HasItem()
    {
        return this.laundryItem != null;
    }

    public void Reset()
    {
        this.isHighlighted = false;
        this.isSelected = false;
        this.isUno = false;
        this.canUno = true;
    }

    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        return worldMouseRect.Contains(worldPoint);
    }
}
