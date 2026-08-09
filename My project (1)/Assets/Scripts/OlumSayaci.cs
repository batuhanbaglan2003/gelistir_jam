using UnityEngine;
using TMPro; // Yazı bileşeni için gerekli kütüphane

public class OlumSayaci : MonoBehaviour
{
    // static değişken kullandığımız için sahne tekrar yüklense bile bu sayı sıfırlanmaz
    public static int toplamOlum = 0; 
    
    private TMP_Text yazi;

    void Start()
    {
        // Objenin üzerindeki yazı bileşenini otomatik alıyoruz
        yazi = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // Yazıyı her karede güncelliyoruz
        yazi.text = "Ölüm: " + toplamOlum;
    }
}