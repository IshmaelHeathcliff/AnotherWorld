using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Mono.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBoard : MonoBehaviour
{
    static WorldBoard _instance;

    public static WorldBoard Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<WorldBoard>();
            }
            
            return _instance;
        }
        private set => _instance = value;
    }
    
    public GameObject[] cellPfs;
    const float Dist = 1.8f;
    public Texture2D boardShape;

    Cell[,] _board;

    public class Point
    {
        public Cell Cell;
        public Point Parent;
        
        public float F => H + G;
        public float H;
        public float G;

        public Point(Cell cell, Point parent, bool init=false)
        {
            Cell = cell;
            Parent = parent;
            H = BoardDistance(Character.Instance.CurrentCell.boardPos, cell.boardPos);
            
            if (init)
            {
                G = 0;
            }
            else
            {
                G = Parent.G + 1;
            }
        }
    }

    public static Vector3 BoardToWorldPosition(Vector3 boardPos)
    {
        var xDirection = new Vector3(boardPos.x * Dist, 0, 0);
        var zDirection = new Vector3(boardPos.y * Dist / 2, 0, boardPos.y * Dist * Mathf.Sqrt(3) / 2);
        return xDirection + zDirection;
    }

    public static float BoardDistance(Vector3 posA, Vector3 posB)
    {
        Vector3 dist = posA - posB;
        return (Mathf.Abs(dist.x) + Mathf.Abs(dist.y) + Mathf.Abs(dist.z)) / 2;
    }

    public bool FindPath(Vector3 targetPos)
    {
        Cell currentCell = Character.Instance.CurrentCell;
        var directDistance = BoardDistance(targetPos, currentCell.boardPos);
        if (directDistance > 2 * Character.Instance.maxDistance)
        {
            return false;
        }
        
        var open = new List<Point>();
        var closed = new List<Point>();
        
        open.Add(new Point(currentCell, null, true));

        while (true)
        {
            if (open.Count == 0)
            {
                Debug.Log("can't find path");
                return false;
            }
            Point minPoint = open[0];

            foreach (Point point in open)
            {
                if (point.F < minPoint.F)
                    minPoint = point;
            }

            open.Remove(minPoint);
            closed.Add(minPoint);

            Vector3 minPointPos = minPoint.Cell.boardPos;
            var x = minPointPos.x.ConvertTo<int>();
            var y = minPointPos.y.ConvertTo<int>();
            var openCells = from point in open select point.Cell;
            var closedCells = from point in closed select point.Cell;


            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    if ( i == j || x + i < 0 || y + j < 0 || 
                         x + i >= _board.GetLength(0) || y + j >= _board.GetLength(1)) continue;
                    
                    Cell nextCell = _board[x + i, y + j];
                    var nextPoint = new Point(nextCell, minPoint);

                    if (nextCell.boardPos == targetPos)
                    {
                        var maxCount = Character.Instance.maxDistance;
                        var path = new Stack<Cell>();
                        var reachablePath = new Queue<Cell>();
                        path.Push(nextCell);
                        Point prePoint = minPoint;
                        while (prePoint.Parent != null)
                        {
                            path.Push(prePoint.Cell);
                            prePoint = prePoint.Parent;
                        }

                        while (reachablePath.Count < maxCount && path.Count > 0)
                        {
                            reachablePath.Enqueue(path.Pop());
                        }

                        Character.Instance.Path = reachablePath;

                        return true;
                    }

                    if (nextCell.canPass && !openCells.Contains(nextCell) && !closedCells.Contains(nextCell))
                    {
                        open.Add(nextPoint);
                    }

                }
            }
        }

    }


    void Awake()
    {
        if (Instance != this)
        {
            DestroyImmediate(this);
            return;
        }
        
        _board = new Cell[boardShape.height, boardShape.width];
    }
    

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
                    var boardPos = new Vector3(i, j, 0-i-j);
                    var cell = cellPf.GetComponent<Cell>();
                    if (cell.MapColor(color))
                    {
                        GameObject obj = Instantiate(cellPf, BoardToWorldPosition(boardPos), Quaternion.identity, worldBoard.transform);
                        var newCell = obj.GetComponent<Cell>();
                        newCell.boardPos = boardPos;
                        _board[i, j] = newCell;
                    }
                }
            }
        }

        Character.Instance.CurrentCell = _board[0, 0];
    }
}
