using UnityEngine;

// Sınıf adın dosya adıyla BİREBİR aynı olmalı (seninki enemy_2_fireball)
public class enemy_2_fireball : MonoBehaviour 
{
    public float hiz = 7f;
    public float yasamSuresi = 3f;
    public int attackDamage = 1; // Alev topu kaç can silecek (1 can = yarım kalp)

    void Start()
    {
        Destroy(gameObject, yasamSuresi); 
    }

    void Update()
    {
        transform.Translate(Vector3.up * hiz * Time.deltaTime, Space.Self);
    }

    // Mermi bir şeye dokunduğunda bu fonksiyon otomatik çalışır
    void OnTriggerEnter2D(Collider2D temas)
    {
        // Eğer temas ettiğimiz objenin etiketi "Player" ise
        if (temas.CompareTag("Player"))
        {
            // Vurduğumuz objeden 'main_player' kodunu çek
            main_player oyuncuKodu = temas.GetComponent<main_player>();
            
            if (oyuncuKodu != null)
            {
                oyuncuKodu.HasarAl(attackDamage); // Oyuncuya hasar ver!
                Debug.Log("Mermi oyuncuya ÇARPTI ve 1 can (yarım kalp) sildi!");
            }
            
            // Mermiyi (kendi kendini) sil
            Destroy(gameObject); 
        }
        else if (temas.CompareTag("Duvar")) 
        {
            Destroy(gameObject);
        }
    }
}