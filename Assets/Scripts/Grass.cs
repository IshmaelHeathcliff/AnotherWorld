using UnityEngine;

public class Grass : Cell
{
    public override bool MapColor(Color color)
    {
        return color.r == 0 && color.b == 00 && color.g > 0.5;
    }
}
