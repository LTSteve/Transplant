using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator Instance = null;

    public List<Transform> WorldBits;

    public Transform RootBit;

    public static float CurrentBossCountDown = 100f;

    private Transform[,] currentWorld;

    private Vector3 center;

    private void Start()
    {
        Instance = this;

        currentWorld = new Transform[5, 5];

        center = transform.position;

        StartCoroutine(_generateWorld());
    }
    private IEnumerator _generateWorld()
    {

        var center = Instantiate(RootBit, Vector3.zero, Quaternion.identity, transform);
        currentWorld[2, 2] = center;

        for(var i = 0; i < 5; i++)
        {
            for(var j = 0; j < 5; j++)
            {
                if(i == 2 && j == 2)
                {
                    continue;//Skip center
                }

                var nextBit = WorldBits[Random.Range(0, WorldBits.Count)];

                currentWorld[i, j] = Instantiate(nextBit, new Vector3(i * 200 - 400, 0, j * 200 - 400), Quaternion.identity, transform);
            }

            yield return null;
        }
        _unlockPlayer();
    }

    private void _unlockPlayer()
    {

    }

    private List<Transform> toCleanUp;
    private bool generating = false;

    private void Update()
    {
        if(PlayerController.Instance == null)
        {
            return;
        }

        var playerTransform = PlayerController.Instance.transform;

        if(!generating && Vector3.Distance(center, playerTransform.position) > 200)
        {
            Debug.Log("Let's go!");
            generating = true;

            var playerX = (int)Mathf.Round((playerTransform.position.x - center.x) / 200f);
            var playerY = (int)Mathf.Round((playerTransform.position.z - center.z) / 200f);

            var newWorld = new Transform[5, 5];

            toCleanUp = new List<Transform>();

            for (var i = 0; i < 5; i++)
            {
                for(var j = 0; j < 5; j++)
                {
                    var newI = i - playerX;
                    var newJ = j - playerY;

                    if(newI >= 5 || newI < 0 || newJ >= 5 || newJ < 0)
                    {
                        toCleanUp.Add(currentWorld[i, j]);
                    }
                    else
                    {
                        newWorld[newI, newJ] = currentWorld[i, j];
                    }
                }
            }

            center = new Vector3(center.x + playerX * 200, 0, center.z + playerY * 200);

            StartCoroutine(_regenerateBits(newWorld));
        }
    }

    private IEnumerator _regenerateBits(Transform[,] newWorld)
    {
        var generated = 0;
        for(var i = 0; i < 5; i++)
        {
            for(var j = 0; j < 5; j++)
            {
                if(newWorld[i,j] == null)
                {
                    var nextBit = WorldBits[Random.Range(0, WorldBits.Count)];

                    newWorld[i, j] = Instantiate(nextBit, new Vector3(i * 200 - 400 + center.x, 0, j * 200 - 400 + center.z), Quaternion.identity, transform);

                    generated++;
                }

                if(generated >= 5)
                {
                    generated = 0;
                    yield return null;
                }
            }
        }

        currentWorld = newWorld;
        generating = false;

        if(generated >= 4)
        {
            yield return null;
        }

        foreach(var toClean in toCleanUp)
        {
            if(toClean == null)
            {
                continue;
            }
            Destroy(toClean.gameObject);
        }
    }
}
