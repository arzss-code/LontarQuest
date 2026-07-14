using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Tooltip("Kecepatan luncur panah")]
    public float speed = 10f;

    [SerializeField]
    float lifeTime = 5f;

    Transform target;
    private int currentDamage = 15;
    private bool hasFireEffect = false;

    void Start()
    {
        Destroy(
            gameObject,
            lifeTime
        );
    }

    public void SetTarget(Transform newTarget, int damageValue, bool isFire = false)
    {
        target = newTarget;
        currentDamage = damageValue;
        hasFireEffect = isFire;
    }

    void Update()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        // sedikit naik ke tengah badan target
        Transform aimPoint =
        target.Find("AimPoint");

        Vector2 targetPosition;

        if(aimPoint != null)
        {
            targetPosition =
            aimPoint.position;
        }
        else
        {
            targetPosition =
            target.position;
        }

        Vector2 direction =
        (
            targetPosition -
            (Vector2)transform.position
        ).normalized;


        // gerak
        transform.position +=
        (Vector3)
        (
            direction *
            speed *
            Time.deltaTime
        );


        // rotasi
        float angle =
        Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;


        // sprite panah menghadap atas
        transform.rotation =
        Quaternion.Euler(
            0,
            0,
            angle - 90f
        );
    }

    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if(other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            return;
        }

        hasHit = true;

        // Otomatis mencari fungsi TakeDamage di semua musuh (Kala, Gana, dll)
        other.SendMessageUpwards("TakeDamage", currentDamage, SendMessageOptions.DontRequireReceiver);

        if (hasFireEffect)
        {
            BurnEffect existingBurn = other.GetComponent<BurnEffect>();
            if (existingBurn == null)
            {
                BurnEffect burn = other.gameObject.AddComponent<BurnEffect>();
                burn.StartBurn(5, 4); // 5 damage per detik selama 4 detik
            }
            else
            {
                existingBurn.StopAllCoroutines();
                existingBurn.StartBurn(5, 4); // Reset durasi api
            }
        }

        Destroy(gameObject);
    }
}