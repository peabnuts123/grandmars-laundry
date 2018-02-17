using System;
using System.Collections.Generic;
using UnityEngine;

public class LaundryColorClass
{
    private static IEnumerable<LaundryColorClass> all = new List<LaundryColorClass> {
        new LaundryColorClass(new Color(200/255F, 44/255F, 44/255F), LaundryColor.red),
        new LaundryColorClass(new Color(22/255F, 175/255F, 85/255F), LaundryColor.green),
        new LaundryColorClass(new Color(92/255F, 153/255F, 235/255F), LaundryColor.blue),
        new LaundryColorClass(new Color(255/255F, 211/255F, 66/255F), LaundryColor.yellow),
        new LaundryColorClass(new Color(207/255F, 58/255F, 152/255F), LaundryColor.pink),
        new LaundryColorClass(new Color(207/255F, 106/255F, 22/255F), LaundryColor.orange),
    };

    public readonly Color color;
    private readonly LaundryColor colorEnum;

    private LaundryColorClass(Color color, LaundryColor colorEnum)
    {
        this.color = color;
        this.colorEnum = colorEnum;
    }

    public static LaundryColorClass GetInstanceFromEnum(LaundryColor colorEnum)
    {
        foreach (LaundryColorClass laundryColorClass in LaundryColorClass.all)
        {
            if (laundryColorClass.colorEnum == colorEnum)
            {
                return laundryColorClass;
            }
        }

        throw new ArgumentException("No Laundry Color Class defined with enum " + colorEnum);
    }
}

public enum LaundryColor
{
    red,
    green,
    blue,
    yellow,
    pink,
    orange
}