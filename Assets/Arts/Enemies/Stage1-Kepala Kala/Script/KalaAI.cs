using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class KalaAI : MonoBehaviour
{
    [Header("Status Bos")]
    public int maxHealth = 150;
    private int currentHealth;
    public float moveSpeed = 3f;

    [Header("UI")]
    public Slider healthBar;

    [Header("Target & Deteksi")]
    public Transform player;
    public LayerMask playerLayer; 
    public float chaseDistance = 8f; 
    public float attackDistance = 1.5f; 

    [Header("Patroli")]
    public float wanderSpeed = 1.5f;
    public float wanderChangeTime = 3f;
    private float wanderTimer;
    private Vector2 wanderDirection; 

    [Header("Efek Visual")]
    public GameObject explosionPrefab; // Seret prefab ledakan ke sini nanti

    [Header("Serangan Ledakan (AoE)")]
    public float chargeTime = 1f; // Waktu ancang-ancang sebelum meledak
    public float attackRadius = 2f; 
    public int damage = 20;
    public float attackCooldown = 2f; // Jeda sebelum bisa meledak lagi

    [Header("Audio SFX")]
    public AudioClip chargeSFX;
    public AudioClip explodeSFX;
    public AudioClip hitSFX;
    public AudioClip deathSFX;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Vector2 movement;
    private bool isDead = false;
    private bool hasEngaged = false;

    // State Machine
    private enum State { Patrol, Chase, Charging, Cooldown }
    private State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Buat massa sangat berat agar tidak bisa didorong oleh player
        rb.mass = 1000f;
        // Kunci rotasi agar boss tidak terguling
        rb.freezeRotation = true;

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(false);
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        currentState = State.Patrol;
        wanderTimer = wanderChangeTime;
        wanderDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Bos sedang ancang-ancang atau pendinginan, hentikan pergerakan
        if (currentState == State.Charging || currentState == State.Cooldown) 
        {
            movement = Vector2.zero;
            UpdateAnimator(movement);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!hasEngaged && distanceToPlayer <= chaseDistance)
        {
            hasEngaged = true;
            if (healthBar != null) healthBar.gameObject.SetActive(true);
        }

        if (distanceToPlayer <= attackDistance && currentState != State.Charging && currentState != State.Cooldown)
        {
            StartCoroutine(SprayAttackRoutine());
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            currentState = State.Chase;
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction;
        }
        else
        {
            currentState = State.Patrol;
            
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                wanderTimer = wanderChangeTime;
                wanderDirection = Random.insideUnitCircle.normalized;
            }
            movement = wanderDirection;
        }

        UpdateAnimator(movement);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (currentState == State.Chase)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else if (currentState == State.Patrol)
        {
            rb.MovePosition(rb.position + movement * wanderSpeed * Time.fixedDeltaTime);
        }
    }

    private float lastMoveX;
    private float lastMoveY;

    void UpdateAnimator(Vector2 dir)
    {
        if (dir.magnitude > 0)
        {
            // Tentukan arah dominan (kiri/kanan ATAU atas/bawah)
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                lastMoveX = Mathf.Sign(dir.x);
                lastMoveY = 0;
            }
            else
            {
                lastMoveX = 0;
                lastMoveY = Mathf.Sign(dir.y);
            }

            anim.SetFloat("MoveX", lastMoveX);
            anim.SetFloat("MoveY", lastMoveY);
            
            // Set LastMove jika Animator membutuhkannya saat diam
            anim.SetFloat("LastMoveX", lastMoveX);
            anim.SetFloat("LastMoveY", lastMoveY);
        }
    }

    // --- SISTEM SERANGAN BOS ---
    IEnumerator SprayAttackRoutine()
    {
        currentState = State.Charging;
        rb.linearVelocity = Vector2.zero;

        // Kunci arah serangan ke pemain saat mulai ancang-ancang
        Vector2 attackDirection = (player.position - transform.position).normalized;
        
        // Update animasi agar bos menghadap ke arah serangan
        UpdateAnimator(attackDirection);

        if (chargeSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargeSFX);
        }

        // Fase Ancang-ancang (Berkedip Merah)
        float timer = 0;
        bool isRed = false;
        while (timer < chargeTime)
        {
            spriteRenderer.color = isRed ? Color.white : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(0.15f);
            timer += 0.15f;
        }
        spriteRenderer.color = Color.white; 

        // Tentukan titik tengah semburan (di depan boss)
        Vector2 sprayCenter = (Vector2)transform.position + attackDirection * (attackRadius * 0.75f);

        if (explodeSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(explodeSFX);
        }

        // --- MASUKKAN KODE INI DI SINI ---
        if (explosionPrefab != null)
        {
            // Rotasikan efek agar menghadap arah semburan
            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            Instantiate(explosionPrefab, sprayCenter, Quaternion.Euler(0, 0, angle));
        }
        // ---------------------------------

        // Fase Ledakan/Semburan (Damage)
        Collider2D[] hits = Physics2D.OverlapCircleAll(sprayCenter, attackRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("TERKENA LEDAKAN BOS! Saka menerima " + damage + " damage!");
                PlayerStats playerStats = hit.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                }

                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                    playerController.ApplyKnockback(knockbackDir);
                }
                
                // Cukup kenai damage satu kali ke player
                break;
            }
        }

        // Fase Cooldown (Diam sejenak setelah meledak)
        currentState = State.Cooldown;
        yield return new WaitForSeconds(attackCooldown);
        
        // Kembali ke rutinitas awal
        currentState = State.Patrol;
    }

    // --- SISTEM HP & MENERIMA SERANGAN ---
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("Bos terkena serangan! Sisa HP: " + currentHealth);

        if (hitSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSFX);
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        // Berkedip sebentar saat menerima damage dari Saka
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        isDead = true;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Kepala Kala Berhasil Dikalahkan!");
        
        if (deathSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSFX);
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // (Opsional) Mainkan animasi mati atau jatuhkan item di sini

        Destroy(gameObject, 0.5f); // Hancur setelah setengah detik
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}