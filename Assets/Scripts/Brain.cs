using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Brain
{
    public float WanderAttentionSpan = 3f;

    private float wanderTime = 0f;

    private Vector3 lookPoint = Vector3.one;

    public Vector3 GetWanderPoint()
    {
        return lookPoint;
    }

    public void Think(float delta)
    {
        wanderTime -= delta;

        if(wanderTime <= 0)
        {
            var r = new Random();

            wanderTime = WanderAttentionSpan;

            lookPoint = new Vector3(Random.value * 2f - 1f, 0f, Random.value * 2f - 1f);

            if(lookPoint.magnitude < 0.5f)
            {
                lookPoint = Vector3.zero;
            }

            lookPoint = lookPoint == Vector3.zero ? lookPoint : lookPoint.normalized;
        }
    }
}

