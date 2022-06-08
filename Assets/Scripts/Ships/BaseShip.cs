using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseShip : MonoBehaviour
{
    public int health;
    public bool canTakeDamage = true;

    protected int currentHealth;
    protected float explosionSize = 1;
    protected float xBGHalf;
    protected float yBGHalf;
    protected float shipHalfX;
    protected float shipHalfY;
    public GameManager gameManager;
    protected AudioSource audioSource;
    protected RectTransform backgroundRect;
    protected GameObject bulletFolder;
    protected Vector2 BGSize;
    protected BoxCollider2D boxCollider;
    protected SpriteRenderer sprite;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameManager = Resources.FindObjectsOfTypeAll<GameManager>()[0];
        BGSize = gameManager.backgroundsFolder.GetComponent<BoxCollider2D>().size;
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        backgroundRect = gameManager.backgroundRect;
        bulletFolder = gameManager.bulletFolder;
        audioSource = gameManager.audioSource;
        currentHealth = health;
        xBGHalf = BGSize.x / 2;
        yBGHalf = BGSize.y / 2;
        shipHalfX = (boxCollider.size.x / 2) + 0.1F;
        shipHalfY = (boxCollider.size.y / 2) + 0.1F;

        if (health < 1)
        {
            Debug.LogError(gameObject.name + " health can not be less than 1");
        }
    }

    public void AddHealth(int amount)
    {
        if ((currentHealth + amount) <= health)
        {
            currentHealth += amount;
        }
        OnHealthAdded();
    }

    protected virtual void OnHealthAdded() { }

    public virtual void RemoveHealth(int amount)
    {
        if (canTakeDamage && gameManager.gameOver == false)
        {
            canTakeDamage = false;
            currentHealth -= amount;

            OnHealthRemoved();

            if (currentHealth > 0)
            {
                StartCoroutine(FlashShip());
            }
            else
            {
                OnDeath();
                gameManager.Kill(gameObject, explosionSize);
            }
        }
    }

    protected virtual void OnHealthRemoved() { }

    protected virtual void OnDeath() { }

    private IEnumerator FlashShip()
    {
        if (CompareTag("Player"))
        {
            yield return StartCoroutine(FlashRed());
        }
        else
        {
            StartCoroutine(FlashRed());
        }
        canTakeDamage = true;
    }

    protected IEnumerator FlashRed()
    {
        ChangeShipColor(Color.red);
        yield return new WaitForSeconds(0.2F);
        ChangeShipColor(Color.white);
    }

    protected void ChangeShipColor(Color color)
    {
        sprite.color = color;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.CompareTag("OrdagaExplosionTrigger"))
            {
                child.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    /// <summary>
    /// Incrimentaly rotates 'rotatable' towards the 'target' at the given 'speed'. Must be continisly updated.
    /// </summary>
    /// <param name="rotatable"></param>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    protected void RotateObjectToObject(Transform rotatable, Transform target, float speed)
    {
        float angle = Mathf.Atan2(target.position.y - rotatable.position.y, target.position.x - rotatable.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rotatable.rotation = Quaternion.RotateTowards(rotatable.rotation, targetRotation, speed * Time.deltaTime);
    }
}
