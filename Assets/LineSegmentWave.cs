using UnityEngine;

public class LineSegmentWave : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 velocity;

    public WaveManager waveManager;
    public float splitLengthUnit = 1.0f; // 線分をどの長さで区切って波を発生させるか

    // --------------------------------------------------------------------

    // 線分を一定単位で区切って，波を発生させる
    Vector3 impulsePoint;
    float waveMaxLength;
    float waveLength = 0;
    Vector3 direction;

    // 波を発生させたindex
    int pre_x = 0;
    int pre_z = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 線分を一定単位で区切って，波を発生させる
        impulsePoint = startPos;
        waveMaxLength = Vector3.Distance(startPos, endPos);
        waveLength = 0;
        direction = Vector3.Normalize(endPos - startPos);
    }

    // Update is called once per frame
    void Update()
    {
        if(splitLengthUnit <= 0)
        {
            return;
        }

        startPos += velocity;
        endPos += velocity;

        gameObject.transform.position = startPos;

        // 線分を一定単位で区切って，波を発生させる
        Vector3 impulsePoint = startPos;
        float waveMaxLength = Vector3.Distance(startPos, endPos);
        float waveLength = 0;
        Vector3 direction = Vector3.Normalize(endPos - startPos);
        Debug.Log(direction);

        // 波を発生させたindex
        Vector2 pre_impulse_index = new Vector2(-1, -1);

        int count = 0;

        while(waveLength < waveMaxLength)
        {
            Debug.Log(count++);
            Debug.Log(impulsePoint);
            Debug.Log(waveLength);

            // 波の発生ポイントが重なっていないかの確認
            Vector2 impulse_index = waveManager.GetVoxelIndexFromPos(impulsePoint);

            // 発生ポイントがボクセル内かどうか
            if(impulse_index.x == -1 && impulse_index.y == -1)
            {
                // 次のポイントへ更新
                impulsePoint += direction * splitLengthUnit;
                waveLength += splitLengthUnit;
                continue;
            }

            // 前回の発生ポイントと同じかどうか
            if(pre_impulse_index == impulse_index)
            {
                // 次のポイントへ更新
                impulsePoint += direction * splitLengthUnit;
                waveLength += splitLengthUnit;
                continue;
            }

            waveManager.AddImpulse(impulsePoint, 0.4f);
            pre_impulse_index = impulse_index;
            
            // 次のポイントへ更新
            impulsePoint += direction * splitLengthUnit;
            waveLength += splitLengthUnit;
        }        
    }

    public void init(Vector3 startPos, Vector3 endPos, Vector3 vel, WaveManager waveManager)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.velocity = vel;
        this.waveManager = waveManager;
    }
}
