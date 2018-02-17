using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laundry : MonoBehaviour
{

    public LaundryType type;
    public float pointValue;


    private LaundryColor _color;
    public LaundryColor color
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
        }
    }

	public static GameObject GetLaundryPrefabForLaundryType(LaundryType laundryType)
    {
        foreach (GameObject gameObject in GameManager._instance.laundryItems)
        {
            Laundry laundryObject = gameObject.GetComponent<Laundry>();

            if (laundryObject.type == laundryType)
            {
                return gameObject;
            }
        }

        return null;
    }
}
