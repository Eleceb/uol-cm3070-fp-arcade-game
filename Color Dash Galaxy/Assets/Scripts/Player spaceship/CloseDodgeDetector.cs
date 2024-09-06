using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDodgeDetector : MonoBehaviour
{
    [SerializeField] int closeDodgeScorePerSecond;

    LevelsManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelsManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
            levelManager.UpdateScore(closeDodgeScorePerSecond);
    }
}
