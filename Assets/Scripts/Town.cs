using UnityEngine;

public class Town : Cell
{
    public override bool MapColor(Color color)
    {
        return color.g == 0 && color.b == 0 && color.r > 0;
    }
}
