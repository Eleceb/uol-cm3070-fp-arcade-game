using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public int currentColorMode;

    [System.Serializable]
    private class ThrusterInformation
    {
        public Vector3[] positions, rotations;
    }

    [SerializeField] Transform closeDodgeDetector;
    [SerializeField] Transform[] thursters;
    [SerializeField] ThrusterInformation[] thrusterInformation;
    [SerializeField] Sprite[] colorModes;
    [SerializeField] GameObject[] colorBullets, changeColorEffects;
    [SerializeField] float accelerationForce, turningTorque, dragCoefficient, bulletOffsetMultiplier;

    private float currentRotationAngle;
    private bool isTouchingSameColor = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb2d;

    private List<Collider2D> collidingObjects = new List<Collider2D>();

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 bulletOffset = new Vector2(-Mathf.Sin(currentRotationAngle), Mathf.Cos(currentRotationAngle)) * bulletOffsetMultiplier;

            AudioManager.Instance.PlaySound(AudioManager.Instance.shootingSound);

            GameObject colorBullet = Instantiate(
                colorBullets[currentColorMode],
                transform.position + (Vector3)bulletOffset,
                Quaternion.Euler(0, 0, transform.eulerAngles.z)
            );

            colorBullet.GetComponent<BulletManager>().bulletColorMode = currentColorMode;
            colorBullet.GetComponent<BulletManager>().SetBulletShootingAngle(currentRotationAngle);
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.R))
        {
            currentColorMode++;

            if (currentColorMode == colorModes.Length)
            {
                currentColorMode = 0;
            }

            /*AudioManager.Instance.PlaySound(AudioManager.Instance.shootingSound);*/
            AudioManager.Instance.PlaySound(AudioManager.Instance.changeColorSound);

            ChangeColorMode(0.2f);
        }
    }
    
    void FixedUpdate()
    {
        currentRotationAngle = Mathf.Deg2Rad * transform.eulerAngles.z;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rb2d.AddForce(new Vector2(
                -accelerationForce * Mathf.Sin(currentRotationAngle),
                accelerationForce * Mathf.Cos(currentRotationAngle)
            ));

            thursters[0].gameObject.SetActive(true);
        }
        else
        {
            thursters[0].gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rb2d.AddForce(new Vector2(
                accelerationForce * Mathf.Sin(currentRotationAngle),
                -accelerationForce * Mathf.Cos(currentRotationAngle)
            ));

            if (((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && 
                    (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))) ||
                !(Input.GetKey(KeyCode.LeftArrow) || 
                    Input.GetKey(KeyCode.A) || 
                    Input.GetKey(KeyCode.RightArrow) || 
                    Input.GetKey(KeyCode.D)))
            {
                thursters[3].gameObject.SetActive(true);
                thursters[4].gameObject.SetActive(true);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                thursters[3].gameObject.SetActive(true);
                thursters[4].gameObject.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                thursters[3].gameObject.SetActive(false);
                thursters[4].gameObject.SetActive(true);
            }
        }
        else
        {
            thursters[3].gameObject.SetActive(false);
            thursters[4].gameObject.SetActive(false);
        }


        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rb2d.AddTorque(turningTorque);

            if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S))
                thursters[2].gameObject.SetActive(true);
            else
                thursters[2].gameObject.SetActive(false);
        }
        else
        {
            thursters[2].gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rb2d.AddTorque(-turningTorque);

            if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S))
                thursters[1].gameObject.SetActive(true);
            else
                thursters[1].gameObject.SetActive(false);
        }
        else
        {
            thursters[1].gameObject.SetActive(false);
        }

        if (closeDodgeDetector != null)
        {
            closeDodgeDetector.position = transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Bullet" && collision.gameObject.layer != 9)
        {
            if (!collidingObjects.Contains(collision))
                collidingObjects.Add(collision);

            if (!isTouchingSameColor || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.Play("TransparentAnimation");
                isTouchingSameColor = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Bullet" && collision.gameObject.layer != 9)
        {
            collidingObjects.Remove(collision);

            if (isTouchingSameColor && collidingObjects.Count == 0)
            {
                animator.Play("TransparentBackAnimation");
                isTouchingSameColor = false;
            }
        }
    }

    void ChangeColorMode(float delay)
    {
        // Instantiate the changeColorEffect prefab at the player's position
        GameObject changeColorEffect = Instantiate(
            changeColorEffects[currentColorMode],
            transform.position,
            Quaternion.identity,
            transform
        );
        Destroy(changeColorEffect, 0.2f);

        Invoke("ChangeSprite", delay);
    }

    void ChangeSprite()
    {
        sr.sprite = colorModes[currentColorMode];

        for (int i = 0; i < thursters.Length; i++)
        {
            thursters[i].localPosition = thrusterInformation[currentColorMode].positions[i];
            thursters[i].localRotation = Quaternion.Euler(thrusterInformation[currentColorMode].rotations[i]);
        }
    }
}
