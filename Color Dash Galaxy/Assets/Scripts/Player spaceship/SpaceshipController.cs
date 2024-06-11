using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] Transform[] thursters;
    [SerializeField] Sprite[] colorModes;
    [SerializeField] GameObject changeColorEffect;
    [SerializeField] float accelerationForce, turningTorque, dragCoefficient;

    private float currentRotationAngle;
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
        if (Input.GetKey(KeyCode.R))
        {
            // Instantiate the changeColorEffect prefab at the player's position
            GameObject colorEffect = Instantiate(changeColorEffect, transform.position, Quaternion.identity);
            Destroy(colorEffect, 0.2f);

            sr.sprite = colorModes[0];
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // Instantiate the changeColorEffect prefab at the player's position
            GameObject colorEffect = Instantiate(changeColorEffect, transform.position, Quaternion.identity);
            Destroy(colorEffect, 0.2f);

            sr.sprite = colorModes[1];
        }
        else if (Input.GetKey(KeyCode.F))
        {
            // Instantiate the changeColorEffect prefab at the player's position
            GameObject colorEffect = Instantiate(changeColorEffect, transform.position, Quaternion.identity);
            Destroy(colorEffect, 0.2f);

            sr.sprite = colorModes[2];
        }
    }
    void FixedUpdate()
    {
        currentRotationAngle = Mathf.Deg2Rad * transform.eulerAngles.z;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rb2d.AddForce(new Vector2(-accelerationForce * Mathf.Sin(currentRotationAngle),
                accelerationForce * Mathf.Cos(currentRotationAngle)));

            thursters[0].gameObject.SetActive(true);
        }
        else
        {
            thursters[0].gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rb2d.AddForce(new Vector2(accelerationForce * Mathf.Sin(currentRotationAngle),
                -accelerationForce * Mathf.Cos(currentRotationAngle)));

            thursters[3].gameObject.SetActive(true);
            thursters[4].gameObject.SetActive(true);
        }
        else
        {
            thursters[3].gameObject.SetActive(false);
            thursters[4].gameObject.SetActive(false);
        }


        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rb2d.AddTorque(turningTorque);

            thursters[1].gameObject.SetActive(true);
        }
        else
        {
            thursters[1].gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rb2d.AddTorque(-turningTorque);

            thursters[2].gameObject.SetActive(true);
        }
        else
        {
            thursters[2].gameObject.SetActive(false);
        }
    }
}
