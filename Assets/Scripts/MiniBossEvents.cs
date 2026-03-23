using UnityEngine;

public class MiniBossEvents : MonoBehaviour
{
    private MiniBoss miniBoss;

    private void Awake()
    {
        miniBoss = GetComponentInParent<MiniBoss>();
    }

    public void SpawnClaw()
    {
        miniBoss?.SpawnClaw();
    }
}
