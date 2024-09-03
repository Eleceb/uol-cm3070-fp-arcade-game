using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualBossPart : MonoBehaviour
{
    int thisPartHP;

    public int thisColor;

    public List<Sprite> bossPartSprites;

    GameObject player;

    [SerializeField] GameObject playerExplosion, bossPartExplosionEffect, bossExplosionEffect, bossFinalExplosionEffect;

    [SerializeField] int bossPartHitScore, bossPartDestroyedScore, bossDestroyedScore;

    [SerializeField] float flashTime;

    [SerializeField] Material grayscaleMaterial;

    Animator animator;

    SpriteRenderer spriteRenderer;

    LevelsManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        levelManager = FindObjectOfType<LevelsManager>();

        thisPartHP = (int)levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["bossPartHP"];

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
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

            if (collision.GetComponent<BulletManager>().bulletColorMode == thisColor && thisPartHP > 0) // The condition of "thisPartHP > 0" avoid multiple explosion effects
            {
                BossManager bossManager = GetComponentInParent<BossManager>();

                thisPartHP -= 1;

                if (thisPartHP <= 0)
                {
                    animator.Play("DamageFlashAnimation");

                    // Make boss cannot fire the spacejunk of this color
                    bossManager.bossRemainingColor.Remove(thisColor);
                    spriteRenderer.material = grayscaleMaterial;
                    spriteRenderer.sprite = bossPartSprites[0];

                    if (bossManager.bossRemainingColor.Count > 0)
                    {
                        Vector2 explosionPosition;
                        if (gameObject == bossManager.bossParts[0]) {
                            explosionPosition = new Vector2(-1.27f, 0.91f);
                        }
                        else if (gameObject == bossManager.bossParts[1])
                        {
                            explosionPosition = new Vector2(0f, -1.67f);
                        }
                        else
                        {
                            explosionPosition = new Vector2(1.27f, 0.91f);
                        }

                        levelManager.UpdateScore(bossPartDestroyedScore);

                        GameObject explosionEffect = Instantiate(
                            bossPartExplosionEffect,
                            (Vector2)transform.position + explosionPosition,
                            Quaternion.identity,
                            transform.parent
                        );
                        explosionEffect.transform.localScale = new Vector2(2.5f, 2.5f);

                        AudioManager.Instance.PlaySound(AudioManager.Instance.explosionSound);

                        Destroy(explosionEffect, 1f);
                    }
                    else
                    {
                        levelManager.UpdateScore(bossPartDestroyedScore);

                        levelManager.isBossDestroyed = true;

                        bossManager.StopAllCoroutines(); // Stop the boss's movements

                        // Play boss explosion effects
                        GameObject explosionEffect = Instantiate(
                            bossExplosionEffect,
                            transform.parent.position,
                            Quaternion.identity,
                            transform.parent
                        );

                        AudioManager.Instance.PlayExplodingSound();

                        Destroy(explosionEffect, 5f);
                        Invoke("DestroyBossObject", 5f);

                        transform.parent.tag = "Untagged";

                        // Check win condition
                        if (levelManager.isBossDestroyed && GameObject.FindGameObjectsWithTag("EnemiesMustBeGoneBeforeWin").Length == 0)
                        {
                            player.GetComponent<SpaceshipController>().enabled = false;
                            player.GetComponent<CircleCollider2D>().enabled = false;
                            levelManager.WinGame(7f);
                        }
                    }
                }
                else
                {
                    animator.Play("DamageFlashAnimation");
                    levelManager.UpdateScore(bossPartHitScore);
                }
            }
        }
        else if (collision.tag == "Player" && !levelManager.isBossDestroyed)
        {
            Destroy(collision.gameObject);

            GameObject explosionEffect = Instantiate(
                playerExplosion,
                collision.transform.position,
                Quaternion.identity
            );

            AudioManager.Instance.PlaySound(AudioManager.Instance.explosionSound);

            Destroy(explosionEffect, 1f);

            levelManager.GameOver();
        }
    }

    private void DestroyBossObject()
    {
        GameObject bigExplosion = Instantiate(
            bossFinalExplosionEffect,
            transform.parent.position,
            Quaternion.identity
        );
        bigExplosion.transform.localScale = new Vector2 ( 4f, 4f );

        AudioManager.Instance.StopExplodingSound();
        AudioManager.Instance.PlaySound(AudioManager.Instance.bigExplosionSound);

        levelManager.UpdateScore(bossDestroyedScore);

        Destroy(transform.parent.gameObject);

        Destroy(bigExplosion, 1f);
    }
}
