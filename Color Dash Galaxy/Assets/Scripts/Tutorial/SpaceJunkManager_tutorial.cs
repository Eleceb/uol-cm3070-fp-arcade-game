using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJunkManager_tutorial : MonoBehaviour
{
    public int junkColor;

    public float speed;

    [SerializeField] Transform playerSpaceship;

    SpriteRenderer spriteRenderer;

    [SerializeField] GameObject spacejunkExplosion, playerExplosion;
    [SerializeField] Sprite[] junkSprites;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = junkSprites[Random.Range(0, junkSprites.Length)];

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        FitPolygonCollider();
    }

    private void FitPolygonCollider()
    {
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
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<PolygonCollider2D>().enabled = false;

                GameObject explosionEffect = Instantiate(
                    spacejunkExplosion,
                    transform.position,
                    Quaternion.identity,
                    transform.parent
                );

                AudioManager.Instance.PlaySound(AudioManager.Instance.spacejunkExplosionSound);

                Destroy(explosionEffect, 1f);

                Invoke("RespawnObject", 1.5f);
            }
        }
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != junkColor)
        {
            playerSpaceship.GetComponent<CircleCollider2D>().enabled = false;
            playerSpaceship.GetComponent<SpriteRenderer>().enabled = false;
            playerSpaceship.GetComponent<SpaceshipController>().enabled = false;
            foreach (Transform child in playerSpaceship.transform)
            {
                child.gameObject.SetActive(false);
            }
            playerSpaceship.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

            GameObject explosionEffect = Instantiate(
                playerExplosion,
                collision.transform.position,
                Quaternion.identity,
                transform.parent
            );

            AudioManager.Instance.PlaySound(AudioManager.Instance.explosionSound);

            Destroy(explosionEffect, 1f);

            Invoke("RespwanPlayer", 1.5f);
        }
    }

    private void RespwanPlayer()
    {
        playerSpaceship.transform.position = new Vector2(4.35f, 0f);
        playerSpaceship.transform.rotation = Quaternion.identity;
        playerSpaceship.GetComponent<SpriteRenderer>().enabled = true;
        playerSpaceship.GetComponent<SpaceshipController>().enabled = true;
        playerSpaceship.GetComponent<CircleCollider2D>().enabled = true;
    }

    private void RespawnObject()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<PolygonCollider2D>().enabled = true;
    }
}
