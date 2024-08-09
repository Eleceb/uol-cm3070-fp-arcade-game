using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualBossPart : MonoBehaviour
{
    int thisPartHP;

    [SerializeField] int thisColor;

    [SerializeField] GameObject playerExplosion, bossExplosionEffect;

    Animator animator;

    LevelsManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelsManager>();

        thisPartHP = (int)levelManager.levelParameters[levelManager.gameDifficulty.ToString()]["bossPartHP"];

        animator = GetComponent<Animator>();
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
                    // Make boss cannot fire the spacejunk of this color
                    GetComponentInParent<BossManager>().bossRemainingColor.Remove(thisColor);

                    GameObject explosionEffect = Instantiate(
                        bossExplosionEffect,
                        transform.position,
                        Quaternion.identity
                    );

                    Destroy(explosionEffect, 1f);

                    animator.Play("BW");
                }
                else
                {
                    animator.Play("WhiteFlash");
                }
            }
        }
        else if (collision.tag == "Player" && collision.GetComponent<SpaceshipController>().currentColorMode != thisColor)
        {
            GameObject explosionEffect = Instantiate(
                playerExplosion,
                collision.transform.position,
                Quaternion.identity
            );
            Destroy(explosionEffect, 1f);
            Destroy(collision.gameObject);
        }
    }
}
