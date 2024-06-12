using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] float bulletSpeed;

    private float bulletShootingAngle;

    void Update()
    {
        transform.position = new Vector2(
            transform.position.x + -bulletSpeed * Mathf.Sin(bulletShootingAngle) * Time.deltaTime, 
            transform.position.y + bulletSpeed * Mathf.Cos(bulletShootingAngle) * Time.deltaTime
        );

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.0 || pos.x > 1.0 || pos.y < 0.0 || pos.y > 1.0)
        {
            Destroy(gameObject);
        }
    }

    public void SetBulletShootingAngle(float angle)
    {
        bulletShootingAngle = angle;
    }
}
