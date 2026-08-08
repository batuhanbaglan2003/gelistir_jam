using UnityEngine;

public class enemy_2 : MonoBehaviour
{
    [Header("Can Sistemi")]
    public int can = 2;

    [Header("Hareket Ayarları")]
    public float hiz = 2f; 
    public float durmaMenzili = 6f; 

    [Header("Saldırı Ayarları")]
    public GameObject alevTopuPrefab; 
    public Transform atisNoktasi; 
    public float atisBeklemeSuresi = 2f; 

    private Transform playerTransform;
    private Collider2D playerCollider;
    private float sonAtisZamani = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerCollider = playerObj.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (playerTransform == null || playerCollider == null) return;

        // Oyuncunun koordinatını değil, yeşil kutusunun (Collider) tam GÖBEĞİNİ hedef al
        Vector2 hedefNoktasi = playerCollider.bounds.center;
        
        Vector2 yon = hedefNoktasi - (Vector2)transform.position;
        transform.up = yon;

        float mesafe = Vector2.Distance(transform.position, hedefNoktasi);

        if (mesafe > durmaMenzili)
        {
            transform.Translate(Vector2.up * hiz * Time.deltaTime, Space.Self);
        }
        else if (mesafe <= durmaMenzili && Time.time >= sonAtisZamani + atisBeklemeSuresi)
        {
            sonAtisZamani = Time.time;
            AtesEt();
        }
    }

    void AtesEt()
    {
        if (alevTopuPrefab == null || atisNoktasi == null) return;
        Instantiate(alevTopuPrefab, atisNoktasi.position, atisNoktasi.rotation);
    }

    public void HasarAl(int alinacakHasar)
    {
        can -= alinacakHasar;
        if (can <= 0)
        {
            Destroy(gameObject);
        }
    }
}