using UnityEngine;

public class Forest : Cell
{
    public override bool MapColor(Color color)
    {
        return color.r == 0 && color.b == 0 && color.g > 0 && color.g <= 0.5;
    }
}
