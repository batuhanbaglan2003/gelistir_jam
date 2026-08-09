using UnityEngine;

public class BossMuzikTetigi : MonoBehaviour
{
    public MuzikSistemi oyununMuzikSistemi;

    void OnTriggerEnter2D(Collider2D diger)
    {
        // Tetikleyiciye değen kişi "Player" etiketine sahipse
        if (diger.CompareTag("Player"))
        {
            if (oyununMuzikSistemi != null)
            {
                Debug.Log("Boss odasına girildi! Müzik değişiyor...");
                oyununMuzikSistemi.BossMuzigineGec();
            }
        }
    }
}