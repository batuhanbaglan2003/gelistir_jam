using System.Collections;
using UnityEngine;

public class boss_1 : MonoBehaviour
{
    [Header("Hayatta Kalma (Can) Sistemi")]
    public float can = 1100f; // Başlangıç canı 1100 yapıldı

    [Header("Boss Ses Efektleri")]
    public AudioClip atilmaSesi;
   
    [Header("Hareket ve Algılama")]
    public float normalHiz = 2.5f;
    public float algilamaMesafesi = 12f; // İstediğin gibi 12 yapıldı

    [Header("Özel Yetenek: Gecikmeli Atılma")]
    public float atilmaHizi = 8f;
    public float kilitlenmeSuresi = 2f; 
    public float ozelYetenekCooldown = 10f;
    private float sonOzelYetenekZamani = -10f; 
    private bool atiliyorMu = false;

    [Header("Normal Saldırı: Yumruk")]
    public float yumrukMenzili = 1.5f;
    public float yumrukBeklemeSuresi = 3f;
    public int yumrukHasari = 2;
    public float vurusYariCapi = 0.8f;
    private float sonYumrukZamani = 0f;
    private bool yumrukAtiyor = false;

    private Transform playerTransform;

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
        // Not: Saniyede kendi kendine azalan can kodu buradan tamamen kaldırıldı. 
        // Artık sadece hasar aldığında veya yetenek kullandığında canı azalacak.

        if (playerTransform == null || atiliyorMu || yumrukAtiyor) return;

        float mesafe = Vector2.Distance(transform.position, playerTransform.position);

        if (mesafe <= algilamaMesafesi)
        {
            transform.up = playerTransform.position - transform.position;

            if (Time.time >= sonOzelYetenekZamani + ozelYetenekCooldown)
            {
                StartCoroutine(HedefeKilitlenVeAtil());
            }
            else if (mesafe <= yumrukMenzili && Time.time >= sonYumrukZamani + yumrukBeklemeSuresi)
            {
                StartCoroutine(YumrukSaldirisi());
            }
            else if (mesafe > yumrukMenzili)
            {
                transform.Translate(Vector2.up * normalHiz * Time.deltaTime, Space.Self);
            }
        }
    }

 public LineRenderer lineRenderer;

   IEnumerator HedefeKilitlenVeAtil()
    {
        atiliyorMu = true;
        sonOzelYetenekZamani = Time.time;
        
        // LineRenderer açık değilse aç ve çizgiye başla
        if (lineRenderer != null) lineRenderer.enabled = true;

        Vector2 kilitlenenHedef = playerTransform.position;
        Debug.Log("Boss oyuncunun konumunu işaretledi! 2 saniye sonra oraya uçacak!");

        // 2 saniye boyunca kilitlenme süresince kırmızı çizgiyi oyuncuya doğru güncelle
        float timer = 0f;
        while (timer < kilitlenmeSuresi)
        {
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, kilitlenenHedef);
            }
            timer += Time.deltaTime;
            yield return null;
        }

       // Kilitlenme süresi bitti, çizgiyi kapat
        if (lineRenderer != null) lineRenderer.enabled = false;

        // Her atıldığında canından 100 eksiltelim
        can -= 100f;
        Debug.Log("Boss atıldı! Harcanan can: 100. Kalan Can: " + can);

        // YENİ: Atılma sesini tam bu anda patlatıyoruz!
        if (atilmaSesi != null)
        {
            GameObject geciciSes = new GameObject("BossAtilmaSesi");
            AudioSource geciciKaynak = geciciSes.AddComponent<AudioSource>();
            geciciKaynak.clip = atilmaSesi;
            geciciKaynak.spatialBlend = 0f;
            geciciKaynak.volume = 1f;
            geciciKaynak.Play();
            Destroy(geciciSes, geciciKaynak.clip.length);
        }

        if (can <= 0)
        {
            Debug.Log("Boss gücünü tüketti ve öldü!");
            Destroy(gameObject);
            yield break;
        }

        bool atilmaHasariVerildi = false;
        main_player oyuncu = FindObjectOfType<main_player>();

        while (Vector2.Distance(transform.position, kilitlenenHedef) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, kilitlenenHedef, atilmaHizi * Time.deltaTime);

            if (!atilmaHasariVerildi && oyuncu != null)
            {
                if (Vector2.Distance(transform.position, oyuncu.transform.position) <= vurusYariCapi)
                {
                    oyuncu.HasarAl(1); 
                    Debug.Log("Boss atılırken oyuncuyu ezdi!");
                    atilmaHasariVerildi = true; 
                }
            }

            yield return null; 
        }

        yield return new WaitForSeconds(0.5f);
        atiliyorMu = false;
    }

    IEnumerator YumrukSaldirisi()
    {
        yumrukAtiyor = true;
        sonYumrukZamani = Time.time;

        yield return new WaitForSeconds(0.2f); 

        main_player oyuncu = FindObjectOfType<main_player>();
        if (oyuncu != null && Vector2.Distance(transform.position, oyuncu.transform.position) <= vurusYariCapi)
        {
            oyuncu.HasarAl(yumrukHasari);
            Debug.Log("Boss acımasız bir yumruk attı!");
        }

        yield return new WaitForSeconds(0.3f); 
        yumrukAtiyor = false;
    }
    // Oyuncu boss'un trigger alanına girdiği an çalışır
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Player"))
        {
            Debug.Log("Boss oyuncuyu ezdi ve öldürdü!");
            
            // Oyuncuyu direkt silmek yerine senin HasarAl sistemine 999 devasa hasar gönderiyoruz!
            // Böylece ana karakterindeki kod çalışacak, hem ölüm sayacı artacak hem de karakter silinecek.
            diger.GetComponent<main_player>().HasarAl(999); 
        }
    }
}