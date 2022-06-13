using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdagaBoss : BaseBoss
{
    public int chargeSpeed;
    public GameObject teleportPoints;

    private int lastPointSet;
    private bool isInPosition;
    private bool canCharge;
    private bool canTeleport;
    private bool canInflictCollsionDamage = true;
    private Transform points;
    private Vector2 startPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        explosionSize = 2.5F;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180));
        points = Instantiate(teleportPoints, gameManager.enemyFolder.transform.position, Quaternion.identity, gameManager.transform).transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (isInPosition)
        {
            if (canTeleport)
            {
                StartCoroutine(TeleportRandomly());
            }

            if (canCharge)
            {
                transform.Translate(chargeSpeed * Time.deltaTime * Vector2.right);
            }
        }
        else
        {
            transform.Translate(speed * Time.deltaTime * Vector2.right);

            if (transform.position.x < (xBGHalf / 2))
            {
                StartCoroutine(OnceInPosition());
            }
        }

        if (IsOutOfBounds())
        {
            turretRotationSpeed = 300;
        }
        else
        {
            turretRotationSpeed = 100;
        }
    }

    private IEnumerator OnceInPosition()
    {
        isInPosition = true;
        canFireGuns = true;
        canFireTurrets = true;
        canTakeDamage = true;
        canShieldsTakeDamage = true;

        StartCoroutine(FireTurretBullets());

        yield return new WaitForSeconds(2);

        canCharge = true;
    }

    private IEnumerator TeleportRandomly()
    {
        if (gameManager.gameOver == false)
        {
            canTeleport = false;
            Transform currentPointSet;

            choosePointSet:
            int nextPointSet = Random.Range(0, points.childCount);
            if (nextPointSet != lastPointSet)
            {
                currentPointSet = points.GetChild(nextPointSet);
                lastPointSet = nextPointSet;
            }
            else
            {
                goto choosePointSet;
            }

            int i = Random.Range(0, 2);
            Transform startGO = currentPointSet.GetChild(i);
            startPos = startGO.position;
            yield return new WaitForSeconds(2);
            StartCoroutine(BlinkCollider(startGO.GetComponent<BoxCollider2D>()));
            transform.SetPositionAndRotation(new Vector2(startPos.x, startPos.y), startGO.transform.rotation);
            canCharge = true;
        }
    }

    private IEnumerator BlinkCollider(BoxCollider2D collider2D)
    {
        collider2D.enabled = false;
        yield return new WaitForSeconds(1);
        collider2D.enabled = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        Destroy(points.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGO = collision.gameObject;
        if (collisionGO.name.Contains("Point"))
        {
            canCharge = false;
            canTeleport = true;
        }
        if (collisionGO.CompareTag("Player"))
        {
            canFireTurrets = false;
            if (canInflictCollsionDamage)
            {
                canInflictCollsionDamage = false;
                collisionGO.GetComponent<PlayerManager>().RemoveHealth(3);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canInflictCollsionDamage = true;
            canFireTurrets = true;
            StartCoroutine(FireTurretBullets());
        }
    }
}
