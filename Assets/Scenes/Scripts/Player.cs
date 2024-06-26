using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    private float x = 0;
    private float y = -3.5f;
    private int adjustX = 0;
    private int adjustY = 0;
    private int fireTimer = 0;
    public AudioClip laserSound;
    public AudioClip backgroundMusic;

    public Color damageColor, normalColor;
    private SpriteRenderer rend;
    private float damageTick;
    private bool damageTaken;

    public Image healthBar;

    public int fireRate = 0;
    public int maxHealth = 400;
    public int health = 400;
    public float speed;

    public bool shotSpreadA, bigBulletA, shieldA, meleeA;


    [SerializeField] GameObject bullet, bigBullet, shield, melee;
    public Transform bulletPos;
    [SerializeField] Image stats;
    
    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(backgroundMusic, Vector3.zero,0.1f);
        
        
    }

    // Update is called once per frame
    void Update()
    {

        if (damageTaken)
        {
            damageTick += Time.deltaTime;
        }

        if (damageTick >= 0.1f)
        {
            damageTaken = false;
            rend = GetComponent<SpriteRenderer>();
            rend.color = normalColor;
        }
        
        if (Input.GetKey(KeyCode.LeftArrow) && Time.timeScale == 1)
        {
            adjustX += -1;
        }
        
        if (Input.GetKey(KeyCode.RightArrow) && Time.timeScale == 1)
        {
            adjustX += 1;
        }

        if (Input.GetKey(KeyCode.UpArrow) && Time.timeScale == 1)
        {
            adjustY += 1;
        }

        if (Input.GetKey(KeyCode.DownArrow) && Time.timeScale == 1)
        {
            adjustY -= 1;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) && Time.timeScale == 1) adjustX = 0;
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) && Time.timeScale == 1) adjustY = 0;

        if (Input.GetKey(KeyCode.Space) && Time.timeScale == 1)
        {
            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0;
            }
        }

        if (health <= 0)
        {
            Destroy(gameObject);
           int s =  GameObject.FindGameObjectWithTag("score").GetComponent<Game>().score;
            ScoreDisplay.score = s;
            SceneManager.LoadScene("GameOver");
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            stats.enabled = !stats.enabled;  
        }
    }
    
    void Shoot()
    {
        if (!meleeA)
        {
            if (!bigBulletA)
            {
                Instantiate(bullet, bulletPos.position + Vector3.up / 1.5f, Quaternion.identity);
                AudioSource.PlayClipAtPoint(laserSound, Vector3.zero, 1);

                if (shotSpreadA)
                {
                    Instantiate(bullet, bulletPos.position + Vector3.right / 4f + Vector3.up / 2f, Quaternion.identity);
                    Instantiate(bullet, bulletPos.position + Vector3.left / 4f + Vector3.up / 2f, Quaternion.identity);
                }
            }

            if (bigBulletA)
            {
                Instantiate(bigBullet, bulletPos.position + Vector3.up / 1.5f, Quaternion.identity);
                AudioSource.PlayClipAtPoint(laserSound, Vector3.zero, 1);

                if (shotSpreadA)
                {
                    Instantiate(bigBullet, bulletPos.position + Vector3.right / 4f + Vector3.up / 2f,
                        Quaternion.identity);
                    Instantiate(bigBullet, bulletPos.position + Vector3.left / 4f + Vector3.up / 2f,
                        Quaternion.identity);
                }
            }
        }

        if (meleeA)
        {
            Instantiate(melee, bulletPos.position, Quaternion.identity);
        }
    }

    private bool shieldUp = false;
    private void FixedUpdate()
    {
        
        fireTimer += 1;
        
        if (adjustX > 0) x += 0.052f * speed;
        if (adjustX > 50) x += 0.025f * speed;
        if (adjustX > 70) x += 0.015f * speed;
        if (adjustX > 125) x += 0.015f * speed;
        if (adjustX < 0) x += -0.052f * speed;
        if (adjustX < -50) x += -0.025f * speed;
        if (adjustX < -70) x += -0.015f * speed;
        if (adjustX < -125) x += -0.015f * speed;
        
        if (adjustY > 0) y += 0.052f * speed;
        if (adjustY > 50) y += 0.025f * speed;
        if (adjustY > 70) y += 0.015f * speed;
        if (adjustY > 125) y += 0.015f * speed;
        if (adjustY < 0) y += -0.052f * speed;
        if (adjustY < -50) y += -0.025f * speed;
        if (adjustY < -70) y += -0.015f * speed;
        if (adjustY < -125) y += -0.015f * speed;

        if (x > 5.12) x = 5.12f;
        if (x < -5.12) x = -5.12f;
        if (y > 4.22) y = 4.22f;
        if (y < -4.22) y = -4.22f;

        if (shieldA && !shieldUp)
        {
            shieldUp = true;
            Instantiate(shield, bulletPos.position, Quaternion.identity);
        }
        transform.position = new Vector3(x, y, -1f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            if (shieldUp)
            {
                Destroy(GameObject.FindWithTag("Shield"));
                shieldUp = false;
                shieldA = false;
            }
            else
            {
                health -= 100;
                damageTick = 0;
                damageTaken = true;
                rend = GetComponent<SpriteRenderer>();
                rend.color = damageColor;
            }
            
        }

        if (other.gameObject.CompareTag("HealthDrop"))
        {
            if (health < maxHealth) health += 100;
            Destroy(other.gameObject);
        }
    }
}
