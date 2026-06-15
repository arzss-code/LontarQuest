using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    private TextMesh textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    /// <summary>
    /// Buat teks damage melayang secara otomatis tanpa perlu Prefab!
    /// </summary>
    public static void Create(Vector3 position, int damageAmount, bool isPlayerDamage)
    {
        // Buat GameObject baru
        GameObject go = new GameObject("DamagePopup_" + damageAmount);
        
        // Posisi sedikit acak di atas karakter
        go.transform.position = position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1f), 0);
        
        // Tambahkan script ini
        DamagePopupManager popup = go.AddComponent<DamagePopupManager>();
        popup.Setup(damageAmount, isPlayerDamage);
    }

    private void Setup(int damageAmount, bool isPlayerDamage)
    {
        // Tambahkan TextMesh bawaan Unity
        textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.text = damageAmount.ToString();
        textMesh.characterSize = 0.06f; // Diperkecil agar teks tidak terlalu raksasa
        textMesh.fontSize = 50; // Resolusi font tetap tinggi agar tidak buram
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        
        // Pastikan tampil di atas sprite 2D lainnya
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "UI"; 
        renderer.sortingOrder = 9999; // Sangat di depan

        if (isPlayerDamage)
        {
            textColor = Color.red; // Warna damage ke Saka
            textMesh.fontSize = 60; // Agak lebih besar sedikit dari damage musuh
            textMesh.fontStyle = FontStyle.Bold;
        }
        else
        {
            textColor = Color.white; // Warna damage ke Musuh
            textMesh.fontStyle = FontStyle.Bold;
        }

        textMesh.color = textColor;
        disappearTimer = 0.2f; // Sangat cepat (0.2 detik saja melayang penuh)
        moveVector = new Vector3(0, 1.5f, 0); // Kecepatan melayang diperlambat agar tidak terlalu tinggi
    }

    private void Update()
    {
        // Gerakkan ke atas
        transform.position += moveVector * Time.deltaTime;
        
        // Kurangi timer
        disappearTimer -= Time.deltaTime;
        
        if (disappearTimer < 0)
        {
            // Mulai memudar (fade out)
            float fadeAmount = 6f; // Fade out lebih cepat
            textColor.a -= fadeAmount * Time.deltaTime;
            textMesh.color = textColor;
            
            // Hancurkan objek jika sudah transparan 100%
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
