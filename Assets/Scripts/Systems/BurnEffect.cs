using UnityEngine;
using System.Collections;

public class BurnEffect : MonoBehaviour
{
    private int damagePerSecond = 5;
    private int duration = 3;
    private Color originalColor = Color.white;
    private SpriteRenderer sr;
    private bool hasOriginalColor = false;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
            hasOriginalColor = true;
        }
    }

    public void StartBurn(int dps, int time)
    {
        damagePerSecond = dps;
        duration = time;
        
        // Jaga-jaga kalau sprite renderer baru di-assign/diubah warnanya
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null && !hasOriginalColor)
            {
                originalColor = sr.color;
                hasOriginalColor = true;
            }
        }

        StopAllCoroutines();
        // Kembalikan ke warna asli sebelum memulai coroutine baru agar tidak menumpuk status warna merah
        if (sr != null && hasOriginalColor)
        {
            sr.color = originalColor;
        }
        StartCoroutine(BurnRoutine());
    }

    private void OnDestroy()
    {
        // Pastikan warna dikembalikan saat komponen dihancurkan
        if (sr != null && hasOriginalColor)
        {
            sr.color = originalColor;
        }
    }

    private IEnumerator BurnRoutine()
    {
        for (int i = 0; i < duration; i++)
        {
            // Ambil damage setiap awal detik
            gameObject.SendMessageUpwards("TakeDamage", damagePerSecond, SendMessageOptions.DontRequireReceiver);
            
            if (sr != null)
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.15f);
                sr.color = originalColor;
                yield return new WaitForSeconds(0.85f);
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
            }
        }
        
        // Hapus skrip ini dari musuh setelah efek selesai
        Destroy(this);
    }
}
