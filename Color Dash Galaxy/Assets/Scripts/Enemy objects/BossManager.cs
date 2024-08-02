using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public int appearSide; // 0: Right, 2: Left.

    Transform playerTransform;

    float initialSpeed, flyingDirection, stoppingCoordinate;
    float speed, stoppingTimeElapsed, stoppingLerpDuration = 0.5f;
    float shootingInterval;

    float[] stoppingPointScreenPortionMinMax = new float[] { 0.35f, 0.65f };

    bool hasStopped;
    float amplitude = 0.5f;  // Height of the hover
    float frequency = 1f;    // Speed of the hover
    Vector3 stoppedPosition;
    float stoppedTime;

    Camera mainCamera;

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
        hasStopped = true;


    }

    private void DecelerateAndStop()
    {
        speed = Mathf.Lerp(initialSpeed, 0f, stoppingTimeElapsed / stoppingLerpDuration);
        stoppingTimeElapsed += Time.deltaTime;
    }

    private void CalculateStoppingPoint()
    {
        stoppingCoordinate = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(stoppingPointScreenPortionMinMax[0], stoppingPointScreenPortionMinMax[1]), 0, Camera.main.nearClipPlane)).x;
    }

    private void LateUpdate()
    {
        if (hasStopped)
        {
            transform.position = stoppedPosition + Vector3.up * amplitude * Mathf.Sin((Time.time - stoppedTime) * frequency);
        }
    }
}
