using System.Collections;
using System.Collections.Generic;
//using TreeEditor;
using UnityEngine;
//using static UnityEditor.Progress;

public class TerrainEditor : MonoBehaviour
{
    private Terrain sandTerrain;
    private float[,] initialHeights;
    private float[,] savedHeights;

    public float perlinStep;
    float perlinNoise;

    private bool digDisabled = true;

    WorldManager worldScript;

    int width;
    int height;

    private void Awake()
    {
        worldScript = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        
        sandTerrain = GetComponent<Terrain>();

        width = sandTerrain.terrainData.heightmapResolution;
        height = sandTerrain.terrainData.heightmapResolution;

        initialHeights = sandTerrain.terrainData.GetHeights(0, 0, width, height);
        savedHeights = sandTerrain.terrainData.GetHeights(0, 0, width, height);

        // Thank you Dr. Thue for the help with figuring out Unity Terrain!
        // ================================================================

        // Resetting the terrain back to the default height upon start
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // initialHeights[i, j] = 0.005f;
                perlinNoise = Mathf.PerlinNoise(i * perlinStep, j * perlinStep);
                initialHeights[i, j] = (perlinNoise * 0.00075f) + 0.0045f;
                savedHeights[i, j] = initialHeights[i, j];
            }
        }


        sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);

        //Vector3 testMound;
        //testMound.x = 10.0f;
        //testMound.y = 0.0f;
        //testMound.z = 46.0f;
        //createMound(testMound);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Thank you Dr. Thue for the help with figuring out Unity Terrain!
        // ================================================================

        // Resetting the terrain back to the default height upon start
        //for (int i = 0; i < width; i++)
        //{
        //    for (int j = 0; j < height; j++)
        //    {
        //        // initialHeights[i, j] = 0.005f;
        //        perlinNoise = Mathf.PerlinNoise(i * perlinStep, j * perlinStep);
        //        initialHeights[i, j] = (perlinNoise * 0.00075f) + 0.0045f;
        //    }
        //}


        //sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);

        //Vector3 testMound;
        //testMound.x = 10.0f;
        //testMound.y = 0.0f;
        //testMound.z = 46.0f;
        //createMound(testMound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void digTerrain(Vector3 crabPos, Quaternion crabRot, float triggerInput)
    {
        // check if the crab is in their home area
        digDisabled = worldScript.getHomeStatus();

        if (digDisabled == false)
        {
            // Ensuring that digging will only happen if the crab is on the terrain.
            if (crabPos.x > 0 && crabPos.x < sandTerrain.terrainData.heightmapResolution)
            {
                if (crabPos.z > 0 && crabPos.z < 70)
                {
                    // Turning the crab's coordinates into an integer to feed it
                    // to the initialHeights array. The multiplication by 5.12
                    // adjusts the digging position to account for the terrain resolution
                    int crabX = (int)(crabPos.x * 5.12f);
                    int crabY = (int)(crabPos.z * 5.12f);

                    triggerInput = triggerInput * 0.00002f;

                    // Debug.Log("Digging at: " + crabX + ", " + crabY);

                    // Pre-determined hole size (could be adjusted to be more dynamic
                    // in later versions)
                    initialHeights[crabY, crabX] -= (triggerInput + 0.000009f);
                    initialHeights[crabY + 1, crabX] -= (triggerInput + 0.000007f);
                    initialHeights[crabY - 1, crabX] -= (triggerInput + 0.000007f);
                    initialHeights[crabY, crabX - 1] -= (triggerInput + 0.000007f);
                    initialHeights[crabY + 1, crabX - 1] -= (triggerInput + 0.000004f);
                    initialHeights[crabY - 1, crabX - 1] -= (triggerInput + 0.000004f);
                    initialHeights[crabY, crabX + 1] -= (triggerInput + 0.000007f);
                    initialHeights[crabY + 1, crabX + 1] -= (triggerInput + 0.000004f);
                    initialHeights[crabY - 1, crabX + 1] -= (triggerInput + 0.000004f);

                    if (initialHeights[crabY, crabX] < 0.0045f)
                    {
                        initialHeights[crabY + 2, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX - 2] -= (triggerInput + 0.000004f);
                    }

                    if (initialHeights[crabY, crabX] < 0.004f)
                    {
                        initialHeights[crabY + 2, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX - 3] -= (triggerInput + 0.000004f);
                    }

                    if (initialHeights[crabY, crabX] < 0.0035f)
                    {
                        initialHeights[crabY + 3, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX - 3] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 4, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 4, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 4, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX - 4] -= (triggerInput + 0.000004f);
                    }

                    if (initialHeights[crabY, crabX] < 0.003f)
                    {
                        initialHeights[crabY + 3, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX - 3] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 4, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 4, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX - 4] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 4, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 4, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX - 4] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 5, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX - 1] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 5, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX - 2] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 1, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX - 5] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 2, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX - 5] -= (triggerInput + 0.000004f);
                    }

                    if (initialHeights[crabY, crabX] < 0.0025f)
                    {
                        initialHeights[crabY + 6, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 6, crabX] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX + 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY, crabX - 6] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 6, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 6, crabX - 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 6, crabX + 1] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 6, crabX - 1] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 6, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 6, crabX - 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 6, crabX + 2] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 6, crabX - 2] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 1, crabX + 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX + 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 1, crabX - 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 1, crabX - 6] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 2, crabX + 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX + 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 2, crabX - 6] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 2, crabX - 6] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 4, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 4, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX - 4] -= (triggerInput + 0.000004f);

                        initialHeights[crabY + 4, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 4, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 3, crabX - 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY + 5, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX + 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX + 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX + 3] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 4, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX - 4] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 3, crabX - 5] -= (triggerInput + 0.000004f);
                        initialHeights[crabY - 5, crabX - 3] -= (triggerInput + 0.000004f);
                    }

                    if (initialHeights[crabY, crabX] < 0.0020f)
                    {

                    }

                        sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);
                }
            }
        }
    }

    public float getTerrainHeight(Vector3 crabPos)
    {
        int crabX = (int)(crabPos.x * 5.12f);
        int crabY = (int)(crabPos.z * 5.12f);
        return initialHeights[crabY, crabX];
    }

    public void resetTerrainHeight()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                initialHeights[i, j] = savedHeights[i, j];
            }
        }
    }

    // createMound is used to push the terrain up at any position where an
    // item has been spawned underground.
    public void createMound(Vector3 itemPos)
    {
        int itemX = (int)(itemPos.x * 5.12f);
        int itemY = (int)(itemPos.z * 5.12f);
        float itemHeight = itemPos.y;

        if (itemHeight < 2.6f)
        {
            initialHeights[itemY, itemX] += 0.0007f;
            initialHeights[itemY, itemX + 1] += 0.0007f;
            initialHeights[itemY, itemX - 1] += 0.0007f;
            initialHeights[itemY + 1, itemX] += 0.0007f;
            initialHeights[itemY - 1, itemX] += 0.0007f;
            initialHeights[itemY + 1, itemX + 1] += 0.0007f;
            initialHeights[itemY + 1, itemX - 1] += 0.00007f;
            initialHeights[itemY - 1, itemX + 1] += 0.00007f;
            initialHeights[itemY - 1, itemX - 1] += 0.00007f;

            initialHeights[itemY + 2, itemX - 1] += 0.0006f;
            initialHeights[itemY + 2, itemX] += 0.0006f;
            initialHeights[itemY + 2, itemX + 1] += 0.0006f;
            initialHeights[itemY - 1, itemX + 2] += 0.0006f;
            initialHeights[itemY, itemX + 2] += 0.0006f;
            initialHeights[itemY + 1, itemX + 2] += 0.0006f;
            initialHeights[itemY - 2, itemX - 1] += 0.0006f;
            initialHeights[itemY - 2, itemX] += 0.0006f;
            initialHeights[itemY - 2, itemX + 1] += 0.0006f;
            initialHeights[itemY - 1, itemX - 2] += 0.0006f;
            initialHeights[itemY, itemX - 2] += 0.0006f;
            initialHeights[itemY + 1, itemX - 2] += 0.0006f;

            initialHeights[itemY + 2, itemX + 2] += 0.0005f;
            initialHeights[itemY - 2, itemX + 2] += 0.0005f;
            initialHeights[itemY + 2, itemX - 2] += 0.0005f;
            initialHeights[itemY - 2, itemX - 2] += 0.0005f;
            initialHeights[itemY + 3, itemX] += 0.0005f;
            initialHeights[itemY - 3, itemX] += 0.0005f;
            initialHeights[itemY, itemX + 3] += 0.0005f;
            initialHeights[itemY, itemX - 3] += 0.0005f;
            initialHeights[itemY + 3, itemX + 1] += 0.0005f;
            initialHeights[itemY + 3, itemX - 1] += 0.0005f;
            initialHeights[itemY - 3, itemX + 1] += 0.0005f;
            initialHeights[itemY - 3, itemX - 1] += 0.0005f;
            initialHeights[itemY + 1, itemX + 3] += 0.0005f;
            initialHeights[itemY - 1, itemX + 3] += 0.0005f;
            initialHeights[itemY + 1, itemX - 3] += 0.0005f;
            initialHeights[itemY - 1, itemX - 3] += 0.0005f;

            Debug.Log("Mound Created.");

            sandTerrain.terrainData.SetHeightsDelayLOD(0, 0, initialHeights);
        }
    }
}
