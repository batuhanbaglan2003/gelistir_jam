using UnityEngine;

public class KameraTakip : MonoBehaviour
{
    public Transform player; // Takip edilecek karakter
    public Vector3 offset = new Vector3(0, 0, -10); // Kameranın Z eksenindeki derinlik ayarı

    void LateUpdate()
    {
        if (player != null)
        {
            // Hiç gecikmesiz doğrudan karaktere sabitler
            transform.position = player.position + offset;
        }
    }
}