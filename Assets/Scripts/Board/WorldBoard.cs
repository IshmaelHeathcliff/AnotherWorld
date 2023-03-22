using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board
{
    public class WorldBoard : SerializedMonoBehaviour
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

        const float Dist = 1.8f;
        const float UpdateTime = 5f;

        public Texture2D boardShape;

        public CellTypeSO cellTypeColor;

        Board _board;

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
                _h = BoardDistance(Character.Character.Instance.CurrentCell.boardPos, cell.boardPos);
            
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

        public class Board
        {
            Cell[,] _board;
            public int Width;
            public int Height;

            public Board(int width, int height)
            {
                Width = width;
                Height = height;
                _board = new Cell[width, height];
            }

            public bool CheckIndex(int x, int y)
            {
                var h = x + Height / 2;
                var w = y + Width / 2;
                
                if (h < 0 || h >= Height || w < 0 || w >= Width)
                {
                    return false;
                }

                return this[x, y] != null;
            }

            public Cell this[int x, int y]
            {
                get => _board[x + Height / 2, y + Width / 2];
                set => _board[x + Height / 2, y + Width / 2] = value;
            }
        }

        public static Vector3 BoardToWorldPosition(Vector3Int boardPos)
        {
            var xDirection = new Vector3(boardPos.x * Dist, 0, 0);
            var zDirection = new Vector3(boardPos.y * Dist / 2, 0, boardPos.y * Dist * Mathf.Sqrt(3) / 2);
            return xDirection + zDirection;
        }
        
        static float BoardDistance(Vector3 posA, Vector3 posB)
        {
            Vector3 dist = posA - posB;
            return (Mathf.Abs(dist.x) + Mathf.Abs(dist.y) + Mathf.Abs(dist.z)) / 2;
        }

        public bool FindPath(Vector3 targetPos)
        {
            Cell currentCell = Character.Character.Instance.CurrentCell;
            var directDistance = BoardDistance(targetPos, currentCell.boardPos);
            if (directDistance > 2 * Character.Character.Instance.maxDistance)
            {
                Character.Character.Instance.Path = new Queue<Cell>();
                return false;
            }
        
            var open = new List<Point>();
            var closed = new List<Point>();
        
            open.Add(new Point(currentCell, null, true));

            while (true)
            {
                if (open.Count == 0)
                {
                    Character.Character.Instance.Path = new Queue<Cell>();
                    // Debug.Log("can't find path");
                    return false;
                }
            
                Point minPoint = open[0];
                foreach (Point point in open.Where(point => point.F < minPoint.F))
                {
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
                        if ( i == j || !_board.CheckIndex(x + i, y + j)) continue;
                    
                        Cell nextCell = _board[x + i, y + j];
                        var nextPoint = new Point(nextCell, minPoint);

                        if (nextCell.boardPos == targetPos)
                        {
                            var maxCount = Character.Character.Instance.maxDistance;
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

                            Character.Character.Instance.Path = reachablePath;

                            return true;
                        }
                        
                        if (BoardDistance(nextCell.boardPos, currentCell.boardPos) >
                            2 * Character.Character.Instance.maxDistance)
                        {
                            continue;
                        }

                        if (nextCell.canPass && !openCells.Contains(nextCell) && !closedCells.Contains(nextCell))
                        {
                            open.Add(nextPoint);
                        }

                    }
                }
            }

        }

        [ContextMenu("Test")]
        void Test()
        {
        }

        [ContextMenu("Create Board")]
        void CreateBoardInEditor()
        {
            StartCoroutine(CreateBoard());
        }
        
        IEnumerator CreateBoard()
        {
            ClearBoard();
            var height = boardShape.height;
            var width = boardShape.width;
            
            GameObject worldBoard = GameObject.Find("WorldBoard");
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var x = i - height / 2;
                    var y = j - width / 2;
                    Color color = boardShape.GetPixel(i, j);
                    var boardPos = new Vector3Int(x, y, -x-y);
                    GameObject cell = CellFactory.Instance.CreatCell(cellTypeColor.cellColor[color]);
                        
                    if (cell is null)
                    {
                        continue;
                    }
                        
                    GameObject obj = Instantiate(cell, BoardToWorldPosition(boardPos), Quaternion.identity, worldBoard.transform);
                    obj.GetComponent<Cell>().boardPos = boardPos;
                }
                
                if( i % (int)(height * Time.deltaTime / UpdateTime + 1) == 0)
                    yield return null;
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

        public IEnumerator InitBoard()
        {
            var height = boardShape.height;
            var width = boardShape.width;
            _board = new Board(width, height);
            GameObject worldBoard = GameObject.Find("WorldBoard");
            var cells = worldBoard.GetComponentsInChildren<Cell>();

            if (cells == null || cells.Length == 0)
            {
                while (!CellFactory.Instance.IsAssetsLoaded)
                {
                    yield return null;
                }
                
                yield return StartCoroutine(CreateBoard());
                cells = worldBoard.GetComponentsInChildren<Cell>();
            }
            
            foreach (Cell cell in cells)
            {
                _board[cell.boardPos.x, cell.boardPos.y] = cell;
            }

            var character = Character.Character.Instance;
            var characterInitialPosition = (Vector3Int)character.initialPosition;
            character.CurrentCell = _board[characterInitialPosition.x, characterInitialPosition.y];
            character.InitCharacter();
        }


        void Awake()
        {
            if (Instance != this)
            {
                DestroyImmediate(this);
                return;
            }
        }
    }
}
