using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Mono.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        public readonly Cell Cell;
        public readonly Point Parent;
        
        public float F => _h + _g;
        readonly float _h;
        readonly float _g;

        public Point(Cell cell, Point parent, bool init=false)
        {
            Cell = cell;
            Parent = parent;
            _h = BoardDistance(Character.Instance.CurrentCell.boardPos, cell.boardPos);
            
            if (init)
            {
                _g = 0;
            }
            else
            {
                _g = Parent._g + 1;
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
            Character.Instance.Path = new Queue<Cell>();
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
            var x = (int)minPointPos.x;
            var y = (int)minPointPos.y;
            var openCells = (from point in open select point.Cell).ToList();
            var closedCells = (from point in closed select point.Cell).ToList();
            
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    if ( i == j || x + i < 0 || y + j < 0 || 
                         x + i >= _board.GetLength(0) || y + j >= _board.GetLength(1)) continue;
                    if (_board[x + i, y + j] == null) continue;
                    
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

    [ContextMenu("Create Board")]
    void CreateBoard()
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
                    if (cell.mapColor == color)
                    {
                        GameObject obj = Instantiate(cellPf, BoardToWorldPosition(boardPos), Quaternion.identity, worldBoard.transform);
                        obj.GetComponent<Cell>().boardPos = boardPos;
                    }
                }
            }
        }
    }

    [ContextMenu("Clear Board")]
    void ClearBoard()
    {
        GameObject worldBoard = GameObject.Find("WorldBoard");
        var cells = worldBoard.GetComponentsInChildren<Cell>();
        if (cells != null)
        {
            foreach (Cell cell in cells)
            {
                DestroyImmediate(cell.gameObject);
            }
        }
    }

    void InitBoard()
    {
        _board = new Cell[boardShape.height, boardShape.width];
        GameObject worldBoard = GameObject.Find("WorldBoard");
        var cells = worldBoard.GetComponentsInChildren<Cell>();
        if (cells == null || cells.Length == 0)
        {
            CreateBoard();
            cells = worldBoard.GetComponentsInChildren<Cell>();
        }
        
        foreach (Cell cell in cells)
        {
            _board[(int)cell.boardPos.x, (int)cell.boardPos.y] = cell;
        }
        Character.Instance.CurrentCell = _board[0, 0];
        }


    void Awake()
    {
        if (Instance != this)
        {
            DestroyImmediate(this);
            return;
        }
    }
    

    void Start()
    {
        InitBoard();
    }
}
