using System.Collections;
using UnityEngine;

public class main_player : MonoBehaviour
{
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
    public int attackDamage = 25; // Vereceğimiz hasar
    public LayerMask enemyLayers; // Vurulacak katman (Enemy)

    private bool isAttacking = false;
    private int comboStep = 0;
    private float lastClickTime = -999f;

    public SpriteRenderer characters_sprite;
    public Sprite normal_picture;
    public Sprite turning_picture;


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

        // ---- HAREKET ----
        if (!isAttacking)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(horizontal, vertical);
            transform.Translate(movement.normalized * forward_speed * Time.deltaTime, Space.Self);
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
        // 1. DÖNÜŞ BAŞLIYOR: Karakterin resmini "Dönme Resmi" ile değiştir!
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

        // 2. DÖNÜŞ BİTTİ: Karakteri tekrar "Normal Resmine" döndür!
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
            // İkinci adımda yazdığımız (veya yazacağımız) Enemy kodunu hedefin içinde arıyoruz
            /*Enemy target = enemyObj.GetComponent<Enemy>();
            
            // Eğer objede Enemy kodu varsa, canını düşürüyoruz
            if (target != null)
            {
                target.TakeDamage(attackDamage);
            }
            else 
            {
                // Eğer Enemy kodu henüz yoksa bile vurduğumuzu log ile görelim
                Debug.Log("Kılıç şuna çarptı (Ama Enemy kodu yok): " + enemyObj.name); 
            }
             */
        }
    }

    // Unity ekranında vuruş menzilini görmeni sağlar (Oyunda görünmez)
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}