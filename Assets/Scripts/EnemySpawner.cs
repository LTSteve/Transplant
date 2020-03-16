using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyController> Enemies;

    public List<float> EnemyWeights;
    public List<float> EnemyWeightGrowths;

    public float ActiveChance = 50f;
    public float ActiveChanceScaling = 100f;

    public float ActivationRange = 200f;
    public float DeactivationRange = 220f;

    public float EnemyLimit = 15f;
    public float EnemyLimitGrowth = 15f;

    private bool activatable = true;

    private void Start()
    {
        var doneness = (100f - WorldGenerator.CurrentBossCountDown) / 100f;

        var currentEnemyLimit = EnemyLimit + doneness * EnemyLimitGrowth;

        ActiveChance += doneness * ActiveChanceScaling;
        if(Enemies.Count >= currentEnemyLimit || Random.value > (ActiveChance / 100f))
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(PlayerController.Instance == null || PlayerController.Instance.WindupIsOver == false)
        {
            return;
        }
        var pDist = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
        if (activatable && pDist < ActivationRange)
        {
            HandleSpawning();
            activatable = false;
        }

        if(!activatable && pDist > DeactivationRange)
        {
            activatable = true;
        }
    }

    private void HandleSpawning()
    {
        var doneness = Mathf.Clamp((100f - WorldGenerator.CurrentBossCountDown) / 100f, 0f, 1f);

        var random = Random.value * 100f;

        var workingvalue = 0f;

        EnemyController chosen = Enemies[0];

        for(var i = 0; i < Enemies.Count; i++)
        {
            workingvalue += EnemyWeights[i] + EnemyWeightGrowths[i] * doneness;

            if(random <= workingvalue)
            {
                chosen = Enemies[i];
                break;
            }
        }

        Instantiate(chosen, transform.position, Quaternion.identity);
    }
}
