﻿using System.Collections;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [System.Serializable]
    public class ThrusterInformation
    {
        public Vector3[] positions, rotations;
    }

    [SerializeField] Transform[] thursters;
    [SerializeField] ThrusterInformation[] thrusterInformation;
    [SerializeField] Sprite[] colorModes;
    [SerializeField] GameObject[] colorBullets, changeColorEffects;
    [SerializeField] float accelerationForce, turningTorque, dragCoefficient, bulletOffsetMultiplier;

    private float currentRotationAngle;
    private int currentColorMode;
    private SpriteRenderer sr;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentColorMode++;

            if (currentColorMode == colorModes.Length)
            {
                currentColorMode = 0;
            }

            ChangeColorMode(0.2f, currentColorMode);
        }
        /*else if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeColorMode(0.2f, 1);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeColorMode(0.2f, 2);
        }*/

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 bulletOffset = new Vector2(-Mathf.Sin(currentRotationAngle), Mathf.Cos(currentRotationAngle)) * bulletOffsetMultiplier;

            GameObject colorBullet = Instantiate(
                colorBullets[currentColorMode],
                transform.position + (Vector3)bulletOffset,
                Quaternion.Euler(0, 0, transform.eulerAngles.z)
            );

            BulletManager bulletManager = colorBullet.GetComponent<BulletManager>();
            bulletManager.SetBulletShootingAngle(currentRotationAngle);
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
    }

    void ChangeColorMode(float delay, int mode)
    {
        // Instantiate the changeColorEffect prefab at the player's position
        GameObject changeColorEffect = Instantiate(
            changeColorEffects[mode],
            transform.position,
            Quaternion.identity,
            transform
        );
        Destroy(changeColorEffect, 0.2f);

        StartCoroutine(ChangeSprite(delay, mode));
    }

    IEnumerator ChangeSprite(float delay, int mode)
    {
        yield return new WaitForSeconds(delay);
        currentColorMode = mode;
        sr.sprite = colorModes[currentColorMode];

        for (int i = 0; i < thursters.Length; i++)
        {
            thursters[i].localPosition = thrusterInformation[currentColorMode].positions[i];
            thursters[i].localRotation = Quaternion.Euler(thrusterInformation[currentColorMode].rotations[i]);
        }
    }
}
