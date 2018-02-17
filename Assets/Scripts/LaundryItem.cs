using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaundryItem : ScriptableObject
{
	public LaundryColor colorEnum;
    public LaundryType type;

    public GameObject CreateInstance(Transform parent)
    {
        if (parent == null)
        {
            throw new ArgumentException("No parent supplied to create instance of LaundryItem");
        }

        GameObject prefab = Laundry.GetLaundryPrefabForLaundryType(this.type);
        GameObject instance = Instantiate(prefab, parent.position, parent.rotation, parent);

        Laundry laundryObject = instance.GetComponent<Laundry>();
        laundryObject.color = this.colorEnum;

        SpriteRenderer spriteRenderer = instance.transform.Find("sprite").GetComponent<SpriteRenderer>();
        spriteRenderer.color = LaundryColorClass.GetInstanceFromEnum(this.colorEnum).color;

        return instance;
    }
}
