using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // BUNU EKLEDİK

public class main_player : MonoBehaviour
{
    [Header("Ses Efektleri")]
    public AudioSource kilicSesKaynagi;

    [Header("Can Sistemi ve UI")]
    public int maksimumCan = 10;
    public int guncelCan;
    public Image kalpGorseli; 
    public Sprite[] kalpResimleri; 

    [Header("Hareket")]
    public float forward_speed = 5f;

    [Header("Saldırı Ayarları")]
    public float swingAngle = 45f;
    public float swingDuration = 0.15f;
    public float spinSpeed = 720f;

    [Header("Combo Ayarları")]
    public float comboResetTime = 0.6f;

    [Header("Savaş Sistemi")]
    public Transform attackPoint; 
    public float attackRange = 1.2f; 
    public int attackDamage = 1; 
    public LayerMask enemyLayers; 

    private bool isAttacking = false;
    private int comboStep = 0;
    private float lastClickTime = -999f;
    private Rigidbody2D rb; 

    // YENİ: Kılıcın savrulurken açık kalıp kalmadığını kontrol eden sistem
    private bool kilicAktifMi = false; 

    public SpriteRenderer characters_sprite;
    public Sprite normal_picture;
    public Sprite turning_picture;

    void Start()
    {
        guncelCan = maksimumCan;
        KalpResminiGuncelle();
        
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        // ---- MOUSE TAKİBİ ----
        if (!isAttacking)
        {
            Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_position.z = 0;
            if (Vector3.Distance(mouse_position, transform.position) > 1f)
            {
                transform.up = mouse_position - transform.position;
            }
        }

        // ---- HAREKET (Fizik motoru velocity ile) ----
        if (!isAttacking)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(horizontal, vertical);
            
            if(rb != null)
                rb.linearVelocity = movement.normalized * forward_speed;
            else
                transform.Translate(movement.normalized * forward_speed * Time.deltaTime, Space.Self);
        }
        else
        {
            if(rb != null) rb.linearVelocity = Vector2.zero; 
        }

        // ---- COMBO RESET ----
        if (comboStep > 0 && Time.time - lastClickTime > comboResetTime)
        {
            comboStep = 0;
        }

       // ---- TIKLAMA ----
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // YENİ: Tıkladığımız an kılıç sesini çal!
            if (kilicSesKaynagi != null)
            {
                kilicSesKaynagi.Play(); 
            }

            lastClickTime = Time.time;
            comboStep++;
            // ... (kodun geri kalanı aynı kalacak)
            switch (comboStep)
            {
                case 1: 
                    StartCoroutine(RunAttack(BodySwing(-swingAngle)));
                    break;
                case 2: 
                    StartCoroutine(RunAttack(BodySwing(swingAngle)));
                    break;
                case 3: 
                    StartCoroutine(RunAttack(BodySwing(-swingAngle)));
                    break;
                case 4: 
                    StartCoroutine(RunAttack(BodySwing(swingAngle)));
                    break;
                default: 
                    StartCoroutine(RunAttack(SpinAttack()));
                    comboStep = 0;
                    break;
            }
        }

        // ---- YENİ: SÜREKLİ HASAR KONTROLÜ ----
        if (kilicAktifMi && attackPoint != null)
        {
            // Koşucuları kontrol et
            enemy_1[] butunKosucular = FindObjectsOfType<enemy_1>();
            foreach (enemy_1 kosucu in butunKosucular)
            {
                if (Vector2.Distance(attackPoint.position, kosucu.transform.position) <= attackRange)
                {
                    kosucu.HasarAl(attackDamage);
                }
            }

            // Büyücüleri kontrol et
            enemy_2[] butunBuyuculer = FindObjectsOfType<enemy_2>();
            foreach (enemy_2 buyucu in butunBuyuculer)
            {
                if (Vector2.Distance(attackPoint.position, buyucu.transform.position) <= attackRange)
                {
                    buyucu.HasarAl(attackDamage);
                }
            }

            // ŞİFACILARI KONTROL ET (BUNU EKLEDİK!)
            enemy_3[] butunSifacilar = FindObjectsOfType<enemy_3>();
            foreach (enemy_3 sifaci in butunSifacilar)
            {
                if (Vector2.Distance(attackPoint.position, sifaci.transform.position) <= attackRange)
                {
                    sifaci.HasarAl(attackDamage);
                }
            }
        }
    }

    IEnumerator RunAttack(IEnumerator routine)
    {
        isAttacking = true;
        yield return StartCoroutine(routine);
        isAttacking = false;
    }

    IEnumerator BodySwing(float angle)
    {
        float startZ = transform.eulerAngles.z;
        float targetZ = startZ + angle;

        float elapsed = 0f;
        
        kilicAktifMi = true; // YENİ: KILIÇ AÇILDI! Savurma bitene kadar değdiği herkesi kesecek

        // 1. Kılıcı ileri savur
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(startZ, targetZ, elapsed / swingDuration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, targetZ);

        // DealDamage()'i sildik çünkü artık Update içinde sürekli kontrol ediliyor

        elapsed = 0f;
        
        // 2. Kılıcı geri çek
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(targetZ, startZ, elapsed / swingDuration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, startZ);

        kilicAktifMi = false; // YENİ: KILIÇ KAPANDI!
    }

    IEnumerator SpinAttack()
    {
        // 1. DÖNÜŞ BAŞLIYOR: Karakterin resmini değiştir!
        if (characters_sprite != null && turning_picture != null)
        {
            characters_sprite.sprite = turning_picture;
        }

        kilicAktifMi = true; // YENİ: DÖNÜŞ BAŞLADI, KILIÇ AÇIK!

        float rotated = 0f;
        while (rotated < 720f)
        {
            float step = spinSpeed * Time.deltaTime;
            rotated += step;
            transform.Rotate(0, 0, -step);
            yield return null;
        }

        kilicAktifMi = false; // YENİ: DÖNÜŞ BİTTİ, KILIÇ KAPANDI!

        // 2. DÖNÜŞ BİTTİ: Karakteri normal resmine döndür!
        if (characters_sprite != null && normal_picture != null)
        {
            characters_sprite.sprite = normal_picture;
        }
    }

    void DealDamage()
    {
        if (attackPoint == null) return; 

        // 1. Sahnede ne kadar enemy_1 (Koşucu) varsa bul
        enemy_1[] butunKosucular = FindObjectsOfType<enemy_1>();
        foreach (enemy_1 kosucu in butunKosucular)
        {
            if (Vector2.Distance(attackPoint.position, kosucu.transform.position) <= attackRange)
            {
                kosucu.HasarAl(attackDamage);
            }
        }

        // 2. Sahnede ne kadar enemy_2 (Büyücü) varsa bul
        enemy_2[] butunBuyuculer = FindObjectsOfType<enemy_2>();
        foreach (enemy_2 buyucu in butunBuyuculer)
        {
            if (Vector2.Distance(attackPoint.position, buyucu.transform.position) <= attackRange)
            {
                buyucu.HasarAl(attackDamage);
            }
        }
    }

    public void HasarAl(int alinacakHasar)
    {
        guncelCan -= alinacakHasar;
        if (guncelCan < 0) guncelCan = 0; 
        
        KalpResminiGuncelle();
        Debug.Log("Oyuncu hasar aldı! Kalan Can: " + guncelCan);

        // CAN SIFIRA DÜŞTÜĞÜNDE ÇALIŞACAK KISIM
        if (guncelCan <= 0)
        {
            Debug.Log("OYUN BİTTİ! Karakter Öldü ve Sahne Sıfırlanıyor...");

            // 1. ADIM: Ölüm sayacını 1 artırıyoruz
            OlumSayaci.toplamOlum++; 

            // 2. ADIM: Destroy yapmak yerine, sahneyi en baştaki haline yeniliyoruz
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        }
    }

    void KalpResminiGuncelle()
    {
        if (kalpGorseli != null && kalpResimleri.Length > guncelCan)
        {
            kalpGorseli.sprite = kalpResimleri[guncelCan];
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}