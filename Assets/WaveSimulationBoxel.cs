using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveSimulationBoxel : MonoBehaviour
{
    public int gridSize = 100;
    // public float spacing = 0.1f;
    public float spacing = 1.0f;
    public float waveSpeed = 1.0f;
    public float timeStep = 0.02f;
    public float damping = 0.996f;

    public float impulse = 1.0f;

    private Mesh mesh;
    private Vector3[] vertices;
    private float[,] prev;
    private float[,] current;
    private float[,] next;

    public GameObject boxelPrefab;
    public GameObject[,] boxels;

    public GameObject spherePrefab;
    private GameObject sphere;
    public float sphereInitHeight = 10;

    void Start()
    {
        boxels = new GameObject[gridSize, gridSize];

        // GenerateMesh();
        GenerateBoxel();

        GameObject sphereObject = GameObject.Instantiate(spherePrefab);
        sphereObject.transform.position = new Vector3(gridSize / 2, sphereInitHeight, gridSize / 2);
        sphere = sphereObject;

        prev = new float[gridSize, gridSize];
        current = new float[gridSize, gridSize];
        next = new float[gridSize, gridSize];

    }

    void Update()
    {
        SimulateWave();
        ApplyToMesh();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddImpulse(gridSize / 2, gridSize / 2, -impulse);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddImpulse(gridSize / 3, gridSize / 3 * 2, -impulse);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            AddImpulse(gridSize / 3 * 2, gridSize / 3, -impulse);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            sphere.transform.position = new Vector3(gridSize / 2, sphereInitHeight, gridSize / 2);
            sphere.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        }
    }

    void SimulateWave()
    {
        float c = waveSpeed;
        float dt = timeStep;
        float dx = spacing;
        float coeff = (c * dt / dx) * (c * dt / dx);

        for (int i = 1; i < gridSize - 1; i++)
        {
            for (int j = 1; j < gridSize - 1; j++)
            {
                next[i, j] = 2 * current[i, j] - prev[i, j] + coeff *
                    (current[i + 1, j] + current[i - 1, j] + current[i, j + 1] + current[i, j - 1] - 4 * current[i, j]);
                next[i, j] *= damping;
            }
        }

        // Swap buffers
        var temp = prev;
        prev = current;
        current = next;
        next = temp;
    }

    void ApplyToMesh()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // int index = i * gridSize + j;
                // Vector3 v = vertices[index];
                // v.y = current[i, j];
                // vertices[index] = v;

                Vector3 pos = boxels[i, j].transform.position;
                pos.y = current[i, j];
                boxels[i, j].transform.position = pos;
            }
        }

        // mesh.vertices = vertices;
        // mesh.RecalculateNormals();
    }

    void AddImpulse(int x, int y, float strength)
    {
        if (x >= 1 && x < gridSize - 1 && y >= 1 && y < gridSize - 1)
        {
            current[x, y] += strength;
        }
    }
    
    void GenerateBoxel()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // 生成
                GameObject boxel = GameObject.Instantiate(boxelPrefab);
                boxels[i, j] = boxel;
                
                // 位置の指定
                boxel.transform.position = new Vector3(i* spacing, 0, j * spacing);
            }
        }
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[gridSize * gridSize];
        int[] triangles = new int[(gridSize - 1) * (gridSize - 1) * 6];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                vertices[i * gridSize + j] = new Vector3(i * spacing, 0, j * spacing);
            }
        }

        int t = 0;
        for (int i = 0; i < gridSize - 1; i++)
        {
            for (int j = 0; j < gridSize - 1; j++)
            {
                int index = i * gridSize + j;

                triangles[t++] = index;
                triangles[t++] = index + gridSize;
                triangles[t++] = index + gridSize + 1;

                triangles[t++] = index;
                triangles[t++] = index + gridSize + 1;
                triangles[t++] = index + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
