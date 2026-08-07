using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox; 
    
    // Artık bu değer, savurma hareketinin "kaç saniye süreceğini" belirliyor. 
    // Sayı küçüldükçe daha hızlı savurur.
    [SerializeField] private float vurusSuresi = 0.2f; 
    [SerializeField] private float beklemeSuresi = 1.0f; 

    private bool isAttacking = false;

    private void Start()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        Quaternion originalRotation = transform.rotation;

        // 1. GERİLME (Rüzgar alma): Önce kılıcı vurmak için geriye doğru (sağa) yumuşakça kalksın
        Quaternion rightRotation = originalRotation * Quaternion.Euler(0, 0, -45f);
        yield return StartCoroutine(SmoothRotate(rightRotation, vurusSuresi / 2f)); // Gerilme daha kısa sürsün

        // 2. VURUŞ BAŞLIYOR: Hitbox'ı aç
        if (attackHitbox != null) attackHitbox.SetActive(true);

        // 3. SAVURMA (Saldırı): Sağdan sola doğru, kılıcı savurarak akışkan bir geçiş yap
        Quaternion leftRotation = originalRotation * Quaternion.Euler(0, 0, 45f);
        yield return StartCoroutine(SmoothRotate(leftRotation, vurusSuresi)); 

        // 4. VURUŞ BİTTİ: Hitbox'ı kapat
        if (attackHitbox != null) attackHitbox.SetActive(false);

        // 5. TOPARLANMA: Vurduktan sonra karakter yavaşça eski düz haline dönsün
        yield return StartCoroutine(SmoothRotate(originalRotation, vurusSuresi / 2f));

        // Yeni vuruş için bekleme süresi
        yield return new WaitForSeconds(beklemeSuresi);
        
        isAttacking = false; 
    }

    // İki açı arasında akışkan geçiş yapmamızı sağlayan yardımcı matematiksel döngümüz
    private IEnumerator SmoothRotate(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float timeElapsed = 0f;

        // Belirlenen süre dolana kadar her frame (kare) burası çalışır
        while (timeElapsed < duration)
        {
            // Zaman aktıkça açıyı adım adım hedefe yaklaştırır
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            
            // Bir sonraki frame'e (kareye) kadar bekle
            yield return null; 
        }

        // Süre bittiğinde, tam hedeflenen açıda olduğumuzdan emin olmak için sabitleriz
        transform.rotation = targetRotation;
    }
}