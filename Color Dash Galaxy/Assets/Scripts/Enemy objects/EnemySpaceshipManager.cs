using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceshipManager : MonoBehaviour
{
    public float initialSpeed;
    public int appearSide; // 0: Right, 1: Down; 2: Left; 3: Up.
    public int thisColor;

    float flyingDirection, stoppingCoordinate;
    float speed, stoppingTimeElapsed, stoppingLerpDuration = 0.5f;

    float[] stoppingPointScreenPortionMinMax = new float[] {0.1f, 0.9f};

    Camera mainCamera;

    [SerializeField] GameObject spacejunkExplosion;

    // Start is called before the first frame update
    void Start()
    {
        CalculateStoppingPoint();

        PickFlyingDirection();

        speed = initialSpeed;
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

    // Update is called once per frame
    void Update()
    {
        switch (appearSide)
        {
            case 0:
                if (transform.position.x < stoppingCoordinate)
                {
                    speed = Mathf.Lerp(initialSpeed, 0f, stoppingTimeElapsed / stoppingLerpDuration);
                    stoppingTimeElapsed += Time.deltaTime;
                }
                break;
            case 1:
                if (transform.position.y < stoppingCoordinate)
                    KeepFlying();
                break;
            case 2:
                if (transform.position.x < stoppingCoordinate)
                    KeepFlying();
                break;
            case 3:
                if (transform.position.y > stoppingCoordinate)
                    KeepFlying();
                break;
        }

        Debug.Log(speed);

        transform.position = new Vector2(
            transform.position.x + speed * Mathf.Cos(flyingDirection) * Time.deltaTime,
            transform.position.y + speed * Mathf.Sin(flyingDirection) * Time.deltaTime
        );
    }

    private void KeepFlying() {
        transform.position = new Vector2(
            transform.position.x + speed * Mathf.Cos(flyingDirection) * Time.deltaTime,
            transform.position.y + speed * Mathf.Sin(flyingDirection) * Time.deltaTime
        );
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
                spacejunkExplosion,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosionEffect, 1f);

            Destroy(gameObject);
        }
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != thisColor)
        {
            Destroy(collision.gameObject);
        }
    }
}
