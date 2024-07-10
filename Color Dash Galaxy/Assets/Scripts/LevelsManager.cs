using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    float junkTime = 30f, bossAppearingTime = 150f; // Boss will spwan at 2:30 after level begun

    [SerializeField] Dictionary<string, Dictionary<string, float>> levelParameters = new Dictionary<string, Dictionary<string, float>>()
    {
        ["Easy"] = new Dictionary<string, float>
        {
            ["minJunkPeriod"] = 10,
            ["maxJunkPeriod"] = 20,
            ["minEnemyShipPeriod"] = 10,
            ["maxEnemyShipPeriod"] = 20,
            ["minEnemyShipProbability"] = 10,
            ["maxEnemyShipProbability"] = 20,
            ["minJunkSpd"] = 10,
            ["maxJunkSpd"] = 20,
            ["minEnemyShipSpd"] = 10,
            ["maxEnemyShipSpd"] = 20,
            ["minEnemyShootingPeriod"] = 10,
            ["maxEnemyShootingPeriod"] = 20,
            ["bossHP"] = 100,
            ["bossShootingPeriod"] = 100,
        },
        ["Normal"] = new Dictionary<string, float>
        {
            ["minJunkPeriod"] = 10,
            ["maxJunkPeriod"] = 20,
            ["minEnemyShipPeriod"] = 10,
            ["maxEnemyShipPeriod"] = 20,
            ["minEnemyShipProbability"] = 10,
            ["maxEnemyShipProbability"] = 20,
            ["minJunkSpd"] = 10,
            ["maxJunkSpd"] = 20,
            ["minEnemyShipSpd"] = 10,
            ["maxEnemyShipSpd"] = 20,
            ["minEnemyShootingPeriod"] = 10,
            ["maxEnemyShootingPeriod"] = 20,
            ["bossHP"] = 100,
            ["bossShootingPeriod"] = 100,
        },
        ["Hard"] = new Dictionary<string, float>
        {
            ["minJunkPeriod"] = 10,
            ["maxJunkPeriod"] = 20,
            ["minEnemyShipPeriod"] = 10,
            ["maxEnemyShipPeriod"] = 20,
            ["minEnemyShipProbability"] = 10,
            ["maxEnemyShipProbability"] = 20,
            ["minJunkSpd"] = 10,
            ["maxJunkSpd"] = 20,
            ["minEnemyShipSpd"] = 10,
            ["maxEnemyShipSpd"] = 20,
            ["minEnemyShootingPeriod"] = 10,
            ["maxEnemyShootingPeriod"] = 20,
            ["bossHP"] = 100,
            ["bossShootingPeriod"] = 100,
        },
    };

    // Start is called before the first frame update
    void Start()
    {

    }
}
