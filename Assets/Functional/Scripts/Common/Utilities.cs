using UnityEngine;

public static class Utilities
{
    public static Color ToColor(this string color)
    {
        return (Color)typeof(Color).GetProperty(color.ToLowerInvariant()).GetValue(null, null);
    }
}
