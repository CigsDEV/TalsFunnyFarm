using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Grid))]
public class GridBasedLighting : MonoBehaviour
{
    public GameObject boundsObject; // The object that determines the bounds for our grid
    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void OnDrawGizmos()
    {
        DrawGridGizmos();
    }

    private void DrawGridGizmos()
    {
        if (grid == null || boundsObject == null) return;

        Vector3 cellSize = grid.cellSize;
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f); // transparent white

        Vector3 objectSize = boundsObject.transform.localScale;

        int gridSizeX = Mathf.CeilToInt(objectSize.x / cellSize.x);
        int gridSizeZ = Mathf.CeilToInt(objectSize.z / cellSize.z);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 cellCenter = new Vector3(x * cellSize.x + cellSize.x * 0.5f, 0, z * cellSize.z + cellSize.z * 0.5f);
                Gizmos.DrawCube(cellCenter, new Vector3(cellSize.x, 0.01f, cellSize.z)); // thin cubes
            }
        }
    }

    [ContextMenu("Update Lighting")]
    void UpdateGridLighting()
    {
        if (boundsObject == null) return;

        Vector3 cellSize = grid.cellSize;
        Vector3 objectSize = boundsObject.transform.localScale;

        int gridSizeX = Mathf.CeilToInt(objectSize.x / cellSize.x);
        int gridSizeZ = Mathf.CeilToInt(objectSize.z / cellSize.z);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Color cellColor = new Color(Random.value, Random.value, Random.value);

                Vector3 cellCenter = new Vector3(x * cellSize.x + cellSize.x * 0.5f, 0, z * cellSize.z + cellSize.z * 0.5f);

                Collider[] renderersInCell = Physics.OverlapBox(cellCenter, new Vector3(cellSize.x * 0.5f, 1000f, cellSize.z * 0.5f));

                foreach (Collider collider in renderersInCell)
                {
                    Renderer rend = collider.GetComponent<Renderer>();
                    if (rend)
                    {
                        Material[] mats = rend.materials;
                        foreach (Material mat in mats)
                        {
                            if (mat.HasProperty("_Color"))
                            {
                                mat.color = cellColor;
                            }
                        }
                    }
                }
            }
        }
    }
}
