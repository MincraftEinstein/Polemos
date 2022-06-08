using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ordaga4Manager : BaseEnemy
{
    public AudioClip detonationSound;
    public Sprite[] sprites;

    private float baseSize;
    private bool canDetontate = true;
    private GameObject detonation;
    private BoxCollider2D detonationCollider;
    private SpriteRenderer detonationSprite;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        detonation = transform.GetChild(1).gameObject;
        detonationCollider = detonation.GetComponent<BoxCollider2D>();
        detonationSprite = detonation.GetComponent<SpriteRenderer>();
        baseSize = detonationCollider.size.x / sprites.Length;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180));
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);

        if (gameManager.gameOver == false)
        {
            RotateObjectToObject(gameObject.transform, player.transform, 25);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canDetontate)
        {
            canDetontate = false;
            boxCollider.enabled = false;
            sprite.enabled = false;
            StartCoroutine(Detonate());
        }
    }

    public IEnumerator Detonate()
    {
        float time = 19 / sprites.Length;
        audioSource.PlayOneShot(detonationSound);
        detonation.SetActive(true);
        for (int i = 0; i < sprites.Length; i++)
        {
            Vector2 size = new Vector2(baseSize * i, baseSize * i);
            detonationCollider.size = size;
            detonationSprite.sprite = sprites[i];
            yield return new WaitForSeconds(time);
        }
        Die();
    }
}
