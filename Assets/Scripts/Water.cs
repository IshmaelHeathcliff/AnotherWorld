using UnityEngine;

public class Water : Cell
{
    public override bool MapColor(Color color)
    {
        return color.r == 0 && color.g == 0 && color.b > 0;
    }
}
