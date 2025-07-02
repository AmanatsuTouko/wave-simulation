using UnityEngine;

public class CircleWave : MonoBehaviour
{
    public Vector3 pos;
    public Vector3 vel;
    public float angle; // degree

    public WaveManager waveManager;
    public float splitAngleUnit = 10.0f; // 線分をどの角度で区切って波を発生させるか

    private float waveRadius = 10.0f;
    private float waveStrength = 0.2f;

    // --------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(splitAngleUnit <= 0)
        {
            return;
        }

        pos += vel;
        gameObject.transform.position = pos;

        // 円を一定単位で区切って，波を発生させる
        float start_angle = -angle * 0.5f;
        float end_angle = angle * 0.5f;
        Vector2 pre_impulse_index = new Vector2(-1, -1);

        Debug.Log(start_angle);
        Debug.Log(end_angle);

        for(float theta = start_angle; theta <= end_angle; theta += splitAngleUnit)
        {
            // 円を構成する点を求める
            Vector3 impusle_point = pos + new Vector3(Mathf.Cos(Mathf.Deg2Rad * theta) * waveRadius, 0, Mathf.Sin(Mathf.Deg2Rad * theta) * waveRadius);
            // 波の発生ボクセルのindex 
            Vector2 impulse_index = waveManager.GetVoxelIndexFromPos(impusle_point);

            // 発生ポイントがボクセル内かどうか
            if(impulse_index.x == -1 && impulse_index.y == -1)
            {
                continue;
            }
            // 前回の発生ポイントと同じかどうか
            if(pre_impulse_index == impulse_index)
            {
                continue;
            }

            // 波に高さの変位を加える
            waveManager.AddImpulse(impusle_point, waveStrength);
            pre_impulse_index = impulse_index;
        }

        // Vector3 impulsePoint = startPos;
        // float waveMaxLength = Vector3.Distance(startPos, endPos);
        // float waveLength = 0;
        // Vector3 direction = Vector3.Normalize(endPos - startPos);
        // Debug.Log(direction);

        // // 波を発生させたindex
        // Vector2 pre_impulse_index = new Vector2(-1, -1);

        // int count = 0;

        // while(waveLength < waveMaxLength)
        // {
        //     Debug.Log(count++);
        //     Debug.Log(impulsePoint);
        //     Debug.Log(waveLength);

        //     // 波の発生ポイントが重なっていないかの確認
        //     Vector2 impulse_index = waveManager.GetVoxelIndexFromPos(impulsePoint);

        //     // 発生ポイントがボクセル内かどうか
        //     if(impulse_index.x == -1 && impulse_index.y == -1)
        //     {
        //         // 次のポイントへ更新
        //         impulsePoint += direction * splitLengthUnit;
        //         waveLength += splitLengthUnit;
        //         continue;
        //     }

        //     // 前回の発生ポイントと同じかどうか
        //     if(pre_impulse_index == impulse_index)
        //     {
        //         // 次のポイントへ更新
        //         impulsePoint += direction * splitLengthUnit;
        //         waveLength += splitLengthUnit;
        //         continue;
        //     }

        //     waveManager.AddImpulse(impulsePoint, 0.4f);
        //     pre_impulse_index = impulse_index;
            
        //     // 次のポイントへ更新
        //     impulsePoint += direction * splitLengthUnit;
        //     waveLength += splitLengthUnit;
        // }        
    }

    public void init(Vector3 pos, Vector3 vel,  float radius, float angle, WaveManager waveManager)
    {
        this.pos = pos;
        this.vel = vel;
        this.waveRadius = radius;
        this.angle = angle;
        this.waveManager = waveManager;
    }
}
