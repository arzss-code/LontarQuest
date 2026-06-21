using UnityEngine;
using System.Collections;

public class BurnEffect : MonoBehaviour
{
    private int damagePerSecond = 5;
    private int duration = 3;
    
    public void StartBurn(int dps, int time)
    {
        damagePerSecond = dps;
        duration = time;
        StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForSeconds(1f);
            
            // Berikan damage
            gameObject.SendMessageUpwards("TakeDamage", damagePerSecond, SendMessageOptions.DontRequireReceiver);
            
            // Efek visual: kedip merah
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                Color orig = sr.color;
                sr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                sr.color = orig;
            }
        }
        
        // Hapus skrip ini dari musuh setelah efek selesai
        Destroy(this);
    }
}
