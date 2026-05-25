using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField]
    float speed = 10f;

    [SerializeField]
    int damage = 25;

    [SerializeField]
    float lifeTime = 5f;

    Transform target;

    void Start()
    {
        Destroy(
            gameObject,
            lifeTime
        );
    }

    public void SetTarget(
    Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if(target == null)
        {
            Destroy(
                gameObject
            );

            return;
        }

        Vector2 direction =

        (
            target.position -
            transform.position
        ).normalized;


        transform.position +=

        (Vector3)
        (
            direction *
            speed *
            Time.deltaTime
        );


        float angle =

        Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;


        transform.rotation =
        Quaternion.Euler(
            0,
            0,
            angle
        );
    }

    private void OnTriggerEnter2D(
    Collider2D other)
    {
        if(
        other.gameObject.layer !=
        LayerMask.NameToLayer(
        "Enemy"))
        {
            return;
        }

        DamageBuddyDamageTracker
        buddy =

        other
        .GetComponentInParent
        <DamageBuddyDamageTracker>();


        if(buddy != null)
        {
            buddy.TakeDamage(
                damage
            );

            Destroy(
                gameObject
            );
        }
    }
}