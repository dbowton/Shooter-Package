using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Colors
{
    public static Color cyan = Color.cyan;
    public static Color magenta = Color.magenta;
    public static Color yellow = Color.yellow;

    public static Color red = Color.red;
    public static Color green = Color.green;
    public static Color blue = Color.blue;

    public static Color Mix(List<(Color color, float weight)> mix)
    {
        Color workingColor = new Color();

        workingColor.r = mix.Average(x => x.color.r * x.weight) / mix.Sum(x => x.weight);
        workingColor.g = mix.Average(x => x.color.g * x.weight) / mix.Sum(x => x.weight);
        workingColor.b = mix.Average(x => x.color.b * x.weight) / mix.Sum(x => x.weight);
        workingColor.a = mix.Average(x => x.color.a * x.weight) / mix.Sum(x => x.weight);

        return workingColor;
    }
}
