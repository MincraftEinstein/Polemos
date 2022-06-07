using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : BaseEnemy
{
    public bool hasGuns;
    public GameObject bullets;
    public AudioClip gunFireSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (hasGuns)
        {
            StartCoroutine(FireGunBullets());
        }
    }

    protected override void Update()
    {
        base.Update();
        transform.Translate(speed * Time.deltaTime * Vector2.left);

        float xBorder = xBGHalf + 3;
        float yBorder = yBGHalf + 3;

        // Right Bound
        if ((transform.position.x + shipHalfX) > xBorder)
        {
            Die();
        }

        // Left Bound
        if ((transform.position.x - shipHalfX) < -xBorder)
        {
            Die();
        }

        // Top Bound
        if ((transform.position.y + shipHalfY) > yBorder)
        {
            Die();
        }

        // Bottom Bound
        if ((transform.position.y - shipHalfY) < -yBorder)
        {
            Die();
        }
    }

    private IEnumerator FireGunBullets()
    {
        while (gameManager.gameOver == false && canFireGuns)
        {
            yield return new WaitForSeconds(0.5F);
            if (IsOutOfBounds() == false)
            {
                int chance = Random.Range(0, 101);
                if (chance <= 25)
                {
                    Instantiate(bullets, transform.position, transform.rotation, bulletFolder.transform);
                    audioSource.PlayOneShot(gunFireSound);
                } 
            }
        }
    }
}
