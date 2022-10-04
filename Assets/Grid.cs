using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DisplayGridGizmos;
    public LayerMask UnwalkableMask;
    public Vector2 GridWorldSize;
    public TerrainType[] WalkableRegions;
    LayerMask WalkableMask;

    Dictionary<int, int> WalkableRegionsDict = new Dictionary<int, int>();

    public float NodeRadius;
    Node[,] grid;

    float NodeDiameter;
    int GridSizeX, GridSizeY;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Awake()
    {
        NodeDiameter = NodeRadius * 2;
        GridSizeX = Mathf.RoundToInt(GridWorldSize.x/NodeDiameter);
        GridSizeY = Mathf.RoundToInt(GridWorldSize.y/NodeDiameter);

        foreach(TerrainType Region in WalkableRegions)
        {
            WalkableMask.value |= Region.TerrainMask.value;
            WalkableRegionsDict.Add((int)Mathf.Log(Region.TerrainMask.value,2),Region.TerrainPenalty);
        }



        CreateGrid();
    }

    public int MaxSize {
        get{
            return GridSizeX*GridSizeY;
        }
    }

    void CreateGrid(){
        grid = new Node[GridSizeX,GridSizeY];
        Vector3 WorldBottomLeft = transform.position - Vector3.right * GridWorldSize.x/2 - Vector3.forward * GridWorldSize.y/2;

        for (int x=0; x<GridSizeX; x++)
        {
            for(int y = 0; y<GridSizeY;y++)
            {
                Vector3 WorldPoint = WorldBottomLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius);
                bool Walkable = !(Physics.CheckSphere(WorldPoint, NodeRadius, UnwalkableMask));
                int MovementPenalty = 0;

                //Raycast code
                if(Walkable)
                {
                    Ray ray = new Ray(WorldPoint + Vector3.up* 50, Vector3.down);
                    RaycastHit hit;

                    if(Physics.Raycast(ray,out hit, 100, WalkableMask))
                    {
                        WalkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out MovementPenalty);
                    }
                }

                grid[x,y] = new Node(Walkable, WorldPoint,x,y, MovementPenalty);
            }
        }
    }

    public List<Node> GetCellNeighbours(Node node)
    {
        List<Node> Neighbors = new List<Node>();

        for(int x=-1; x<=1;x++)
        {
            for (int y=-1; y<=1;y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                int CheckX = node.GridX+x;
                int CheckY = node.GridY+y;

                if(CheckX >= 0 && CheckX < GridSizeX && CheckY >= 0 && CheckY < GridSizeY)
                {
                    Neighbors.Add(grid[CheckX,CheckY]);
                }
            }
        }
        return Neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 WorldPos)
    {
        float PercentX = (WorldPos.x + GridWorldSize.x/2) / GridWorldSize.x;
        float PercentY = (WorldPos.z + GridWorldSize.y/2) / GridWorldSize.y;
        PercentX = Mathf.Clamp01(PercentX);
        PercentY = Mathf.Clamp01(PercentY);

        int x = Mathf.RoundToInt((GridSizeX-1)*PercentX);
        int y = Mathf.RoundToInt((GridSizeY-1)*PercentY);
        return grid[x,y];
    }


    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x,1, GridWorldSize.y));

            if (grid != null && DisplayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.Walkable) ? Color.white: Color.red;
                    Gizmos.DrawCube(n.WorldPosition, Vector3.one * (NodeDiameter-0.1f));
                }
            }
    }

    //Path Penalty
    [System.Serializable]
    public class TerrainType{
        public LayerMask TerrainMask;
        public int TerrainPenalty;
    }
}
