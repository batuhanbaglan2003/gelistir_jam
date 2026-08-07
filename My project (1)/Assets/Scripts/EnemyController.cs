using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    [SerializeField] private GameObject attackHitbox; 
    [SerializeField] private float vurusSuresi = 0.5f; 
    [SerializeField] private float beklemeSuresi = 1.0f; 

    [Header("Yapay Zeka Ayarları")]
    [SerializeField] private float algilamaMesafesi = 5f; // Düşmanın seni göreceği sınır
    [SerializeField] private float saldiriMesafesi = 1.2f; // Kılıç vurmaya başlayacağı sınır
    [SerializeField] private float hareketHizi = 2f; // Düşmanın yürüme hızı

    private Transform playerTarget;
    private bool isAttacking = false;

    private void Start()
    {
        if (attackHitbox != null) attackHitbox.SetActive(false);

        // 1. RADAR: Oyun başladığında sahnede "Player" etiketine sahip objeyi bulur
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    private void Update()
    {
        // Hedef yoksa veya zaten kılıç savuruyorsa yeni hareket yapma
        if (playerTarget == null || isAttacking) return;

        // 2. MESAFE ÖLÇÜMÜ: Düşman ile oyuncu arasındaki mesafeyi hesaplar
        float mesafe = Vector2.Distance(transform.position, playerTarget.position);

        // 3. DURUM MAKİNESİ (Eğer oyuncu algılama menziline girdiyse)
        if (mesafe <= algilamaMesafesi)
        {
            // YÜZÜNÜ OYUNCUYA DÖN (Vektör Matematiği)
            Vector2 yon = playerTarget.position - transform.position;
            
            // Eğer düşman yan veya ters dönüyorsa sondaki -90f kısmını +90f, 180f veya 0f yaparak düzeltebilirsin.
            float aci = Mathf.Atan2(yon.y, yon.x) * Mathf.Rad2Deg - 90f; 
            transform.rotation = Quaternion.Euler(0, 0, aci);

            // SALDIRI YA DA TAKİP KARARI
            if (mesafe <= saldiriMesafesi)
            {
                // Menzile girdiyse dur ve kılıç savur
                StartCoroutine(AttackSequence());
            }
            else
            {
                // Henüz vuracak kadar yakın değilse üstüne yürümeye devam et
                transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, hareketHizi * Time.deltaTime);
            }
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        Quaternion originalRotation = transform.rotation;

        Quaternion rightRotation = originalRotation * Quaternion.Euler(0, 0, -45f);
        yield return StartCoroutine(SmoothRotate(rightRotation, vurusSuresi / 2f)); 

        if (attackHitbox != null) attackHitbox.SetActive(true);

        Quaternion leftRotation = originalRotation * Quaternion.Euler(0, 0, 45f);
        yield return StartCoroutine(SmoothRotate(leftRotation, vurusSuresi)); 

        if (attackHitbox != null) attackHitbox.SetActive(false);

        yield return StartCoroutine(SmoothRotate(originalRotation, vurusSuresi / 2f));

        yield return new WaitForSeconds(beklemeSuresi);
        
        isAttacking = false; 
    }

    private IEnumerator SmoothRotate(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }
        transform.rotation = targetRotation;
    }
}