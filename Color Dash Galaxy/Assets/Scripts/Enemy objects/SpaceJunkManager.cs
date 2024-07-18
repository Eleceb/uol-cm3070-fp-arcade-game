using UnityEngine;

public class SpaceJunkManager : MonoBehaviour
{
    public float parentShipShootAngle;
    public int appearSide; // 0: Right, 1: Down; 2: Left; 3: Up.
    public bool isFromShip;
    public int junkColor;

    float speed, flyingDirection;

    SpriteRenderer spriteRenderer;

    Camera mainCamera;

    [SerializeField] GameObject spacejunkExplosion, playerExplosion;

    LevelsManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelsManager>();

        speed = Random.Range(levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["minEnemyShipSpd"], levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["maxEnemyShipSpd"]);

        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        PickColor();

        PickFlyingDirection();
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
                spriteRenderer.color = new Color(1f, 0f, 0f, 1f);
                break;
            case 1:
                spriteRenderer.color = new Color(0.255f, 0.761f, 0.965f, 1f);
                break;
            case 2:
                spriteRenderer.color = new Color(0.965f, 0.584f, 0.031f, 1f);
                break;
        }
    }

    private void PickFlyingDirection()
    {
        if (isFromShip)
        {
            flyingDirection = parentShipShootAngle;
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
        if (collision.tag == "Bullet" && collision.GetComponent<BulletManager>().bulletColorMode == junkColor)
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
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != junkColor)
        {
            GameObject explosionEffect = Instantiate(
                playerExplosion,
                collision.transform.position,
                Quaternion.identity
            );
            Destroy(explosionEffect, 1f);
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "BoundaryDestroyer")
        {
            Destroy(gameObject);
        }
    }
}
