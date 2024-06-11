using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] Transform[] thursters;
    [SerializeField] float accelerationForce, turningTorque, dragCoefficient;

    private float currentRotationAngle;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
