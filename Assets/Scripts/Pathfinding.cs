using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int position;
    public Node parent;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public Node(Vector2Int position)
    {
        this.position = position;
    }
}

public class Pathfinding : MonoBehaviour
{
    public GameObject startPosition;
    public GameObject goalPosition;
    public GridManager gridManager;


    [ContextMenu("Find Path")]
    public void ShowPath()
    {
        FindPath(
            gridManager.GetGridPosition(startPosition.transform.position),
            gridManager.GetGridPosition(goalPosition.transform.position)
        );
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        if (gridManager == null) return null;

        List<Node> openList = new List<Node>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Node startNode = new Node(start);
        openList.Add(startNode);

        Dictionary<Vector2Int, Node> allNodes = new Dictionary<Vector2Int, Node>
        {
            { start, startNode }
        };

        while (openList.Count > 0)
        {
            // Get node with lowest fCost
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.position);

            // Reached goal
            if (currentNode.position == goal)
                return RetracePath(currentNode);

            foreach (Vector2Int neighborPos in GetNeighbors(currentNode.position))
            {
                if (!gridManager.isWalkable(neighborPos) || closedSet.Contains(neighborPos))
                    continue;

                int newG = currentNode.gCost + ((neighborPos.x != currentNode.position.x && neighborPos.y != currentNode.position.y) ? 14 : 10);


                Node neighborNode;
                if (allNodes.ContainsKey(neighborPos))
                    neighborNode = allNodes[neighborPos];
                else
                {
                    neighborNode = new Node(neighborPos);
                    allNodes[neighborPos] = neighborNode;
                }

                if (newG < neighborNode.gCost || !openList.Contains(neighborNode))
                {
                    neighborNode.gCost = newG;
                    neighborNode.hCost = Heuristic(neighborPos, goal);
                    neighborNode.parent = currentNode;

                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }

        return null; // no path found
    }

    private List<Vector2Int> RetracePath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        // Manhattan distance for 4-directional movement
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
            };
        foreach (var dir in directions)
        {
            Vector2Int neighborPos = pos + dir;
            if (neighborPos.x >= 0 && neighborPos.y >= 0 &&
                neighborPos.x < gridManager.gridLengthX && neighborPos.y < gridManager.gridLengthY)
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    public void OnDrawGizmos()
    {
        if (gridManager == null) return;

        List<Vector2Int> path = FindPath(
            gridManager.GetGridPosition(startPosition.transform.position),
            gridManager.GetGridPosition(goalPosition.transform.position)
        );

        if (path != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = gridManager.getWorldPosition(path[i]);
                Vector3 end   = gridManager.getWorldPosition(path[i + 1]);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}