using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSlot : MonoBehaviour
{
    private const int OBJECT_INDEX = 2;
    private Rect worldMouseRect;
    private Animator animator;

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

    void Start()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        worldMouseRect = new Rect(this.transform.position.x + boxCollider.offset.x - boxCollider.size.x * 0.5f, this.transform.position.y + boxCollider.offset.y - boxCollider.size.y * 0.5f, boxCollider.size.x, boxCollider.size.y);

        this.animator = GetComponent<Animator>();
    }

    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        return worldMouseRect.Contains(worldPoint);
    }

    public GameObject GetObject()
    {
        return this.transform.GetChild(OBJECT_INDEX).gameObject;
    }

    public bool HasObject()
    {
        return this.transform.childCount > OBJECT_INDEX;
    }
}
