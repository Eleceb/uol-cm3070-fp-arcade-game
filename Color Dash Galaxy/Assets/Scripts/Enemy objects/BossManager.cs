using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossManager : MonoBehaviour
{
    public int appearSide; // 0: Right, 2: Left.

    Transform playerTransform;

    float initialSpeed, stoppingCoordinate;
    float speed, stoppingTimeElapsed, stoppingLerpDuration = 2f;
    float shootingInterval, movingInterval;

    float[] stoppingPointScreenPortionMinMax = new float[] { 0.35f, 0.65f };

    bool hasStopped;
    float amplitude = 0.5f;  // Height of the hover
    float frequency = 1f;    // Speed of the hover
    Vector3 stoppedPosition;
    float stoppedTime;

    float transFormScale;

    public List<int> bossRemainingColor = new List<int> {0, 1, 2};

    [SerializeField] float spacejunkSpreadAngleOneSide;
    [SerializeField] GameObject spaceJunk;
    [SerializeField] GameObject spaceshipExplosion;

    GameObject spaceJunkShotOut, spaceShipDeployed;

    LevelsManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelsManager>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        transFormScale = transform.localScale.x;

        CalculateStoppingPoint();

        initialSpeed = levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minEnemyShipSpd"];
        speed = initialSpeed;

        StartCoroutine(BehaviorCoroutine());
    }

    private IEnumerator BehaviorCoroutine()
    {
        // Travel and stop
        while (speed > 0)
        {
            switch (appearSide)
            {
                case 0:
                    if (transform.position.x < stoppingCoordinate)
                        DecelerateAndStop();
                    break;
                case 2:
                    if (transform.position.x > stoppingCoordinate)
                        DecelerateAndStop();
                    break;
            }

            // Keep flying
            transform.position = new Vector2(
                transform.position.x + speed * (appearSide == 0 ? -1 : 1) * Time.deltaTime,
                transform.position.y
            );

            // Wait for next frame
            yield return null;
        }

        stoppedPosition = transform.position;
        stoppedTime = Time.time;
        StartCoroutine(HoverUpDownCoroutine());
        StartCoroutine(MoveLeftRightCoroutine());

        // Start deploying spacejunks
        while (true)
        {
            shootingInterval = Random.Range(levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minBossShootingPeriod"], levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxBossShootingPeriod"]);
            yield return new WaitForSeconds(shootingInterval);

            if (playerTransform)
            {
                spaceJunkShotOut = Instantiate(spaceJunk, transform.position, transform.rotation);
                spaceJunkShotOut.GetComponent<SpaceJunkManager>().isFromShip = true;
                spaceJunkShotOut.GetComponent<SpaceJunkManager>().parentShipShootAngle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x);
                spaceJunkShotOut.GetComponent<SpaceJunkManager>().junkColor = bossRemainingColor[Random.Range(0, bossRemainingColor.Count)];
            }
        }
    }

    private IEnumerator HoverUpDownCoroutine()
    {
        while (true)
        {
            transform.position = new Vector2(transform.position.x, stoppedPosition.y + amplitude * Mathf.Sin((Time.time - stoppedTime) * frequency));
            yield return null;
        }
    }

    private IEnumerator MoveLeftRightCoroutine()
    {
        float residueDistanceAfterReachingTargetX = initialSpeed * stoppingLerpDuration / 2f;

        while (true)
        {
            movingInterval = Random.Range(levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minBossMovementPeriod"], levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxBossMovementPeriod"]);
            yield return new WaitForSeconds(movingInterval);

            if (transform.position.x >= 0)
            {
                float xTargetCoordinate = Random.Range(-7f, 0f);
                transform.localScale = new Vector2(-transFormScale, transFormScale);
                stoppingTimeElapsed = 0;
                while (transform.position.x > xTargetCoordinate)
                {
                    AccelerateToInitailSpeed();
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(xTargetCoordinate, transform.position.y), speed * Time.deltaTime);
                    yield return null;
                }
                stoppingTimeElapsed = 0;
                while (speed > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(xTargetCoordinate - residueDistanceAfterReachingTargetX, transform.position.y), speed * Time.deltaTime);
                    DecelerateAndStop();
                    yield return null;
                }
            }
            else
            {
                float xTargetCoordinate = Random.Range(0f, 7f);
                transform.localScale = new Vector2(transFormScale, transFormScale);
                stoppingTimeElapsed = 0;
                while (transform.position.x < xTargetCoordinate)
                {
                    AccelerateToInitailSpeed();
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(xTargetCoordinate, transform.position.y), speed * Time.deltaTime);
                    yield return null;
                }
                stoppingTimeElapsed = 0;
                while (speed > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector2(xTargetCoordinate + residueDistanceAfterReachingTargetX, transform.position.y), speed * Time.deltaTime);
                    DecelerateAndStop();
                    yield return null;
                }
            }
        }
    }

    private void DecelerateAndStop()
    {
        speed = Mathf.Lerp(initialSpeed, 0f, stoppingTimeElapsed / stoppingLerpDuration);
        stoppingTimeElapsed += Time.deltaTime;
    }

    private void AccelerateToInitailSpeed()
    {
        speed = Mathf.Lerp(0f, initialSpeed, stoppingTimeElapsed / stoppingLerpDuration);
        stoppingTimeElapsed += Time.deltaTime;
    }

    private void CalculateStoppingPoint()
    {
        stoppingCoordinate = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(stoppingPointScreenPortionMinMax[0], stoppingPointScreenPortionMinMax[1]), 0, Camera.main.nearClipPlane)).x;
    }
}
