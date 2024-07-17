using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceshipManager : MonoBehaviour
{
    public float initialSpeed, rotationSpeed;
    public int appearSide; // 0: Right, 1: Down; 2: Left; 3: Up.
    public int thisColor;

    Transform playerTransform;

    float flyingDirection, stoppingCoordinate;
    float speed, stoppingTimeElapsed, stoppingLerpDuration = 0.5f;

    float[] stoppingPointScreenPortionMinMax = new float[] {0.1f, 0.9f};

    Camera mainCamera;

    [SerializeField] float spacejunkSpreadAngleOneSide, shootingInterval;
    [SerializeField] GameObject[] spaceJunks;
    [SerializeField] GameObject spaceshipExplosion;

    GameObject spaceJunkShotOut;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        CalculateStoppingPoint();

        PickFlyingDirection();

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
            float targetAngle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg + 270f;
            Quaternion targetQuaternion = Quaternion.Euler(new Vector3(0, 0, targetAngle));
            while (transform.rotation.z != targetQuaternion.z)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Keep shooting spacejunks
        while (true)
        {
            spaceJunkShotOut = Instantiate(spaceJunks[Random.Range(0, spaceJunks.Length)], transform.position, transform.rotation);
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().isFromShip = true;
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().parentShipShootAngle = (transform.eulerAngles.z - 270f + Random.Range(-spacejunkSpreadAngleOneSide, spacejunkSpreadAngleOneSide)) * Mathf.Deg2Rad;
            spaceJunkShotOut.GetComponent<SpaceJunkManager>().junkColor = thisColor;
            yield return new WaitForSeconds(shootingInterval);
        }
    }

    private void DecelerateAndStop()
    {
        speed = Mathf.Lerp(initialSpeed, 0f, stoppingTimeElapsed / stoppingLerpDuration);
        stoppingTimeElapsed += Time.deltaTime;
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
        if (collision.tag == "BoundaryDestroyer" || collision.tag == "Bullet" && collision.GetComponent<BulletManager>().bulletColorMode == thisColor)
        {
            Destroy(collision.gameObject);

            GameObject explosionEffect = Instantiate(
                spaceshipExplosion,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosionEffect, 1f);

            Destroy(gameObject);
        }
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != thisColor)
        {
            GameObject explosionEffect = Instantiate(
                spaceshipExplosion,
                collision.transform.position,
                Quaternion.identity
            );
            Destroy(explosionEffect, 1f);
            Destroy(collision.gameObject);
        }
    }
}
