using UnityEngine;
using UnityEngine.UIElements;

public class WaveSimulationBoxel : MonoBehaviour
{
    public int gridSize = 100;
    public float spacing = 1.0f;
    public float waveSpeed = 1.0f;
    public float timeStep = 0.02f;
    public float damping = 0.996f;
    public float impulse = 1.0f;

    // ---------------------------------------

    private float[,] prev;
    private float[,] current;
    private float[,] next;

    // ---------------------------------------

    private Vector3 waveBasePos; // voxelを配置する基準位置

    // ---------------------------------------

    public GameObject boxelPrefab;
    public GameObject[,] voxels;

    public GameObject spherePrefab;
    private GameObject sphere;
    public float sphereInitHeight = 10;

    void Start()
    {
        voxels = new GameObject[gridSize, gridSize];

        waveBasePos = new Vector3(-gridSize / 2.0f, 0.0f, -gridSize / 2.0f);

        GenerateVoxel();

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
        ApplyToVoxels();

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

    void ApplyToVoxels()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 pos = voxels[i, j].transform.position;
                pos.y = current[i, j];
                voxels[i, j].transform.position = pos;
            }
        }
    }

    void AddImpulse(int x, int y, float strength)
    {
        if (x >= 1 && x < gridSize - 1 && y >= 1 && y < gridSize - 1)
        {
            current[x, y] += strength;
        }
    }
    
    void GenerateVoxel()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // 生成
                GameObject voxel = GameObject.Instantiate(boxelPrefab);
                voxels[i, j] = voxel;
                
                // 位置の指定
                voxel.transform.position = waveBasePos + new Vector3(i* spacing, 0, j * spacing);
            }
        }
    }
}
