using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceshipManager : MonoBehaviour
{
    public float rotationSpeed;
    public int appearSide; // 0: Right, 1: Down; 2: Left; 3: Up.
    public int thisColor;

    [SerializeField] int enemyShipScore;

    Transform playerTransform;

    float initialSpeed, flyingDirection, stoppingCoordinate;
    float speed, stoppingTimeElapsed, stoppingLerpDuration = 0.5f;
    float shootingInterval;

    float[] stoppingPointScreenPortionMinMax = new float[] {0.1f, 0.9f};

    Camera mainCamera;

    GameObject player;

    [SerializeField] float spacejunkSpreadAngleOneSide, bulletOffsetMultiplier;
    [SerializeField] GameObject spaceJunk;
    [SerializeField] GameObject spaceshipExplosion;
    [SerializeField] Transform[] thursters;

    GameObject spaceJunkShotOut;

    LevelsManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelsManager>();

        player = GameObject.FindGameObjectWithTag("Player");

        playerTransform = player.transform;

        CalculateStoppingPoint();

        PickFlyingDirection();

        initialSpeed = Random.Range(levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minEnemyShipSpd"], levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxEnemyShipSpd"]);
        speed = initialSpeed;

        StartCoroutine(BehaviorCoroutine());
    }

    private void CalculateStoppingPoint()
    {
        if (appearSide == 0 || appearSide == 2)
        {
            stoppingCoordinate = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(stoppingPointScreenPortionMinMax[0], stoppingPointScreenPortionMinMax[1]), 0, Camera.main.nearClipPlane)).x;
        }
        else
        {
            stoppingCoordinate = Camera.main.ViewportToWorldPoint(new Vector3(0, Random.Range(stoppingPointScreenPortionMinMax[0], stoppingPointScreenPortionMinMax[1]), Camera.main.nearClipPlane)).y;
        }
    }

    private void PickFlyingDirection()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3[] corners = new Vector3[4];

        corners[0] = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)); // Bottom-left
        corners[1] = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)); // Bottom-right
        corners[2] = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane)); // Top-left
        corners[3] = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane)); // Top-right

        switch (appearSide)
        {
            case 0: // Appear on right side
                flyingDirection = Random.Range(0, 0.999f) < (corners[2].y - transform.position.y) / (corners[2].y - corners[0].y) ?
                    Random.Range(Mathf.Atan2(corners[2].y - transform.position.y, corners[2].x - transform.position.x), Mathf.PI) :
                    Random.Range(-Mathf.PI, Mathf.Atan2(corners[0].y - transform.position.y, corners[0].x - transform.position.x));
                break;
            case 1: // Appear on down side
                flyingDirection = Random.Range(
                    Mathf.Atan2(corners[3].y - transform.position.y, corners[3].x - transform.position.x),
                    Mathf.Atan2(corners[2].y - transform.position.y, corners[2].x - transform.position.x)
                );
                break;
            case 2: // Appear on left side
                flyingDirection = Random.Range(
                    Mathf.Atan2(corners[1].y - transform.position.y, corners[1].x - transform.position.x),
                    Mathf.Atan2(corners[3].y - transform.position.y, corners[3].x - transform.position.x)
                );
                break;
            case 3: // Appear on up side
                flyingDirection = Random.Range(
                    Mathf.Atan2(corners[0].y - transform.position.y, corners[0].x - transform.position.x),
                    Mathf.Atan2(corners[1].y - transform.position.y, corners[1].x - transform.position.x)
                );
                break;
        }

        transform.rotation = Quaternion.Euler(0, 0, flyingDirection * Mathf.Rad2Deg + 270f);
    }

    private IEnumerator BehaviorCoroutine()
    {
        // Travel and stop
        while (speed > 0) {
            if (thursters.Length == 3)
            {
                thursters[0].gameObject.SetActive(true);
            }
            else if (thursters.Length == 4)
            {
                thursters[0].gameObject.SetActive(true);
                thursters[3].gameObject.SetActive(true);
            }

            switch (appearSide)
            {
                case 0:
                    if (transform.position.x < stoppingCoordinate)
                        DecelerateAndStop();
                    break;
                case 1:
                    if (transform.position.y > stoppingCoordinate)
                        DecelerateAndStop();
                    break;
                case 2:
                    if (transform.position.x > stoppingCoordinate)
                        DecelerateAndStop();
                    break;
                case 3:
                    if (transform.position.y < stoppingCoordinate)
                        DecelerateAndStop();
                    break;
            }

            // Keep flying
            transform.position = new Vector2(
                transform.position.x + speed * Mathf.Cos(flyingDirection) * Time.deltaTime,
                transform.position.y + speed * Mathf.Sin(flyingDirection) * Time.deltaTime
            );

            // Wait for next frame
            yield return null;
        }

        // Rotate towards player
        if (playerTransform) // Avoid error when the player is already destroyed
        {
            float turningTime = 0; // For setting the max time limit allow the rotation to happen

            float targetAngle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg + 270f;
            Quaternion targetQuaternion = Quaternion.Euler(new Vector3(0, 0, targetAngle));

            float previousEulerAngleZ = transform.rotation.eulerAngles.z;

            while (transform.rotation.z != targetQuaternion.z && turningTime <= 2.5f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);
                turningTime += Time.deltaTime;

                if (Mathf.Abs(transform.rotation.eulerAngles.z - previousEulerAngleZ) < 180f)
                {
                    if (transform.rotation.eulerAngles.z > previousEulerAngleZ)
                    {
                        thursters[1].gameObject.SetActive(true);
                    }
                    else if (transform.rotation.eulerAngles.z < previousEulerAngleZ)
                    {
                        thursters[2].gameObject.SetActive(true);
                    }
                    else
                    {
                        thursters[1].gameObject.SetActive(false);
                        thursters[2].gameObject.SetActive(false);
                    }
                }

                previousEulerAngleZ = transform.rotation.eulerAngles.z;

                yield return null;
            }
        }

        thursters[1].gameObject.SetActive(false);
        thursters[2].gameObject.SetActive(false);

        // Shoot the first bullet immediately after rotated
        if (playerTransform)
        {
            Vector2 bulletOffset = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z)) * bulletOffsetMultiplier;
            spaceJunkShotOut = Instantiate(spaceJunk, transform.position + (Vector3)bulletOffset, transform.rotation);
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().isFromShip = true;
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().parentShipShootAngle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x);
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().junkColor = thisColor;
        }

        // Keep shooting spacejunks
        while (true)
        {
            shootingInterval = Random.Range(levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minEnemyShootingPeriod"], levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxEnemyShootingPeriod"]);
            yield return new WaitForSeconds(shootingInterval);

            Vector2 bulletOffset = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.z)) * bulletOffsetMultiplier;
            spaceJunkShotOut = Instantiate(spaceJunk, transform.position + (Vector3)bulletOffset, transform.rotation);
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().isFromShip = true;
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().parentShipShootAngle = (transform.eulerAngles.z - 270f + Random.Range(-spacejunkSpreadAngleOneSide, spacejunkSpreadAngleOneSide)) * Mathf.Deg2Rad;
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().junkColor = thisColor;
        }
    }

    private void DecelerateAndStop()
    {
        speed = Mathf.Lerp(initialSpeed, 0f, stoppingTimeElapsed / stoppingLerpDuration);
        stoppingTimeElapsed += Time.deltaTime;

        if (thursters.Length == 3)
        {
            thursters[0].gameObject.SetActive(false);
        }
        else if (thursters.Length == 4)
        {
            thursters[0].gameObject.SetActive(false);
            thursters[3].gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    private void CheckCollision(Collider2D collision)
    {
        if (collision.tag == "BoundaryDestroyer" || collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);

            if (collision.GetComponent<BulletManager>().bulletColorMode == thisColor)
            {
                levelManager.UpdateScore(enemyShipScore);

                gameObject.tag = "Untagged";

                // Check win condition
                if (PlayerPrefs.GetInt("IsSurvivalMode") == -1 && levelManager.isBossDestroyed && GameObject.FindGameObjectWithTag("EnemiesMustBeGoneBeforeWin") == null)
                {
                    player.GetComponent<SpaceshipController>().enabled = false;
                    player.GetComponent<CircleCollider2D>().enabled = false;
                    levelManager.WinGame(2f);
                }

                Destroy(gameObject);

                GameObject explosionEffect = Instantiate(
                    spaceshipExplosion,
                    transform.position,
                    Quaternion.identity,
                    transform.parent
                );

                AudioManager.Instance.PlaySound(AudioManager.Instance.explosionSound);

                Destroy(explosionEffect, 1f);
            }
        }
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != thisColor)
        {
            Destroy(collision.gameObject);

            GameObject explosionEffect = Instantiate(
                spaceshipExplosion,
                collision.transform.position,
                Quaternion.identity,
                transform.parent
            );

            AudioManager.Instance.PlaySound(AudioManager.Instance.explosionSound);

            Destroy(explosionEffect, 1f);

            levelManager.GameOver();
        }
    }
}
