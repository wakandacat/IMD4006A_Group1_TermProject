using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{
    private Terrain sandTerrain;
    private float[,] initialHeights;

    // Start is called before the first frame update
    void Start()
    {
        // Thank you Dr. Thue for the help with figuring out Unity Terrain!
        // ================================================================

        sandTerrain = GetComponent<Terrain>();

        int width = sandTerrain.terrainData.heightmapResolution;
        int height = sandTerrain.terrainData.heightmapResolution;

        initialHeights = sandTerrain.terrainData.GetHeights(0, 0, width, height);

        // Resetting the terrain back to the default height upon start
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                initialHeights[i, j] = 0.001f;
            }
        }

        /*
        initialHeights[6, 5] -= 0.0005f;
        initialHeights[5, 5] -= 0.00025f;
        initialHeights[4, 5] -= 0.00025f;
        initialHeights[5, 6] -= 0.00025f;
        initialHeights[5, 4] -= 0.00015f;
        initialHeights[4, 4] -= 0.00015f;
        initialHeights[4, 6] -= 0.00015f;
        initialHeights[6, 4] -= 0.00015f;
        initialHeights[6, 6] -= 0.00015f;
        */
        sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void digTerrain(Vector3 crabPos, Quaternion crabRot, float triggerInput)
    {
        // Ensuring that digging will only happen if the crab is on the terrain.
        if (crabPos.x > 0 && crabPos.x < sandTerrain.terrainData.heightmapResolution)
        {
            if (crabPos.z > 0 && crabPos.z < sandTerrain.terrainData.heightmapResolution)
            {
                // Turning the crab's coordinates into an integer to feed it
                // to the initialHeights array. The multiplication by 5.12
                // adjusts the digging position to account for the terrain resolution
                int crabX = (int)(crabPos.x * 5.12f);
                int crabY = (int)(crabPos.z * 5.12f);



                Debug.Log("Digging at: " + crabX + ", " + crabY);

                // Pre-determined hole size (could be adjusted to be more dynamic
                // in later versions)
                initialHeights[crabY, crabX] -= 0.0005f;
                initialHeights[crabY + 1, crabX] -= 0.00025f;
                initialHeights[crabY - 1, crabX] -= 0.00025f;
                initialHeights[crabY, crabX -1] -= 0.00025f;
                initialHeights[crabY + 1, crabX -1] -= 0.00015f;
                initialHeights[crabY - 1, crabX - 1] -= 0.00015f;
                initialHeights[crabY, crabX + 1] -= 0.00015f;
                initialHeights[crabY + 1, crabX + 1] -= 0.00015f;
                initialHeights[crabY - 1, crabX + 1] -= 0.00015f;

                sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);
            }
        }
    }
}
