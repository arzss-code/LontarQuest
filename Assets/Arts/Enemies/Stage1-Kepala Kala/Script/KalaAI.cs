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
    public float chaseDistance = 15f; 
    public float attackDistance = 1.5f; 
    
    [Header("Arena Settings")]
    [Tooltip("Batas maksimal bos boleh mengejar, jika lewat akan kembali ke tengah")]
    public float leashRadius = 12f;
    private Vector2 startPosition;

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
    private enum State { Patrol, Chase, Charging, Cooldown, Return }
    private State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Buat massa sangat berat dan ganti ke Kinematic agar tidak bisa didorong player
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        
        startPosition = transform.position;

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            // SetActive(false) dihapus. Biar BossArenaController yang mengatur Parent UI-nya.
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
        float distanceFromStart = Vector2.Distance(transform.position, startPosition);

        // Jika keluar dari arena (leash), kembali ke posisi awal
        if (currentState == State.Return)
        {
            if (distanceFromStart < 1f)
            {
                currentState = State.Patrol;
            }
            else
            {
                movement = (startPosition - (Vector2)transform.position).normalized;
                UpdateAnimator(movement);
            }
            return;
        }

        if (distanceFromStart > leashRadius && (currentState == State.Chase || currentState == State.Patrol))
        {
            currentState = State.Return;
            return;
        }

        // Aktifkan boss jika pemain masuk jangkauan deteksi
        if (!hasEngaged && distanceToPlayer <= chaseDistance)
        {
            hasEngaged = true;
            // SetActive dihapus. BossArenaController yang akan memunculkan UI saat arena terkunci.
        }

        if (distanceToPlayer <= attackDistance && currentState != State.Charging && currentState != State.Cooldown)
        {
            StartCoroutine(SprayAttackRoutine());
        }
        else if (hasEngaged && distanceToPlayer <= chaseDistance)
        {
            // Sebagai Boss, dia harus terus mengejar selama pemain masih di dalam arena
            currentState = State.Chase;
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction;
        }
        else
        {
            // Pemain kabur jauh, bos mondar-mandir di tengah
            currentState = State.Patrol;
            
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                wanderTimer = wanderChangeTime;
                wanderDirection = Random.insideUnitCircle.normalized;
            }
            
            // Usahakan patroli tidak keluar dari titik awal
            Vector2 desiredPos = (Vector2)transform.position + wanderDirection;
            if (Vector2.Distance(desiredPos, startPosition) > (leashRadius * 0.5f))
            {
                wanderDirection = (startPosition - (Vector2)transform.position).normalized;
            }
            
            movement = wanderDirection;
        }

        UpdateAnimator(movement);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (currentState == State.Chase || currentState == State.Return)
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

        // Fase Ancang-ancang (Berkedip Putih, bukan merah)
        float timer = 0;
        bool isBlink = false;
        while (timer < chargeTime)
        {
            // Kedip antara warna asli (putih) dan abu-abu agak redup untuk efek charging
            spriteRenderer.color = isBlink ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
            isBlink = !isBlink;
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
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;
        Debug.Log("Bos terkena serangan! Sisa HP: " + currentHealth);
        
        // Memunculkan angka damage putih (false = musuh)
        DamagePopupManager.Create(transform.position, damageAmount, false);

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
        // Berkedip MERAH saat menerima damage (Sesuai permintaan)
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
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

        // SetActive dihapus. BossArenaController yang akan menghilangkan UI saat mendeteksi bos hancur.

        // (Opsional) Mainkan animasi mati atau jatuhkan item di sini

        Destroy(gameObject, 0.5f); // Hancur setelah setengah detik
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)transform.position, leashRadius);
    }
}