using System.Data.SqlTypes;
using UnityEngine;

public class WaveManager : MonoBehaviour
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

    // ---------------------------------------

    public GameObject lineSegmentWavePrefab;

    public GameObject circleWavePrefab;

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
            AddImpulseWithIndex(gridSize / 2, gridSize / 2, -impulse);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddImpulseWithIndex(gridSize / 3, gridSize / 3 * 2, -impulse);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            AddImpulseWithIndex(gridSize / 3 * 2, gridSize / 3, -impulse);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            sphere.transform.position = new Vector3(5.0f, sphereInitHeight, 0);
            sphere.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            Vector3 pos = new Vector3(0, 0, 0);
            Vector3 direction = new Vector3(0.0f, 0.0f, 1.0f);
            CreateLineSegmentWave(pos, direction, 6.0f, -0.2f);
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            Vector3 pos = new Vector3(-30, 0, 0);
            Vector3 vel = new Vector3(0.2f, 0.0f, 0.0f);
            CreateCircleWave(pos, vel, 10.0f, 120.0f);
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

    void AddImpulseWithIndex(int x, int y, float strength)
    {
        if (x >= 1 && x < gridSize - 1 && y >= 1 && y < gridSize - 1)
        {
            current[x, y] += strength;
        }
    }

    public void AddImpulse(Vector3 pos, float strength)
    {
        float range = gridSize / 2.0f - 1.0f;
        if ( -range < pos.x && pos.x < range && -range < pos.y && pos.y < range )
        {
            // 値を補正
            pos.x += gridSize / 2.0f;
            pos.z += gridSize / 2.0f;

            // インデックスを求める
            int x_idx = (int)(pos.x / spacing);
            int z_idx = (int)(pos.z / spacing);

            // その地点でimpulseを加える
            AddImpulseWithIndex(x_idx, z_idx, strength);
        }
    }

    public Vector2 GetVoxelIndexFromPos(Vector3 pos)
    {
        float range = gridSize / 2.0f - 1.0f;
        if ( -range < pos.x && pos.x < range && -range < pos.y && pos.y < range )
        {
            // 値を補正する
            pos.x += gridSize / 2.0f;
            pos.z += gridSize / 2.0f;

            // インデックスを求める
            int x_idx = (int)(pos.x / spacing);
            int z_idx = (int)(pos.z / spacing);

            return new Vector2(x_idx, z_idx);
        }

        // エラーインデックスを返す
        return new Vector2(-1, -1);
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

    void CreateLineSegmentWave(Vector3 pos, Vector3 direction, float length, float speed)
    {
        // 線分を作成する

        // 線分の向きは，波の向きと垂直にする
        direction.y = 0;
        Vector3 lineSegmentDir = Vector3.Cross(direction.normalized, Vector3.up);

        // 線分の始点
        Vector3 startPos = pos + lineSegmentDir * length / 2.0f;
        Vector3 endPos   = pos - lineSegmentDir * length / 2.0f;

        // 線分を作成
        GameObject lineSegmentWave = GameObject.Instantiate(lineSegmentWavePrefab);
        lineSegmentWave.GetComponent<LineSegmentWave>().init(startPos, endPos, lineSegmentDir * speed, this);
    }

    void CreateCircleWave(Vector3 pos, Vector3 velocity, float radius, float angle)
    {
        GameObject circleWave = GameObject.Instantiate(circleWavePrefab);
        circleWave.GetComponent<CircleWave>().init(pos, velocity, radius, angle, this);
    }
}
