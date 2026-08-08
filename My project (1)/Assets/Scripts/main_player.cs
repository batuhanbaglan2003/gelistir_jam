using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI resimlerini değiştirmek için eklendi

public class main_player : MonoBehaviour
{
    [Header("Can Sistemi ve UI")]
    public int maksimumCan = 10;
    public int guncelCan;
    public Image kalpGorseli; // Canvas'taki Image buraya sürüklenecek
    public Sprite[] kalpResimleri; // 11 adet (0'dan 10'a kadar) kalp resimleri buraya eklenecek

    [Header("Hareket")]
    public float forward_speed = 5f;

    [Header("Saldırı Ayarları")]
    public float swingAngle = 45f;
    public float swingDuration = 0.15f;
    public float spinSpeed = 720f;

    [Header("Combo Ayarları")]
    public float comboResetTime = 0.6f;

    [Header("Savaş Sistemi")]
    public Transform attackPoint; // Karakterin tam önüne koyduğumuz boş obje
    public float attackRange = 1.2f; // Kılıcın menzili
    public int attackDamage = 1; // Vereceğimiz hasar (1 can = yarım kalp mantığına göre ayarladık)
    public LayerMask enemyLayers; // Vurulacak katman (Enemy)

    private bool isAttacking = false;
    private int comboStep = 0;
    private float lastClickTime = -999f;
    private Rigidbody2D rb; // Fizik motoru için

    public SpriteRenderer characters_sprite;
    public Sprite normal_picture;
    public Sprite turning_picture;

    void Start()
    {
        // Oyuna başlarken canı fulle ve kalpleri güncelle
        guncelCan = maksimumCan;
        KalpResminiGuncelle();
        
        rb = GetComponent<Rigidbody2D>(); // Karakterdeki Rigidbody'i bul
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
            
            // Eğer fizik motoru varsa rb ile, yoksa Translate ile hareket et
            if(rb != null)
                rb.linearVelocity = movement.normalized * forward_speed;
            else
                transform.Translate(movement.normalized * forward_speed * Time.deltaTime, Space.Self);
        }
        else
        {
            if(rb != null) rb.linearVelocity = Vector2.zero; // Saldırırken karakteri durdur
        }

        // ---- COMBO RESET ----
        if (comboStep > 0 && Time.time - lastClickTime > comboResetTime)
        {
            comboStep = 0;
        }

        // ---- TIKLAMA ----
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            lastClickTime = Time.time;
            comboStep++;

            switch (comboStep)
            {
                case 1: // sağ
                    StartCoroutine(RunAttack(BodySwing(-swingAngle)));
                    break;
                case 2: // sol
                    StartCoroutine(RunAttack(BodySwing(swingAngle)));
                    break;
                case 3: // sağ
                    StartCoroutine(RunAttack(BodySwing(-swingAngle)));
                    break;
                case 4: // sol
                    StartCoroutine(RunAttack(BodySwing(swingAngle)));
                    break;
                default: // 5. tık -> spin, combo sıfırlanır
                    StartCoroutine(RunAttack(SpinAttack()));
                    comboStep = 0;
                    break;
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
        
        // 1. Kılıcı ileri savur
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(startZ, targetZ, elapsed / swingDuration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, targetZ);

        // KILIÇ HEDEFE ULAŞTIĞI AN HASAR VUR!
        DealDamage();

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
    }

    IEnumerator SpinAttack()
    {
        // 1. DÖNÜŞ BAŞLIYOR: Karakterin resmini değiştir!
        if (characters_sprite != null && turning_picture != null)
        {
            characters_sprite.sprite = turning_picture;
        }

        // Alan hasarı
        DealDamage();

        float rotated = 0f;
        while (rotated < 720f)
        {
            float step = spinSpeed * Time.deltaTime;
            rotated += step;
            transform.Rotate(0, 0, -step);
            yield return null;
        }

        // 2. DÖNÜŞ BİTTİ: Karakteri normal resmine döndür!
        if (characters_sprite != null && normal_picture != null)
        {
            characters_sprite.sprite = normal_picture;
        }
    }

    void DealDamage()
    {
        if (attackPoint == null) return;

        // attackPoint merkezli bir daire çiz ve içindeki enemyLayers katmanındaki herkesi bul
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemyObj in hitEnemies)
        {
            // İsimleri senin dosya adlarına göre düzelttim (enemy_1 ve enemy_2)
            
            // Önce enemy_1 (Kosucu) kodunu arıyoruz
            enemy_1 kosucuDusman = enemyObj.GetComponent<enemy_1>();
            if (kosucuDusman != null)
            {
                kosucuDusman.HasarAl(attackDamage);
            }
            
            // Eğer enemy_1 değilse, belki enemy_2 (Buyucu) dur
            enemy_2 buyucuDusman = enemyObj.GetComponent<enemy_2>();
            if(buyucuDusman != null)
            {
                buyucuDusman.HasarAl(attackDamage);
            }
        }
    }

    // --- YENİ: DÜŞMANLARDAN VE MERMİLERDEN HASAR ALMA SİSTEMİ ---
    public void HasarAl(int alinacakHasar)
    {
        guncelCan -= alinacakHasar;
        
        // Can sıfırın altına inmesin
        if (guncelCan < 0) guncelCan = 0; 
        
        KalpResminiGuncelle();
        Debug.Log("Oyuncu hasar aldı! Kalan Can: " + guncelCan);

        if (guncelCan <= 0)
        {
            Debug.Log("OYUN BİTTİ! Karakter Öldü.");
            // İleride buraya oyun bitiş ekranını veya yeniden başlatma kodunu yazarız
        }
    }

    // --- YENİ: KALP ARAYÜZÜNÜ (UI) GÜNCELLEME ---
    void KalpResminiGuncelle()
    {
        // Eğer UI resmini sürüklemeyi unuttuysak veya dizi boşsa hata vermesin diye kontrol ediyoruz
        if (kalpGorseli != null && kalpResimleri.Length > guncelCan)
        {
            kalpGorseli.sprite = kalpResimleri[guncelCan];
        }
    }

    // Unity ekranında vuruş menzilini görmeni sağlar
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}