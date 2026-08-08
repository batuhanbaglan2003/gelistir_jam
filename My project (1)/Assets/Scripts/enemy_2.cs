using UnityEngine;

// DİKKAT: Dosyanın adı enemy_2.cs ise sınıf adı enemy_2 olmalı.
// Eğer dosyanın adı enemy2.cs ise sınıf adını enemy2 yap.
public class enemy_2 : MonoBehaviour
{
    [Header("Can Sistemi")]
    public int can = 2; // Büyücünün canı (2 kılıç darbesinde ölür)

    [Header("Hareket Ayarları")]
    public float hiz = 2f; 
    public float durmaMenzili = 6f; 

    [Header("Saldırı Ayarları")]
    public GameObject alevTopuPrefab; 
    public Transform atisNoktasi; 
    public float atisBeklemeSuresi = 2f; 

    private Transform playerTransform;
    private float sonAtisZamani = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("Düşman oyuncuyu BULDU!");
        }
        else
        {
            Debug.LogError("Düşman oyuncuyu BULAMADI! Player etiketi eksik olabilir.");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Düşmanı oyuncuya döndür (Z ekseninde)
        Vector2 yon = playerTransform.position - transform.position;
        transform.up = yon;

        float mesafe = Vector2.Distance(transform.position, playerTransform.position);

        if (mesafe > durmaMenzili)
        {
            transform.Translate(Vector2.up * hiz * Time.deltaTime, Space.Self);
        }
        else if (mesafe <= durmaMenzili && Time.time >= sonAtisZamani + atisBeklemeSuresi)
        {
            sonAtisZamani = Time.time;
            AtesEt();
        }
    }

    void AtesEt()
    {
        if (alevTopuPrefab == null || atisNoktasi == null) return;

        // Mermiyi Yarat
        Instantiate(alevTopuPrefab, atisNoktasi.position, atisNoktasi.rotation);
    }

    // --- YENİ EKLENDİ: SEN KILIÇ SALLADIĞINDA ÇALIŞACAK ---
    public void HasarAl(int alinacakHasar)
    {
        can -= alinacakHasar;
        Debug.Log("Enemy 2 hasar aldı! Kalan Can: " + can);

        if (can <= 0)
        {
            Debug.Log("Enemy 2 ÖLDÜ!");
            Destroy(gameObject); // Büyücü silinir
        }
    }
}