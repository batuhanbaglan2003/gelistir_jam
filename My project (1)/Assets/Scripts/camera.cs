using UnityEngine;

public class camera : MonoBehaviour
{
   public Transform takipEdilecekKarakter; // main_player'ı buraya sürükleyeceğiz
    public float takipHizi = 5f; // Kameranın yumuşaklığı (istediğin gibi artır/azalt)
    public Vector3 ofset = new Vector3(0, 0, -10); // Kameranın Z eksenindeki yüksekliği

    void Start()
    {
        // Oyuna başlarken Player etiketli objeyi otomatik bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            takipEdilecekKarakter = playerObj.transform;
        }
    }

    void LateUpdate()
    {
        if (takipEdilecekKarakter == null) return;

        // Hedef konumu belirle (Karakterin yeri + Z eksenindeki uzaklık)
        Vector3 hedefKonum = takipEdilecekKarakter.position + ofset;

        // Kamerayı yumuşak bir şekilde hedefe doğru kaydır
        transform.position = Vector3.Lerp(transform.position, hedefKonum, takipHizi * Time.deltaTime);
    }
}
