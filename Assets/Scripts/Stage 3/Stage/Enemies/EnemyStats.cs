using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] private int maxHP = 85;

    private int currentHP;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    //--------------------------------------------------

    [Header("Damage Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.12f;

    private Color originalColor;

    //--------------------------------------------------

    [Header("Death")]
    [SerializeField] private float destroyDelay = 0.25f;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.75f;

    [Header("UI")]
    [SerializeField] private EnemyHealthBar healthBar;

    //--------------------------------------------------
    // Components
    //--------------------------------------------------

    private EnemyMovement movement;
    private EnemyAttack attack;
    private EnemyDetection detection;
    public bool IsDead => isDead;

    private bool isDead;

    //--------------------------------------------------

    private void Awake()
    {

        healthBar.Initialize(maxHP);
        currentHP = maxHP;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        detection = GetComponentInChildren<EnemyDetection>();
    }

    //--------------------------------------------------

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        currentHP = Mathf.Clamp(
            currentHP,
            0,
            maxHP);

        DamagePopupManager.Create(
            transform.position,
            damage,
            false);

        StartCoroutine(DamageFlashRoutine());
        

        if (currentHP <= 0)
        {
            Die();
        }

        healthBar.SetHealth(currentHP);
    }

    //--------------------------------------------------

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHP += amount;

        currentHP = Mathf.Clamp(
            currentHP,
            0,
            maxHP);
    }

    //--------------------------------------------------

    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
    }

    //--------------------------------------------------

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        //----------------------------------
        // Laporkan ke Quest
        //----------------------------------

        if (Stage3QuestManager.Instance != null)
        {
            Stage3QuestManager.Instance.RegisterEnemyKill();
        }

        //----------------------------------
        // Disable AI
        //----------------------------------

        if (movement != null)
            movement.enabled = false;

        if (attack != null)
            attack.enabled = false;

        if (detection != null)
            detection.enabled = false;

        //----------------------------------
        // Disable Collider
        //----------------------------------

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        //----------------------------------

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Color color = spriteRenderer.color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(
                1f,
                0f,
                timer / fadeDuration);

            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}