using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Utility : ScriptableObject
{
	public static LaundryItem GetRandomLaundryItem()
	{
		LaundryItem laundryItem = ScriptableObject.CreateInstance<LaundryItem>();
		laundryItem.colorEnum = Utility.GetRandomLaundryColor();
		laundryItem.type = Utility.GetRandomLaundryType();

		return laundryItem;
	}
	
    public static LaundryType GetRandomLaundryType()
	{
		return GetRandomEnum<LaundryType>();
	}

	public static LaundryColor GetRandomLaundryColor()
	{
		return GetRandomEnum<LaundryColor>();
	}

	private static T GetRandomEnum<T>() 
	{
		Array laundryTypes = Enum.GetValues(typeof(T));
		return (T)(laundryTypes.GetValue(Random.Range(0, laundryTypes.Length)));
	}
}
