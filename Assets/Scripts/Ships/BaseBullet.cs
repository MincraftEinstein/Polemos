using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public float speed;
    public int damageAmount = 1;
    public bool isEnemyBullet;
    public Directions dirction;

    private GameManager gameManager;

    void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManager>()[0];
    }

    void Update()
    {
        if (dirction == Directions.Right)
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
        }

        Vector2 BGSize = gameManager.backgroundsFolder.GetComponent<BoxCollider2D>().size;
        float xBGHalf = BGSize.x / 2;
        float yBGHalf = BGSize.y / 2;

        Vector2 laserSize = GetComponent<BoxCollider2D>().size;
        float xLaserHalf = laserSize.x / 2;
        float yLaserHalf = laserSize.y / 2;

        // Right Bound
        if ((transform.position.x - xLaserHalf) > xBGHalf)
        {
            Destroy(gameObject);
        }

        // Left Bound
        if ((transform.position.x + xLaserHalf) < -xBGHalf)
        {
            Destroy(gameObject);
        }

        // Top Bound
        if ((transform.position.y - yLaserHalf) > yBGHalf)
        {
            Destroy(gameObject);
        }

        // Bottom Bound
        if ((transform.position.y + yLaserHalf) < -yBGHalf)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if (isEnemyBullet)
        {
            if (gObject.CompareTag("Player"))
            {
                gObject.GetComponent<PlayerManager>().RemoveHealth(damageAmount);
            }
        }
        else
        {
            if (gObject.CompareTag("Enemy") || gObject.CompareTag("BossEnemy"))
            {
                gObject.GetComponent<BaseEnemy>().RemoveHealth(damageAmount);
            }
            else if (gObject.CompareTag("EnemyShield"))
            {
                gObject.transform.parent.parent.GetComponent<BaseEnemy>().RemoveShieldHealth(gObject);
            }
        }

        if (!gObject.CompareTag("OrdagaExplosionTrigger"))
        {
            Destroy(gameObject);
        }
    }

    [Serializable]
    public enum Directions
    {
        Left,
        Right,
    }
}
