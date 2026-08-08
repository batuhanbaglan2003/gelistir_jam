using UnityEngine;

public class enemy_3 : MonoBehaviour
{
    [Header("Can Sistemi")]
    public int can = 2; // Şifacılar genelde kırılgandır

    [Header("Hareket Ayarları")]
    public float hiz = 3f; 
    public float kacisMenzili = 5f; // Oyuncu bu kadar yaklaşırsa kaçmaya başlar

    [Header("Şifa Ayarları")]
    public float sifaMenzili = 4f; // İyileştirme çemberinin büyüklüğü
    public float sifaBeklemeSuresi = 3f; // Kaç saniyede bir can basacağı
    public int sifaMiktari = 1; // Her basışta kaç can vereceği

    private Transform playerTransform;
    private float sonSifaZamani = 0f;

    // Hasar Kalkanı (Kılıç açıkken saniyede 10 kere vurulmasını engeller)
    private float sonHasarAlmaZamani = 0f;
    private float hasarAlmaBeklemeSuresi = 0.4f;

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

        float mesafe = Vector2.Distance(transform.position, playerTransform.position);

        // 1. KAÇIŞ MEKANİĞİ
        if (mesafe < kacisMenzili)
        {
            // Oyuncunun tersi yönüne bak (Arkanı dön)
            Vector2 kacisYonu = transform.position - playerTransform.position;
            transform.up = kacisYonu;

            // Baktığın yöne doğru koş
            transform.Translate(Vector2.up * hiz * Time.deltaTime, Space.Self);
        }
        else
        {
            // Kaçmıyorsa yüzünü oyuncuya dönüp izlesin
            transform.up = playerTransform.position - transform.position;
        }

        // 2. ŞİFA DAĞITMA MEKANİĞİ
        if (Time.time >= sonSifaZamani + sifaBeklemeSuresi)
        {
            sonSifaZamani = Time.time;
            SifaDagit();
        }
    }

    void SifaDagit()
    {
        bool sifaVerildiMi = false;

        // 1. Etraftaki Koşucuları bul ve iyileştir
        enemy_1[] kosucular = FindObjectsOfType<enemy_1>();
        foreach (enemy_1 kosucu in kosucular)
        {
            if (Vector2.Distance(transform.position, kosucu.transform.position) <= sifaMenzili)
            {
                kosucu.can += sifaMiktari;
                sifaVerildiMi = true;
            }
        }

        // 2. Etraftaki Büyücüleri bul ve iyileştir
        enemy_2[] buyuculer = FindObjectsOfType<enemy_2>();
        foreach (enemy_2 buyucu in buyuculer)
        {
            if (Vector2.Distance(transform.position, buyucu.transform.position) <= sifaMenzili)
            {
                buyucu.can += sifaMiktari;
                sifaVerildiMi = true;
            }
        }

        if (sifaVerildiMi)
        {
            Debug.Log("Şifacı etrafındakilere CAN BASTI!");
            // İleride buraya yeşil bir patlama efekti (Particle) ekleyebiliriz.
        }
    }

    public void HasarAl(int alinacakHasar)
    {
        // Kalkan devredeyse hasarı iptal et
        if (Time.time < sonHasarAlmaZamani + hasarAlmaBeklemeSuresi) return;

        sonHasarAlmaZamani = Time.time;
        can -= alinacakHasar;
        Debug.Log("Şifacı kılıç darbesi aldı! Kalan can: " + can); 
        
        if (can <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Şifa menzilini Unity ekranında kırmızı çizgiyle görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sifaMenzili);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, kacisMenzili);
    }
}