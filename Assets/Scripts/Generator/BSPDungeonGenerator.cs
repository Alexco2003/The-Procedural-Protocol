using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonGenerator : MonoBehaviour
{
    public enum TileType { Empty, Floor, Wall }

    [Header("Dungeon Settings")]
    public int gridWidth = 40;
    public int gridDepth = 40;

    [Header("BSP Settings")]
    public int minPartitionSize = 10;
    public int minRoomSize = 4;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    private TileType[,] grid;

    public List<Vector2Int> roomCenters = new List<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        grid = new TileType[gridWidth, gridDepth];
        roomCenters.Clear();

        List<Leaf> leaves = new List<Leaf>();
        Leaf root = new Leaf(0, 0, gridWidth, gridDepth, minPartitionSize, minRoomSize);
        leaves.Add(root);

        bool didSplit = true;
        while (didSplit)
        {
            didSplit = false;
            List<Leaf> newLeaves = new List<Leaf>();
            foreach (Leaf l in leaves)
            {
                if (l.leftChild == null && l.rightChild == null)
                {
                    if (l.width > minPartitionSize * 2 || l.height > minPartitionSize * 2 || Random.value > 0.2f)
                    {
                        if (l.Split())
                        {
                            newLeaves.Add(l.leftChild);
                            newLeaves.Add(l.rightChild);
                            didSplit = true;
                        }
                        else newLeaves.Add(l);
                    }
                    else newLeaves.Add(l);
                }
                else
                {
                    newLeaves.Add(l.leftChild);
                    newLeaves.Add(l.rightChild);
                }
            }
            leaves = newLeaves;
        }

        root.CreateRooms();

        foreach (Leaf l in leaves)
        {
            if (l.room != null)
            {
                int cx = l.room.x + l.room.width / 2;
                int cy = l.room.y + l.room.height / 2;
                roomCenters.Add(new Vector2Int(cx, cy));

                for (int x = l.room.x; x < l.room.x + l.room.width; x++)
                {
                    for (int y = l.room.y; y < l.room.y + l.room.height; y++)
                    {
                        grid[x, y] = TileType.Floor;
                    }
                }
            }
        }

        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            ConnectWithAStar(roomCenters[i], roomCenters[i + 1]);
        }

        InstantiateDungeon();
    }

    void ConnectWithAStar(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> openSet = new List<Vector2Int> { start };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        gScore[start] = 0;
        Vector2Int[] neighbors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            foreach (Vector2Int pos in openSet)
            {
                float fCurrent = gScore[current] + Mathf.Abs(current.x - end.x) + Mathf.Abs(current.y - end.y);
                float fPos = gScore.ContainsKey(pos) ? gScore[pos] + Mathf.Abs(pos.x - end.x) + Mathf.Abs(pos.y - end.y) : float.MaxValue;
                if (fPos < fCurrent) current = pos;
            }

            if (current == end)
            {
                while (cameFrom.ContainsKey(current))
                {
                    if (grid[current.x, current.y] == TileType.Empty)
                        grid[current.x, current.y] = TileType.Floor;
                    current = cameFrom[current];
                }
                return;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int dir in neighbors)
            {
                Vector2Int neighbor = current + dir;

                if (neighbor.x < 1 || neighbor.x >= gridWidth - 1 || neighbor.y < 1 || neighbor.y >= gridDepth - 1)
                    continue;

                if (closedSet.Contains(neighbor)) continue;

                float moveCost = (grid[neighbor.x, neighbor.y] == TileType.Empty) ? 3f : 1f;
                float tentativeG = gScore[current] + moveCost;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
    }

    void InstantiateDungeon()
    {
        GameObject dungeonParent = new GameObject("DungeonGenerated");

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridDepth; y++)
            {
                TileType type = grid[x, y];
                Vector3 worldPos = new Vector3(x, 0, y);

                if (type == TileType.Floor)
                {
                    Instantiate(floorPrefab, worldPos, Quaternion.identity, dungeonParent.transform);
                }
                else if (type == TileType.Empty)
                {
                    if (IsAdjacentToFloor(x, y))
                    {
                        grid[x, y] = TileType.Wall;
                        Vector3 wallPos = new Vector3(x, 1f, y);
                        Instantiate(wallPrefab, wallPos, Quaternion.identity, dungeonParent.transform);
                    }
                }
            }
        }
    }

    bool IsAdjacentToFloor(int x, int y)
    {
        if (x > 0 && grid[x - 1, y] == TileType.Floor) return true;
        if (x < gridWidth - 1 && grid[x + 1, y] == TileType.Floor) return true;
        if (y > 0 && grid[x, y - 1] == TileType.Floor) return true;
        if (y < gridDepth - 1 && grid[x, y + 1] == TileType.Floor) return true;

        if (x > 0 && y > 0 && grid[x - 1, y - 1] == TileType.Floor) return true;
        if (x < gridWidth - 1 && y > 0 && grid[x + 1, y - 1] == TileType.Floor) return true;
        if (x > 0 && y < gridDepth - 1 && grid[x - 1, y + 1] == TileType.Floor) return true;
        if (x < gridWidth - 1 && y < gridDepth - 1 && grid[x + 1, y + 1] == TileType.Floor) return true;

        return false;
    }

    class Rect
    {
        public int x, y, width, height;
        public Rect(int x, int y, int w, int h) { this.x = x; this.y = y; width = w; height = h; }
    }

    class Leaf
    {
        public int x, y, width, height;
        public int minPartitionSize, minRoomSize;
        public Leaf leftChild, rightChild;
        public Rect room;

        public Leaf(int x, int y, int w, int h, int minP, int minR)
        {
            this.x = x; this.y = y; width = w; height = h;
            minPartitionSize = minP; minRoomSize = minR;
        }

        public bool Split()
        {
            if (leftChild != null || rightChild != null) return false;

            bool splitH = Random.value > 0.5f;
            if (width > height && (float)width / height >= 1.25f) splitH = false;
            else if (height > width && (float)height / width >= 1.25f) splitH = true;

            int max = (splitH ? height : width) - minPartitionSize;
            if (max <= minPartitionSize) return false;

            int split = Random.Range(minPartitionSize, max);

            if (splitH)
            {
                leftChild = new Leaf(x, y, width, split, minPartitionSize, minRoomSize);
                rightChild = new Leaf(x, y + split, width, height - split, minPartitionSize, minRoomSize);
            }
            else
            {
                leftChild = new Leaf(x, y, split, height, minPartitionSize, minRoomSize);
                rightChild = new Leaf(x + split, y, width - split, height, minPartitionSize, minRoomSize);
            }
            return true;
        }

        public void CreateRooms()
        {
            if (leftChild != null || rightChild != null)
            {
                if (leftChild != null) leftChild.CreateRooms();
                if (rightChild != null) rightChild.CreateRooms();
            }
            else
            {
                int roomW = Random.Range(minRoomSize, width - 2);
                int roomH = Random.Range(minRoomSize, height - 2);
                int roomX = Random.Range(1, width - roomW - 1);
                int roomY = Random.Range(1, height - roomH - 1);
                room = new Rect(x + roomX, y + roomY, roomW, roomH);
            }
        }
    }
}