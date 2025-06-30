using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveSimulation : MonoBehaviour
{
    public int gridSize = 100;
    public float spacing = 0.1f;
    public float waveSpeed = 1.0f;
    public float timeStep = 0.02f;
    public float damping = 0.996f;

    private Mesh mesh;
    private Vector3[] vertices;
    private float[,] prev;
    private float[,] current;
    private float[,] next;

    void Start()
    {
        GenerateMesh();

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
            AddImpulse(gridSize / 2, gridSize / 2, 0.5f);
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
                int index = i * gridSize + j;
                Vector3 v = vertices[index];
                v.y = current[i, j];
                vertices[index] = v;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void AddImpulse(int x, int y, float strength)
    {
        if (x >= 1 && x < gridSize - 1 && y >= 1 && y < gridSize - 1)
        {
            current[x, y] += strength;
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
