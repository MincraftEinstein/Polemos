using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakarBoss : BaseBoss
{
    public GameObject backupEnemy;
    public GameObject bullet;
    public AudioClip gunFireSound;
    public Vector2 rightBulletPosition;
    public Vector2 leftBulletPosition;

    private bool isInPosition;
    private bool isMoving;
    private bool isTouchingTopEdge;
    private bool isTouchingBottomEdge;
    private int direction;
    private Vector2 destination;

    // Start is called before the first frame update
    protected override void Start()
    {
        shieldTurretSize = 0.5F;
        base.Start();
        shieldTurretHealth = 3;
        shieldRespawnInterval = 12;
        explosionSize = 3;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isInPosition)
        {
            if (isMoving)
            {
                if (direction == 0)
                {
                    transform.Translate(speed * Time.deltaTime * Vector2.up);

                    if (transform.position.y > destination.y)
                    {
                        transform.position = new Vector2(transform.position.x, destination.y);
                        isMoving = false;
                    }
                }
                else
                {
                    transform.Translate(speed * Time.deltaTime * Vector2.down);

                    if (transform.position.y < destination.y)
                    {
                        transform.position = new Vector2(transform.position.x, destination.y);
                        isMoving = false;
                    }
                }
            }

            if ((transform.position.y + shipHalfY) > yBGHalf)
            {
                transform.position = new Vector2(transform.position.x, yBGHalf - shipHalfY);
                isMoving = false;
                isTouchingTopEdge = true;
            }
            else
            {
                isTouchingTopEdge = false;
            }

            if ((transform.position.y - shipHalfY) < -yBGHalf)
            {
                transform.position = new Vector2(transform.position.x, -yBGHalf + shipHalfY);
                isMoving = false;
                isTouchingBottomEdge = true;
            }
            else
            {
                isTouchingBottomEdge = false;
            }
        }
        else
        {
            transform.Translate(speed * Time.deltaTime * Vector2.left);

            if (transform.position.x < ((xBGHalf / 4) * 3))
            {
                isInPosition = true;
                canFireGuns = true;
                canFireTurrets = true;
                canTakeDamage = true;
                canShieldsTakeDamage = true;

                StartCoroutine(FireGunBullets());
                StartCoroutine(FireTurretBullets());
                StartCoroutine(DestinationController());
            }
        }
    }

    private IEnumerator DestinationController()
    {
        while (gameManager.gameOver == false)
        {
            if (isMoving == false)
            {
                yield return new WaitForSeconds(Random.Range(0, 4));
                if (isTouchingTopEdge)
                {
                    SetMoving(1);
                }
                else if (isTouchingBottomEdge)
                {
                    SetMoving(0);
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        SetMoving(1);
                    }
                    else
                    {
                        SetMoving(0);
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1F);
            }
        }
    }

    private void SetMoving(int direction)
    {
        float distance;
        if (direction == 0)
        {
            distance = Random.Range(yBGHalf, transform.position.y + shipHalfY);
        }
        else
        {
            distance = Random.Range(-yBGHalf, transform.position.y - shipHalfY);
        }

        destination = new Vector2(transform.position.x, distance);
        isMoving = true;
        this.direction = direction;
    }

    private IEnumerator FireGunBullets()
    {
        while (gameManager.gameOver == false && canFireGuns)
        {
            CreateBullet(leftBulletPosition);
            CreateBullet(rightBulletPosition);
            yield return new WaitForSeconds(Random.Range(2, 5));
        }
    }

    private void CreateBullet(Vector2 position)
    {
        Instantiate(bullet, new Vector2(transform.position.x + position.x, transform.position.y + position.y), transform.rotation, bulletFolder.transform);
        audioSource.PlayOneShot(gunFireSound);
    }
}
