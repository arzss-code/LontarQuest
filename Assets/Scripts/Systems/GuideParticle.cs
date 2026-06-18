using UnityEngine;
using System.Collections;

public class GuideParticle : MonoBehaviour
{
    [Header("Pengaturan Terbang")]
    [Tooltip("Kecepatan partikel melayang")]
    public float speed = 5f;
    [Tooltip("Waktu diam (detik) sebelum terbang, agar terlihat jelas")]
    public float waitBeforeFlying = 1f;
    [Tooltip("Lengkungan terbang agar natural (tidak lurus kaku)")]
    public float heightCurve = 1.5f; 

    private Vector2 startPos;
    private Transform target;

    public void Init(Transform targetTransform)
    {
        target = targetTransform;
        startPos = transform.position;
        StartCoroutine(FlyToTarget());
    }

    private IEnumerator FlyToTarget()
    {
        // 1. Diam sejenak agar pemain sadar ada partikel muncul
        yield return new WaitForSeconds(waitBeforeFlying);

        if (target == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float journeyLength = Vector2.Distance(startPos, target.position);
        float startTime = Time.time;
        float distanceCovered = 0f;

        // 2. Proses Terbang
        while (distanceCovered < journeyLength)
        {
            if (target == null) break;

            distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Interpolasi posisi lurus dari Saka ke Pintu Keluar
            Vector2 currentPos = Vector2.Lerp(startPos, target.position, fractionOfJourney);
            transform.position = currentPos;

            // Berhenti jika sudah sangat dekat dengan target
            if (fractionOfJourney >= 0.95f)
            {
                break;
            }

            yield return null;
        }

        // 3. Menghilang Perlahan
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // Matikan sumber debu
            var emission = ps.emission;
            emission.enabled = false;
            
            // Hancurkan objek setelah debu terakhir memudar (Sesuai umur partikel)
            Destroy(gameObject, ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
