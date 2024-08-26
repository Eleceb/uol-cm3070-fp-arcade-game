using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject spacejunk;
    [SerializeField] GameObject[] enemySpaceships;
    [SerializeField] GameObject boss;

    [SerializeField] Vector4 enemiesAppearLinesOnESWNsides, randomizedEnemiesPositionsLeftRightUpDown;

    [SerializeField] GameObject gameOverMenu, winMenu;
    [SerializeField] Button gameOverMenuDefaultButton, winMenuDefaultButton;
    [SerializeField] Text scoreText;

    int enemyAppearSide;
    Vector2 enemyAppearPosition;
    float[] bossVerticalAppearancePositionRange = new float[] { -2.5f, 2.5f };

    public int score;

    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard
    }
    public GameDifficulty gameDifficulty;

    public bool isBossDestroyed;

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
            ["bossPartHP"] = 5,
            ["minBossShootingPeriod"] = 1f,
            ["maxBossShootingPeriod"] = 2f,
            ["minBossMovementPeriod"] = 7.5f,
            ["maxBossMovementPeriod"] = 15f,
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
            ["bossPartHP"] = 100,
            ["minBossShootingPeriod"] = 0.5f,
            ["maxBossShootingPeriod"] = 3,
            ["minBossMovementPeriod"] = 7.5f,
            ["maxBossMovementPeriod"] = 15f,
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
            ["bossPartHP"] = 100,
            ["minBossShootingPeriod"] = 0.5f,
            ["maxBossShootingPeriod"] = 3,
            ["minBossMovementPeriod"] = 7.5f,
            ["maxBossMovementPeriod"] = 15f,
        },
    };

    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetInt("Difficulty", 0)) { 
            case 0:
                gameDifficulty = GameDifficulty.Easy; 
                break;
            case 1:
                gameDifficulty = GameDifficulty.Normal;
                break;
            case 2:
                gameDifficulty = GameDifficulty.Hard;
                break;
        }

        score = 0;

        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        float timePassed = 0f;
        float nextEnemyAppearTime;

        SpaceJunkManager spaceJunkManager;
        EnemySpaceshipManager enemySpaceshipManager;
        BossManager bossManager;

        // Generate only space junks
        while (timePassed < levelParameters[gameDifficulty.ToString()]["junkTime"])
        {
            nextEnemyAppearTime = Random.Range(levelParameters[gameDifficulty.ToString()]["minJunkPeriod"], levelParameters[gameDifficulty.ToString()]["maxJunkPeriod"]);
            yield return new WaitForSeconds(nextEnemyAppearTime);

            PickEnemyAppearPosition();
            spaceJunkManager = Instantiate(spacejunk, enemyAppearPosition, Quaternion.identity).GetComponent<SpaceJunkManager>();
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
                spaceJunkManager = Instantiate(spacejunk, enemyAppearPosition, Quaternion.identity).GetComponent<SpaceJunkManager>();
                spaceJunkManager.appearSide = enemyAppearSide;
            }

            timePassed += nextEnemyAppearTime;
        }

        PickBossAppearPosition();
        bossManager = Instantiate(boss, enemyAppearPosition, Quaternion.identity).GetComponent<BossManager>();
        bossManager.appearSide = enemyAppearSide;

        AudioManager.Instance.PlayMusic(AudioManager.Instance.bossMusic);
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

    private void PickBossAppearPosition()
    {
        enemyAppearSide = Random.Range(0, 1) > 0.5 ? 0 : 2;

        switch (enemyAppearSide)
        {
            case 0: // East side
                enemyAppearPosition = new Vector2(enemiesAppearLinesOnESWNsides.x, Random.Range(bossVerticalAppearancePositionRange[0], bossVerticalAppearancePositionRange[1]));
                break;
            case 2: // West side
                enemyAppearPosition = new Vector2(enemiesAppearLinesOnESWNsides.z, Random.Range(bossVerticalAppearancePositionRange[0], bossVerticalAppearancePositionRange[1]));
                break;
        }
    }

    private bool enemyshipNotJunk(float t)
    {
        float spaceShipP = (levelParameters[gameDifficulty.ToString()]["maxEnemyShipProbability"] - levelParameters[gameDifficulty.ToString()]["minEnemyShipProbability"]) / (levelParameters[gameDifficulty.ToString()]["bossAppearingTime"] - levelParameters[gameDifficulty.ToString()]["junkTime"]) * (t - levelParameters[gameDifficulty.ToString()]["junkTime"]) + levelParameters[gameDifficulty.ToString()]["minEnemyShipProbability"];
        return Random.Range(0f, 1f) < spaceShipP;
    }

    public void GameOver()
    {
        Invoke("GameOverActions", 1f);
    }

    private void GameOverActions()
    {
        scoreText.gameObject.SetActive(false);

        gameOverMenu.SetActive(true);

        gameOverMenuDefaultButton.Select();

        StopAllCoroutines();
    }

    public void WinGame(float timeDelay)
    {
        Invoke("WinGameActions", timeDelay);
    }
    private void WinGameActions()
    {
        scoreText.gameObject.SetActive(false);

        winMenu.SetActive(true);

        winMenuDefaultButton.Select();

        StopAllCoroutines();
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;

        scoreText.text = "Score: " + score.ToString();
    }
}
