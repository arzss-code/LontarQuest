using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    public float expandSpeed = 10f; // Kecepatan lingkaran membesar
    public float lifeTime = 0.3f;   // Berapa lama efek ini muncul sebelum hancur

    void Start()
    {
        // Otomatis menghancurkan diri setelah beberapa detik
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Membuat lingkaran membesar seiring waktu seperti gelombang kejut
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
    }
}
