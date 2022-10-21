using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class WorldBoard : MonoBehaviour
{
    public GameObject[] cellPfs;
    const float dist = 1.8f;
    public Texture2D boardShape;

    List<Cell> _cells = new List<Cell>();

    void Start()
    {
        GameObject worldBoard = GameObject.Find("WorldBoard");

        for (var i = 0; i < boardShape.height; i++)
        {
            for (var j = 0; j < boardShape.width; j++)
            {
                Color color = boardShape.GetPixel(j, i);
                foreach (GameObject cellPf in cellPfs)
                {
                    var boardPos = new Vector2(i, j);
                    var cell = cellPf.GetComponent<Cell>();
                    if (cell.MapColor(color))
                    {
                        GameObject obj = Instantiate(cellPf, BoardToWorldPosition(boardPos), Quaternion.identity, worldBoard.transform);
                        var newCell = obj.GetComponent<Cell>();
                        newCell.boardPos = boardPos;
                        _cells.Add(newCell);
                    }
                }
            }
        }
    }
    
    public static Vector3 BoardToWorldPosition(Vector2 boardPos)
    {
        var x = dist * 0.866f * boardPos.x;
        var z = dist * boardPos.y;
        if (boardPos.x % 2 != 0) z += dist/2;
        return new Vector3(x, 0, z);
    }
}
