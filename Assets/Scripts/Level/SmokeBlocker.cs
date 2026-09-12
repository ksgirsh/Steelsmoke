using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]

public class SmokeBlocker : MonoBehaviour
{
    [SerializeField] SmokeEmitter smoke;
    private BoxCollider2D boxColl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();   

    }

    // Update is called once per frame
    void Update()
    {
        GridCellInsideCollider();
    }

    void GridCellInsideCollider()
    {
        Vector2 min = WorldToGrid(boxColl.bounds.min);
        Vector2 max = WorldToGrid(boxColl.bounds.max);

        int minX = Mathf.Clamp(Mathf.FloorToInt(min.x), 1, smoke.GridSize);
        int maxX = Mathf.Clamp(Mathf.FloorToInt(max.x), 1, smoke.GridSize);
        int minY = Mathf.Clamp(Mathf.FloorToInt(min.y), 1, smoke.GridSize);
        int maxY = Mathf.Clamp(Mathf.FloorToInt(max.y), 1, smoke.GridSize);

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                Vector2 gridWorldPos = GridToWorld(new Vector2(i, j));
                if (boxColl.OverlapPoint(gridWorldPos))
                {
                    smoke.SetBlockedCell(i, j);
                }
            }
        }
    }

    private Vector2 WorldToGrid(Vector2 pos)
    {
        Vector2 localPos = (pos - (Vector2)smoke.transform.position);
        Vector2 newPos = new Vector2(((localPos.x / smoke.CellSize) + (smoke.GridSize / 2f)), ((localPos.y / smoke.CellSize) + (smoke.GridSize / 2f)));
        return newPos;

        //divide by cell size, add half of grid size
    }
    private Vector2 GridToWorld(Vector2 pos)
    {
        Vector2 wrldPos = (pos);
        Vector2 newPos = new Vector2(((wrldPos.x - (smoke.GridSize / 2f)) * smoke.CellSize), ((wrldPos.y - (smoke.GridSize / 2f)) * smoke.CellSize));
        return (newPos + (Vector2)smoke.transform.position);

        //divide by cell size, add half of grid size
    }
}
