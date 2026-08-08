using System.Collections;
using UnityEngine;

// Sınıf adı dosya adıyla BİREBİR aynı olmalı
public class enemy_1 : MonoBehaviour 
{
    [Header("Can Sistemi")]
    public int can = 3; 

    [Header("Hareket Ayarları")]
    public float hiz = 3f;
    public float algilamaMesafesi = 7f; // YENİ: Düşmanın seni fark edeceği maksimum uzaklık

    [Header("Saldırı Ayarları")]
    public float saldiriMenzili = 1.5f; 
    public float saldiriBeklemeSuresi = 1.5f; 
    public int attackDamage = 1; 
    
    public float savurmaAcisi = 45f;    
    public float savurmaHizi = 0.15f;    

    [Header("Hasar Sistemi (Hitbox)")]
    public Transform vurusNoktasi; 
    public float vurusYariCapi = 0.5f; 

    private Transform playerTransform;
    private bool isAttacking = false;
    private float sonSaldiriZamani = 0f;

    // YENİ: Kılıç savrulurken saniyede 10 kere hasar almayı engelleyen kalkan sistemi
    private float sonHasarAlmaZamani = 0f;
    private float hasarAlmaBeklemeSuresi = 0.4f; // Kılıç sallama süresinden büyük olmalı

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Mesafeyi ölçüyoruz
        float mesafe = Vector2.Distance(transform.position, playerTransform.position);

        // EĞER OYUNCU ALGILAMA MESAFESİNDEYSE HAREKETE GEÇ (Yeni eklenen kontrol)
        if (mesafe <= algilamaMesafesi) 
        {
            // Yüzünü oyuncuya dön
            if (!isAttacking)
            {
                transform.up = playerTransform.position - transform.position;
            }

            // Saldırı menziline girene kadar oyuncuya doğru yürü
            if (mesafe > saldiriMenzili && !isAttacking)
            {
                transform.Translate(Vector2.up * hiz * Time.deltaTime, Space.Self);
            }
            // Saldırı menziline girdiyse ve bekleme süresi dolduysa saldır!
            else if (mesafe <= saldiriMenzili && Time.time > sonSaldiriZamani + saldiriBeklemeSuresi && !isAttacking)
            {
                sonSaldiriZamani = Time.time;
                StartCoroutine(KazmaSavur());
            }
        }
    }

    IEnumerator KazmaSavur()
    {
        isAttacking = true;

        float startZ = transform.eulerAngles.z;
        float targetZ = startZ - savurmaAcisi; 
        float elapsed = 0f;

        while (elapsed < savurmaHizi)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(startZ, targetZ, elapsed / savurmaHizi);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, targetZ);

        VurulanlariTara();

        elapsed = 0f;

        while (elapsed < savurmaHizi)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(targetZ, startZ, elapsed / savurmaHizi);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, startZ);

        isAttacking = false;
    }

    void VurulanlariTara()
    {
        main_player oyuncu = FindObjectOfType<main_player>();
        
        if (oyuncu != null)
        {
            if (Vector2.Distance(transform.position, oyuncu.transform.position) <= vurusYariCapi)
            {
                oyuncu.HasarAl(attackDamage); 
                Debug.Log("Enemy 1 sana KESİN OLARAK VURDU!");
            }
        }
    }

    public void HasarAl(int alinacakHasar)
    {
        // YENİ EKLENDİ: Kalkan (Cooldown) devredeyse hasarı iptal et
        if (Time.time < sonHasarAlmaZamani + hasarAlmaBeklemeSuresi) return;

        sonHasarAlmaZamani = Time.time;
        can -= alinacakHasar;
        Debug.Log("Enemy 1 hasar aldı! Kalan Can: " + can);

        if (can <= 0)
        {
            Debug.Log("Enemy 1 ÖLDÜ!");
            Destroy(gameObject); 
        }
    }

    void OnDrawGizmosSelected()
    {
        if (vurusNoktasi == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(vurusNoktasi.position, vurusYariCapi);
    }
}