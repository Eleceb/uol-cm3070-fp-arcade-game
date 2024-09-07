using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject spacejunk;
    [SerializeField] GameObject[] enemySpaceships;
    [SerializeField] GameObject boss;

    [SerializeField] Vector4 enemiesAppearLinesOnESWNsides, randomizedEnemiesPositionsLeftRightUpDown;

    [SerializeField] GameObject gameOverMenu, winMenu;
    [SerializeField] Button gameOverMenuDefaultButton, winMenuDefaultButton;
    [SerializeField] Text scoreText;
    [SerializeField] TimeCount timeCountScript;

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
            ["junkTime"] = 45,
            ["bossAppearingTime"] = 150, // Boss will spawn at 2:30 after level begun
            ["minJunkPeriod"] = 2,
            ["maxJunkPeriod"] = 4,
            ["junkInRowProbability"] = 0.1f,
            ["minJunkRowSize"] = 4,
            ["maxJunkRowSize"] = 6,
            ["minEnemyShipPeriod"] = 3,
            ["maxEnemyShipPeriod"] = 6,
            ["minEnemyShipProbability"] = 0.3f,
            ["maxEnemyShipProbability"] = 0.7f,
            ["minJunkSpd"] = 1,
            ["maxJunkSpd"] = 4,
            ["minEnemyShipSpd"] = 2,
            ["maxEnemyShipSpd"] = 5,
            ["minEnemyShootingPeriod"] = 1,
            ["maxEnemyShootingPeriod"] = 3,
            ["bossPartHP"] = 5,
            ["minBossShootingPeriod"] = 3f,
            ["maxBossShootingPeriod"] = 5f,
            ["minBossMovementPeriod"] = 7.5f,
            ["maxBossMovementPeriod"] = 15f,
        },
        ["Normal"] = new Dictionary<string, float>
        {
            ["junkTime"] = 30,
            ["bossAppearingTime"] = 150, // Boss will spwan at 2:30 after level begun
            ["minJunkPeriod"] = 1,
            ["maxJunkPeriod"] = 2,
            ["junkInRowProbability"] = 0.2f,
            ["minJunkRowSize"] = 4,
            ["maxJunkRowSize"] = 6,
            ["minEnemyShipPeriod"] = 2.5f,
            ["maxEnemyShipPeriod"] = 5,
            ["minEnemyShipProbability"] = 0.3f,
            ["maxEnemyShipProbability"] = 0.7f,
            ["minJunkSpd"] = 1.5f,
            ["maxJunkSpd"] = 4.5f,
            ["minEnemyShipSpd"] = 2.5f,
            ["maxEnemyShipSpd"] = 6,
            ["minEnemyShootingPeriod"] = 1,
            ["maxEnemyShootingPeriod"] = 2,
            ["bossPartHP"] = 10,
            ["minBossShootingPeriod"] = 2f,
            ["maxBossShootingPeriod"] = 3,
            ["minBossMovementPeriod"] = 6f,
            ["maxBossMovementPeriod"] = 10f,
        },
        ["Hard"] = new Dictionary<string, float>
        {
            ["junkTime"] = 30,
            ["bossAppearingTime"] = 150, // Boss will spwan at 2:30 after level begun
            ["minJunkPeriod"] = 0.5f,
            ["maxJunkPeriod"] = 1.5f,
            ["junkInRowProbability"] = 0.25f,
            ["minJunkRowSize"] = 4,
            ["maxJunkRowSize"] = 6,
            ["minEnemyShipPeriod"] = 2,
            ["maxEnemyShipPeriod"] = 3,
            ["minEnemyShipProbability"] = 0.3f,
            ["maxEnemyShipProbability"] = 0.7f,
            ["minJunkSpd"] = 2,
            ["maxJunkSpd"] = 5,
            ["minEnemyShipSpd"] = 2.5f,
            ["maxEnemyShipSpd"] = 6.5f,
            ["minEnemyShootingPeriod"] = 0.5f,
            ["maxEnemyShootingPeriod"] = 1.5f,
            ["bossPartHP"] = 20,
            ["minBossShootingPeriod"] = 1f,
            ["maxBossShootingPeriod"] = 2,
            ["minBossMovementPeriod"] = 4f,
            ["maxBossMovementPeriod"] = 6f,
        },
    };

    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetInt("Difficulty", 0))
        {
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

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        float timePassed = 0f;
        float nextEnemyAppearTime;
        bool hasLerpVolumeExecuted = false;

        SpaceJunkManager spaceJunkManager;
        EnemySpaceshipManager enemySpaceshipManager;
        BossManager bossManager;

        // Generate only space junks
        while (timePassed < levelParameters[gameDifficulty.ToString()]["junkTime"])
        {
            if (timePassed >= levelParameters[gameDifficulty.ToString()]["bossAppearingTime"] - 10f && !hasLerpVolumeExecuted)
            {
                StartCoroutine(LerpVolume());
                hasLerpVolumeExecuted = true;
            }

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
            if (timePassed >= levelParameters[gameDifficulty.ToString()]["bossAppearingTime"] - 10f && !hasLerpVolumeExecuted)
            {
                StartCoroutine(LerpVolume());
                hasLerpVolumeExecuted = true;
            }

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
        isBossDestroyed = false;

        AudioManager.Instance.musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.bossMusic, true);

        while (!isBossDestroyed)
        {
            if (gameDifficulty == GameDifficulty.Hard)
            {
                // Choose appear spacejunk or enemy spaceship
                if (Random.Range(0f, 1f) > 0.5f)
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
            }
            else if (gameDifficulty == GameDifficulty.Normal)
            {
                nextEnemyAppearTime = Random.Range(levelParameters[gameDifficulty.ToString()]["minJunkPeriod"], levelParameters[gameDifficulty.ToString()]["maxJunkPeriod"]);
                yield return new WaitForSeconds(nextEnemyAppearTime);

                PickEnemyAppearPosition();
                spaceJunkManager = Instantiate(spacejunk, enemyAppearPosition, Quaternion.identity).GetComponent<SpaceJunkManager>();
                spaceJunkManager.appearSide = enemyAppearSide;
            }
            else if (gameDifficulty == GameDifficulty.Easy)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        if (PlayerPrefs.GetInt("IsSurvivalMode", -1) == 1)
            StartCoroutine(SpawnEnemies());
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
        timeCountScript.enabled = false;

        scoreText.gameObject.SetActive(false);
        timeCountScript.gameObject.SetActive(false);

        gameOverMenu.SetActive(true);

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.gameOverMusic, false);

        gameOverMenuDefaultButton.GetComponent<ButtonSelect>().isFirstDefaultButtonSelection = true;
        gameOverMenuDefaultButton.Select();

        StopAllCoroutines();
    }

    public void WinGame(float timeDelay)
    {
        Invoke("WinGameActions", timeDelay);
    }
    private void WinGameActions()
    {
        player.GetComponent<SpaceshipController>().enabled = false;
        player.GetComponent<CircleCollider2D>().enabled = false;

        timeCountScript.enabled = false;

        scoreText.gameObject.SetActive(false);
        timeCountScript.gameObject.SetActive(false);

        winMenu.SetActive(true);

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.winMusic, false);

        winMenuDefaultButton.GetComponent<ButtonSelect>().isFirstDefaultButtonSelection = true;
        winMenuDefaultButton.Select();

        StopAllCoroutines();
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;

        scoreText.text = "Score: " + score.ToString();
    }

    private IEnumerator LerpVolume()
    {
        float startVolume = AudioManager.Instance.musicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < 7.5f)
        {
            elapsedTime += Time.deltaTime;
            AudioManager.Instance.musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / 7.5f);
            yield return null;
        }

        AudioManager.Instance.musicSource.volume = 0f;
    }
}
