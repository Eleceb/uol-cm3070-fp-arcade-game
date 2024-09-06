using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJunkManager : MonoBehaviour
{
    public float parentShipShootAngle;
    public int appearSide; // 0: Right, 1: Down; 2: Left; 3: Up.
    public bool isFromShip;
    public int junkColor;

    [SerializeField] int spaceJunkScore;

    public float speed;
    float flyingDirection;

    Vector2 originalPosition;

    SpriteRenderer spriteRenderer;

    Camera mainCamera;

    [SerializeField] GameObject spacejunkExplosion, playerExplosion;
    [SerializeField] Sprite[] spaceJunkSpritesRed, spaceJunkSpritesBlue, spaceJunkSpritesYellow;

    LevelsManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelsManager>();

        if (speed == 0)
            speed = Random.Range(levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minJunkSpd"], levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxJunkSpd"]);

        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        originalPosition = transform.position;

        PickColor();

        PickFlyingDirection();

        // Spwan a row if random results in the probability range
        if (!isFromShip && Random.Range(0f,1f) < levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["junkInRowProbability"])
        {
            StartCoroutine(SpawnRowCoroutine());            
        }
    }

    private void PickColor()
    {
        if (!isFromShip)
        {
            // Randomly pick a color for the spacejunk
            junkColor = Random.Range(0, 3);
        }
        switch (junkColor)
        {
            case 0:
                spriteRenderer.sprite = spaceJunkSpritesRed[Random.Range(0, spaceJunkSpritesRed.Length)];
                break;
            case 1:
                spriteRenderer.sprite = spaceJunkSpritesBlue[Random.Range(0, spaceJunkSpritesBlue.Length)];
                break;
            case 2:
                spriteRenderer.sprite = spaceJunkSpritesYellow[Random.Range(0, spaceJunkSpritesYellow.Length)];
                break;
        }

        // Make the polygon collider fit the shape of the randomly picked sprite
        PolygonCollider2D pc2D = GetComponent<PolygonCollider2D>();

        Physics2D.SyncTransforms(); // Ensure the physics system is up to date
        pc2D.pathCount = spriteRenderer.sprite.GetPhysicsShapeCount();

        for (int i = 0; i < pc2D.pathCount; i++)
        {
            List<Vector2> path = new List<Vector2>();
            spriteRenderer.sprite.GetPhysicsShape(i, path);
            pc2D.SetPath(i, path.ToArray());
        }
    }

    private void PickFlyingDirection()
    {
        if (isFromShip)
        {
            flyingDirection = parentShipShootAngle;
            transform.eulerAngles = new Vector3(0f, 0f, parentShipShootAngle * Mathf.Rad2Deg - 90f);
        }
        else
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
        }
    }

    private void Update()
    {
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
        if (collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);

            if (collision.GetComponent<BulletManager>().bulletColorMode == junkColor)
            {

                levelManager.UpdateScore(spaceJunkScore);

                Destroy(gameObject);

                GameObject explosionEffect = Instantiate(
                    spacejunkExplosion,
                    transform.position,
                    Quaternion.identity,
                    transform.parent
                );

                AudioManager.Instance.PlaySound(AudioManager.Instance.spacejunkExplosionSound);

                Destroy(explosionEffect, 1f);
            }
        }
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != junkColor)
        {
            Destroy(collision.gameObject);

            GameObject explosionEffect = Instantiate(
                playerExplosion,
                collision.transform.position,
                Quaternion.identity,
                transform.parent
            );

            AudioManager.Instance.PlaySound(AudioManager.Instance.explosionSound);

            Destroy(explosionEffect, 1f);

            levelManager.GameOver();
        }
        else if (collision.tag == "BoundaryDestroyer")
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator SpawnRowCoroutine()
    {
        int junkNumberInRow = Random.Range((int)levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minJunkRowSize"], (int)levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxJunkRowSize"]);
        
        for (int i = 0; i < junkNumberInRow; i++)
        {
            yield return new WaitForSeconds(0.8f * 1.2f / speed); // 0.8f is about the dimension size of a spacejunk, *1.2 give some space between the spacejunks
            GameObject nextSpaceJunk = Instantiate(gameObject, originalPosition, Quaternion.identity);
            nextSpaceJunk.GetComponent<SpaceJunkManager>().isFromShip = true;
            nextSpaceJunk.GetComponent<SpaceJunkManager>().junkColor = junkColor;
            nextSpaceJunk.GetComponent<SpaceJunkManager>().speed = speed;
            nextSpaceJunk.GetComponent<SpaceJunkManager>().parentShipShootAngle = flyingDirection;
        }
    }
}
