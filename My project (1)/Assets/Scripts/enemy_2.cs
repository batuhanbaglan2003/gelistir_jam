using UnityEngine;

public class enemy_2 : MonoBehaviour
{
    [Header("Can Sistemi")]
    public int can = 2;

    [Header("Hareket Ayarları")]
    public float hiz = 2f; 
    public float durmaMenzili = 6f; 
    public float algilamaMesafesi = 10f; // YENİ EKLENDİ

    [Header("Saldırı Ayarları")]
    public GameObject alevTopuPrefab; 
    public Transform atisNoktasi; 
    public float atisBeklemeSuresi = 2f; 

    private Transform playerTransform;
    private Collider2D playerCollider;
    private float sonAtisZamani = 0f;

    // YENİ: Kılıç savrulurken saniyede 10 kere hasar almayı engelleyen kalkan sistemi
    private float sonHasarAlmaZamani = 0f;
    private float hasarAlmaBeklemeSuresi = 0.4f; // Kılıç sallama süresinden büyük olmalı

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerCollider = playerObj.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (playerTransform == null || playerCollider == null) return;

        // Oyuncunun koordinatını değil, yeşil kutusunun (Collider) tam GÖBEĞİNİ hedef al
        Vector2 hedefNoktasi = playerCollider.bounds.center;
        
        // Mesafeyi en baştan ölçüyoruz
        float mesafe = Vector2.Distance(transform.position, hedefNoktasi);

        // EĞER OYUNCU ALGILAMA MESAFESİNDEYSE HAREKETE GEÇ
        if (mesafe <= algilamaMesafesi)
        {
            // Önce oyuncuya dön
            Vector2 yon = hedefNoktasi - (Vector2)transform.position;
            transform.up = yon;

            // Eğer durma menzilinden uzaktaysa yaklaş
            if (mesafe > durmaMenzili)
            {
                transform.Translate(Vector2.up * hiz * Time.deltaTime, Space.Self);
            }
            // Durma menziline girdiyse ve bekleme süresi dolduysa ateş et
            else if (mesafe <= durmaMenzili && Time.time >= sonAtisZamani + atisBeklemeSuresi)
            {
                sonAtisZamani = Time.time;
                AtesEt();
            }
        }
    }

    void AtesEt()
    {
        if (alevTopuPrefab == null || atisNoktasi == null) return;
        Instantiate(alevTopuPrefab, atisNoktasi.position, atisNoktasi.rotation);
    }

    public void HasarAl(int alinacakHasar)
    {
        // YENİ EKLENDİ: Kalkan (Cooldown) devredeyse hasarı iptal et
        if (Time.time < sonHasarAlmaZamani + hasarAlmaBeklemeSuresi) return;

        sonHasarAlmaZamani = Time.time;
        can -= alinacakHasar;
        Debug.Log("Büyücü kılıç darbesi aldı! Kalan can: " + can); 
        
        if (can <= 0)
        {
            Destroy(gameObject);
        }
    }
}