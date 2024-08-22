using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualBossPart : MonoBehaviour
{
    int thisPartHP;

    public int thisColor;

    public List<Sprite> bossPartSprites;

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
                thisPartHP -= 1;

                if (thisPartHP <= 0)
                {
                    animator.Play("DamageFlashAnimation");

                    // Make boss cannot fire the spacejunk of this color
                    GetComponentInParent<BossManager>().bossRemainingColor.Remove(thisColor);
                    spriteRenderer.material = grayscaleMaterial;

                    if (GetComponentInParent<BossManager>().bossRemainingColor.Count > 0)
                    {
                        levelManager.UpdateScore(bossPartDestroyedScore);

                        GameObject explosionEffect = Instantiate(
                            bossPartExplosionEffect,
                            transform.position,
                            Quaternion.identity
                        );

                        Destroy(explosionEffect, 1f);
                    }
                    else
                    {
                        levelManager.UpdateScore(bossDestroyedScore + bossPartDestroyedScore);

                        levelManager.isBossDestroyed = true;

                        transform.GetComponentInParent<BossManager>().StopAllCoroutines(); // Stop the boss's movements

                        // Play boss explosion effects
                        GameObject explosionEffect = Instantiate(
                            bossExplosionEffect,
                            transform.parent.position,
                            Quaternion.identity
                        );

                        Destroy(explosionEffect, 5f);
                        Invoke("DestroyBossObject", 5f);

                        transform.parent.tag = "Untagged";

                        // Check win condition
                        if (levelManager.isBossDestroyed && GameObject.FindGameObjectsWithTag("EnemiesMustBeGoneBeforeWin").Length == 0)
                        {
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
            GameObject explosionEffect = Instantiate(
                playerExplosion,
                collision.transform.position,
                Quaternion.identity
            );
            Destroy(explosionEffect, 1f);
            Destroy(collision.gameObject);

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

        Destroy(transform.parent.gameObject);

        Destroy(bigExplosion, 1f);
    }
}
