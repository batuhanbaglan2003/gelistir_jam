using UnityEngine;

public class enemy_2_fireball : MonoBehaviour 
{
    public float hiz = 7f;
    public float yasamSuresi = 3f;
    public int attackDamage = 1; 
    public float carpmaMesafesi = 1.0f; // Sıyırmaması için alanı biraz büyüttük

    private main_player oyuncu;
    private Collider2D oyuncuCollider;
    private Vector3 ucusYonu; // Merminin gideceği kesin çizgi

    void Start()
    {
        oyuncu = FindObjectOfType<main_player>(); 
        if (oyuncu != null)
        {
            oyuncuCollider = oyuncu.GetComponent<Collider2D>();
            
            // Doğduğu an oyuncunun göbeğinin yerini tespit et ve o yönü kilitle!
            ucusYonu = (oyuncuCollider.bounds.center - transform.position).normalized;
        }
        Destroy(gameObject, yasamSuresi); 
    }

    void Update()
    {
        // Unity'nin kendi yönlerini (transform.up vb.) tamamen siktir et, kilitlenen yöne uç
        transform.position += ucusYonu * hiz * Time.deltaTime;

        if (oyuncu != null && oyuncuCollider != null)
        {
            float mesafe = Vector2.Distance(transform.position, oyuncuCollider.bounds.center);
            
            if (mesafe <= carpmaMesafesi)
            {
                oyuncu.HasarAl(attackDamage);
                Debug.Log("Alev topu tam göbekten vurdu!");
                Destroy(gameObject); 
            }
        }
    }
}