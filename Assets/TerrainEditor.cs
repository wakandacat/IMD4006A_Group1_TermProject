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

        initialHeights[6, 5] -= 0.0005f;
        initialHeights[5, 5] -= 0.00025f;
        initialHeights[4, 5] -= 0.00025f;
        initialHeights[5, 6] -= 0.00025f;
        initialHeights[5, 4] -= 0.00015f;
        initialHeights[4, 4] -= 0.00015f;
        initialHeights[4, 6] -= 0.00015f;
        initialHeights[6, 4] -= 0.00015f;
        initialHeights[6, 6] -= 0.00015f;

        sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void digTerrain(float crabX, float crabY, float crabRotation, float triggerInput)
    {
        
    }
}
