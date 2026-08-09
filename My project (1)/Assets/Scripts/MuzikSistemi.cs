using System.Collections;
using UnityEngine;

public class MuzikSistemi : MonoBehaviour
{
    [Header("Müzik Ayarları")]
    public AudioSource teyp;
    public AudioClip normalMuzik;
    public AudioClip bossMuzigi;
    
    [Header("Geçiş Ayarları")]
    public float gecisSuresi = 2f; // Müziğin birbirine karışma süresi (saniye)

    void Start()
    {
        // Oyun başlar başlamaz normal müziği kasede tak ve tam sesle çalmaya başla
        if (normalMuzik != null)
        {
            teyp.clip = normalMuzik;
            teyp.volume = 1f;
            teyp.Play();
        }
    }

    public void BossMuzigineGec()
    {
        // Eğer zaten boss müziği çalmıyorsa, yavaş geçişi (Fade) başlat
        if (teyp.clip != bossMuzigi)
        {
            StartCoroutine(MuzikGecisi(bossMuzigi));
        }
    }

    // YENİ: Sesi yavaşça kısıp yeni müziği yavaşça açan sinematik sistem
    IEnumerator MuzikGecisi(AudioClip yeniMuzik)
    {
        // 1. AŞAMA: Şu an çalan müziğin sesini yavaşça sıfıra indir (Fade Out)
        float baslangicSesi = teyp.volume;
        
        while (teyp.volume > 0)
        {
            teyp.volume -= baslangicSesi * Time.deltaTime / (gecisSuresi / 2f);
            yield return null;
        }

        teyp.volume = 0f; // Tamamen sessiz

        // 2. AŞAMA: Kasedi çıkar, yeni müziği tak ve başlat
        teyp.clip = yeniMuzik;
        teyp.Play();

        // 3. AŞAMA: Yeni müziğin sesini yavaşça eski seviyesine yükselt (Fade In)
        while (teyp.volume < baslangicSesi)
        {
            teyp.volume += baslangicSesi * Time.deltaTime / (gecisSuresi / 2f);
            yield return null;
        }

        teyp.volume = baslangicSesi; // Ses normale döndü
    }
}