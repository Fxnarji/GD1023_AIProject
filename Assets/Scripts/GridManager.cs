using UnityEngine;

public class GridManager : MonoBehaviour
{

#region inputs
    public float cellSize = 1f;
    public int gridLengthX = 30;
    public int gridLengthY = 11;
    public int gridDisplayHeight = 1;
    public bool displayGrid = true;
    public GameObject gridOriginObject;
    public GameObject Walls;
    public GameObject Towers;


    #endregion


    public class CellState
    {
        public bool isWalkable = true;
        public bool underAttack = false;
    }
    public CellState[,] CellStateArray;

    [ContextMenu("Update Arrays")]
    public void Start()
    {
        UpdateIsWalkable();
    }
    public Vector3 getWorldPosition(Vector2Int GridPos)
    {
        Vector3 gridOrigin = gridOriginObject.transform.position;

        Vector3 worldPos = new Vector3(
            gridOrigin.x + (GridPos.x * cellSize) + cellSize / 2,       //x
            gridDisplayHeight,                                          //y       
            gridOrigin.z + (GridPos.y * cellSize) + cellSize / 2);      //z

        return worldPos;
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 gridOrigin = gridOriginObject.transform.position;
        int x = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - gridOrigin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    public bool isWalkable(Vector2Int gridPos)
    {
        return CellStateArray[gridPos.x, gridPos.y].isWalkable;
    }

    public bool isUnderAttack(Vector2Int gridPos)
    {
        return CellStateArray[gridPos.x, gridPos.y].underAttack;
    }

    public void UpdateIsWalkable()
    {
        CellStateArray = new CellState[gridLengthX, gridLengthY];

        for (int x = 0; x < gridLengthX; x++)
        {
            for (int y = 0; y < gridLengthY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = getWorldPosition(gridPos);

                // Create the cell first
                CellStateArray[x, y] = new CellState();

                // Check for obstacles
                Collider[] colliders = Physics.OverlapBox(worldPos, new Vector3(cellSize / 2, cellSize / 2, cellSize / 2));
                bool walkable = true;
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.transform.IsChildOf(Walls.transform))
                    {
                        walkable = false;
                        break;
                    }
                }

                CellStateArray[x, y].isWalkable = walkable;
                

                bool attacked = false;
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.transform.IsChildOf(Towers.transform))
                    {
                        attacked = true;
                        break;
                    }
                }

                CellStateArray[x, y].underAttack = attacked;
            }
        }
    }


    /// 
    /// Debug Stuff from here
    /// 
    private void drawGrid()
    {
        for (int x = 0; x < gridLengthX; x++)
        {
            for (int y = 0; y < gridLengthY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (isUnderAttack(gridPos))
                    Gizmos.color = Color.yellow;
                else if (isWalkable(gridPos))
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red; // optional for blocked cells



                Gizmos.DrawWireSphere(getWorldPosition(gridPos), 0.25f);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (displayGrid == false) return;
        drawGrid();

    }
}
