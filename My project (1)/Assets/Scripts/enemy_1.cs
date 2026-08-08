using System.Collections;
using UnityEngine;

// Sınıf adı dosya adıyla BİREBİR aynı olmalı (seninki enemy_1)
public class enemy_1 : MonoBehaviour 
{
    [Header("Can Sistemi")]
    public int can = 3; // Koşucu düşmanın canı (3 vuruşta ölür)

    [Header("Hareket Ayarları")]
    public float hiz = 3f;

    [Header("Saldırı Ayarları")]
    public float saldiriMenzili = 1.5f; 
    public float saldiriBeklemeSuresi = 1.5f; 
    public int attackDamage = 1; // Kazma vurduğunda senden gidecek can (1 can = yarım kalp)
    
    public float savurmaAcisi = 45f;    
    public float savurmaHizi = 0.15f;    

    [Header("Hasar Sistemi (Hitbox)")]
    public Transform vurusNoktasi; // Kazmanın ucu
    public float vurusYariCapi = 0.5f; 
    public LayerMask playerLayer; // Unity'den 'Player' katmanını seçeceğiz

    private Transform playerTransform;
    private bool isAttacking = false;
    private float sonSaldiriZamani = 0f;

    void Start()
    {
        // Player etiketine sahip objeyi (seni) bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float mesafe = Vector2.Distance(transform.position, playerTransform.position);

        // Saldırmıyorsa oyuncuya doğru dön
        if (!isAttacking)
        {
            transform.up = playerTransform.position - transform.position;
        }

        // Menzilde değilse yürü
        if (mesafe > saldiriMenzili && !isAttacking)
        {
            transform.Translate(Vector2.up * hiz * Time.deltaTime, Space.Self);
        }
        // Menzildeyse ve bekleme süresi dolmuşsa saldır
        else if (mesafe <= saldiriMenzili && Time.time > sonSaldiriZamani + saldiriBeklemeSuresi && !isAttacking)
        {
            sonSaldiriZamani = Time.time;
            StartCoroutine(KazmaSavur());
        }
    }

    IEnumerator KazmaSavur()
    {
        isAttacking = true;

        float startZ = transform.eulerAngles.z;
        float targetZ = startZ - savurmaAcisi; 
        float elapsed = 0f;

        // 1. İleri Savur
        while (elapsed < savurmaHizi)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(startZ, targetZ, elapsed / savurmaHizi);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, targetZ);

        // --- KAZMA HEDEFE ULAŞTIĞINDA HASAR VUR ---
        VurulanlariTara();

        elapsed = 0f;

        // 2. Geri Çek
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
        if (vurusNoktasi == null) return;

        // Kazmanın ucundaki yuvarlak alanda 'Player' arıyoruz
        Collider2D[] vurulanlar = Physics2D.OverlapCircleAll(vurusNoktasi.position, vurusYariCapi, playerLayer);
        
        foreach (Collider2D vurulan in vurulanlar)
        {
            // Vurulan objede main_player kodu var mı?
            main_player oyuncuKodu = vurulan.GetComponent<main_player>();
            
            if (oyuncuKodu != null)
            {
                oyuncuKodu.HasarAl(attackDamage); // Senin canını azaltır!
                Debug.Log("Enemy 1 sana KAZMA İLE VURDU!");
            }
        }
    }

    // --- YENİ EKLENDİ: SEN KILIÇ SALLADIĞINDA ÇALIŞACAK ---
    public void HasarAl(int alinacakHasar)
    {
        can -= alinacakHasar;
        Debug.Log("Enemy 1 hasar aldı! Kalan Can: " + can);

        if (can <= 0)
        {
            Debug.Log("Enemy 1 ÖLDÜ!");
            Destroy(gameObject); // Düşman silinir
        }
    }

    void OnDrawGizmosSelected()
    {
        if (vurusNoktasi == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(vurusNoktasi.position, vurusYariCapi);
    }
}