using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/SebLague/Procedural-Landmass-Generation/blob/master/Proc%20Gen%20E03/Assets/Scripts/Noise.cs


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Grid : MonoBehaviour
{

    public int xSize, zSize;

    private Mesh mesh;

    private Vector3[] vertices;

    //public GameObject camera;

    public GameObject player;

    public GameObject tree;

    public GameObject bush;

    public int octaves;

    public int persistence;

    public float lacunarity;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = xSize / 2f;
        float halfHeight = zSize / 2f;

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0, z = 0; z <= zSize; z++)
        { 
            for (int x = 0; x <= xSize; x++, i++)
            {

                for (int o = 0; 0 < octaves; o++)
                {
                    float sampleX = (x / halfWidth) * frequency;
                    float sampleZ = (z / halfHeight) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    amplitude *= persistence;
                    frequency *= lacunarity;

                    vertices[i] = new Vector3(x, noiseHeight, z);
                    uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                }
                

                //float y = Mathf.PerlinNoise(x * .1f, z * .1f) * 2f;
               // vertices[i] = new Vector3(x, y, z);
               // uv[i] = new Vector2((float)x / xSize, (float)z / zSize);

                int rand = Random.Range(1, 200);

                if (rand == 2)
                {

                    Instantiate(tree, vertices[i], Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), this.gameObject.transform);
                } else if (rand == 3 || rand == 4)
                {
                    Instantiate(bush, vertices[i], Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), this.gameObject.transform);
                }
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            { 

                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        if (xSize >= zSize)
        {
            //camera.transform.position = new Vector3(xSize / 2, (xSize), xSize / 2);
            Instantiate(player, new Vector3(xSize / 2, 20, xSize / 2), Quaternion.identity);
        } else
        {
            //camera.transform.position = new Vector3(zSize / 2, (zSize), zSize / 2);
            Instantiate(player, new Vector3(zSize / 2, 20, zSize / 2), Quaternion.identity);
        }


        //GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;

        //UnityEditor.Unwrapping.GenerateSecondaryUVSet(mesh);
    }
}
