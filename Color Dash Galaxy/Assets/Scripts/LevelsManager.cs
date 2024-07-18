using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] spacejunks;
    [SerializeField] GameObject[] enemySpaceships;

    [SerializeField] Vector4 enemiesAppearLinesOnESWNsides, randomizedEnemiesPositionsLeftRightUpDown;

    int enemyAppearSide;
    Vector2 enemyAppearPosition;

    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard
    }
    public GameDifficulty gameDifficulty;

    public Dictionary<string, Dictionary<string, float>> levelParameters = new Dictionary<string, Dictionary<string, float>>()
    {
        ["Easy"] = new Dictionary<string, float>
        {
            ["junkTime"] = 30,
            ["bossAppearingTime"] = 150, // Boss will spawn at 2:30 after level begun
            ["minJunkPeriod"] = 1,
            ["maxJunkPeriod"] = 2,
            ["minEnemyShipPeriod"] = 2,
            ["maxEnemyShipPeriod"] = 4,
            ["minEnemyShipProbability"] = 0.3f,
            ["maxEnemyShipProbability"] = 0.7f,
            ["minJunkSpd"] = 1,
            ["maxJunkSpd"] = 5,
            ["minEnemyShipSpd"] = 2,
            ["maxEnemyShipSpd"] = 5,
            ["minEnemyShootingPeriod"] = 0.5f,
            ["maxEnemyShootingPeriod"] = 3,
            ["bossHP"] = 100,
            ["bossShootingPeriod"] = 100,
        },
        ["Normal"] = new Dictionary<string, float>
        {
            ["junkTime"] = 30,
            ["bossAppearingTime"] = 150, // Boss will spwan at 2:30 after level begun
            ["minJunkPeriod"] = 1,
            ["maxJunkPeriod"] = 2,
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
            ["junkTime"] = 30,
            ["bossAppearingTime"] = 150, // Boss will spwan at 2:30 after level begun
            ["minJunkPeriod"] = 1,
            ["maxJunkPeriod"] = 2,
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
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        float timePassed = 0f;
        float nextEnemyAppearTime;

        SpaceJunkManager spaceJunkManager;
        EnemySpaceshipManager enemySpaceshipManager;

        // Generate only space junks
        while (timePassed < levelParameters[gameDifficulty.ToString()]["junkTime"])
        {
            nextEnemyAppearTime = Random.Range(levelParameters[gameDifficulty.ToString()]["minJunkPeriod"], levelParameters[gameDifficulty.ToString()]["maxJunkPeriod"]);
            yield return new WaitForSeconds(nextEnemyAppearTime);

            PickEnemyAppearPosition();
            spaceJunkManager = Instantiate(spacejunks[Random.Range(0, spacejunks.Length)], enemyAppearPosition, Quaternion.identity).GetComponent<SpaceJunkManager>();
            spaceJunkManager.appearSide = enemyAppearSide;

            timePassed += nextEnemyAppearTime;
        }

        // Generate enemy spaceships and spacejunks according to the time passed and probability
        while (timePassed < levelParameters[gameDifficulty.ToString()]["bossAppearingTime"])
        {
            // Choose appear spacejunk or enemy spaceship
            if (enemyshipNotJunk(timePassed))
            {
                nextEnemyAppearTime = Random.Range(levelParameters[gameDifficulty.ToString()]["minEnemyShipPeriod"], levelParameters[gameDifficulty.ToString()]["maxEnemyShipPeriod"]);
                yield return new WaitForSeconds(nextEnemyAppearTime);

                int enemyShipColor = Random.Range(0, enemySpaceships.Length); // Pick enemy spaceship color
                PickEnemyAppearPosition();
                enemySpaceshipManager = Instantiate(enemySpaceships[enemyShipColor], enemyAppearPosition, Quaternion.identity).GetComponent<EnemySpaceshipManager>();
                enemySpaceshipManager.appearSide = enemyAppearSide;
                enemySpaceshipManager.thisColor = enemyShipColor;
            }
            else
            {
                nextEnemyAppearTime = Random.Range(levelParameters[gameDifficulty.ToString()]["minJunkPeriod"], levelParameters[gameDifficulty.ToString()]["maxJunkPeriod"]);
                yield return new WaitForSeconds(nextEnemyAppearTime);

                PickEnemyAppearPosition();
                spaceJunkManager = Instantiate(spacejunks[Random.Range(0, spacejunks.Length)], enemyAppearPosition, Quaternion.identity).GetComponent<SpaceJunkManager>();
                spaceJunkManager.appearSide = enemyAppearSide;
            }

            timePassed += nextEnemyAppearTime;
        }
    }

    private void PickEnemyAppearPosition()
    {
        enemyAppearSide = Random.Range(0, 4);

        switch (enemyAppearSide)
        {
            case 0: // East side
                enemyAppearPosition = new Vector2(enemiesAppearLinesOnESWNsides.x, Random.Range(randomizedEnemiesPositionsLeftRightUpDown.y, randomizedEnemiesPositionsLeftRightUpDown.w));
                break;
            case 1: // South side
                enemyAppearPosition = new Vector2(Random.Range(randomizedEnemiesPositionsLeftRightUpDown.z, randomizedEnemiesPositionsLeftRightUpDown.x), enemiesAppearLinesOnESWNsides.y);
                break;
            case 2: // West side
                enemyAppearPosition = new Vector2(enemiesAppearLinesOnESWNsides.z, Random.Range(randomizedEnemiesPositionsLeftRightUpDown.y, randomizedEnemiesPositionsLeftRightUpDown.w));
                break;
            case 3: // North side
                enemyAppearPosition = new Vector2(Random.Range(randomizedEnemiesPositionsLeftRightUpDown.z, randomizedEnemiesPositionsLeftRightUpDown.w), enemiesAppearLinesOnESWNsides.w);
                break;
        }
    }

    private bool enemyshipNotJunk(float t)
    {
        float spaceShipP = (levelParameters[gameDifficulty.ToString()]["maxEnemyShipProbability"] - levelParameters[gameDifficulty.ToString()]["minEnemyShipProbability"]) / (levelParameters[gameDifficulty.ToString()]["bossAppearingTime"] - levelParameters[gameDifficulty.ToString()]["junkTime"]) * (t - levelParameters[gameDifficulty.ToString()]["junkTime"]) + levelParameters[gameDifficulty.ToString()]["minEnemyShipProbability"];
        return Random.Range(0f, 1f) < spaceShipP;
    }
}
