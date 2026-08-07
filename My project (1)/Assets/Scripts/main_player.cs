using System.Collections;
using UnityEngine;

public class main_player : MonoBehaviour
{
    [Header("Hareket")]
    public float forward_speed = 5f;

    [Header("Kılıçlar (opsiyonel, görsel efekt için)")]
    public Transform rightSword;
    public Transform leftSword;

    [Header("Saldırı Ayarları")]
    public float swingAngle = 45f;
    public float swingDuration = 0.15f;
    public float spinSpeed = 720f;

    [Header("Combo Ayarları")]
    public float comboResetTime = 0.6f;

    private bool isAttacking = false;
    private int comboStep = 0;
    private float lastClickTime = -999f;

    void Update()
    {
        // ---- MOUSE TAKİBİ ----
        if (!isAttacking)
        {
            Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse_position.z = 0;
            if (Vector3.Distance(mouse_position, transform.position) > 1f)
            {
                transform.up = mouse_position - transform.position;
            }
        }

        // ---- HAREKET ----
        if (!isAttacking)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");
            Vector2 movement = new Vector2(horizontal, vertical);
            transform.Translate(movement.normalized * forward_speed * Time.deltaTime, Space.World);
        }

        // ---- COMBO RESET ----
        if (comboStep > 0 && Time.time - lastClickTime > comboResetTime)
        {
            comboStep = 0;
        }

        // ---- TIKLAMA ----
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            lastClickTime = Time.time;
            comboStep++;

            switch (comboStep)
            {
                case 1: // sağ
                    StartCoroutine(RunAttack(BodySwing(-swingAngle)));
                    break;
                case 2: // sol
                    StartCoroutine(RunAttack(BodySwing(swingAngle)));
                    break;
                case 3: // sağ
                    StartCoroutine(RunAttack(BodySwing(-swingAngle)));
                    break;
                case 4: // sol
                    StartCoroutine(RunAttack(BodySwing(swingAngle)));
                    break;
                default: // 5. tık -> spin, combo sıfırlanır
                    StartCoroutine(RunAttack(SpinAttack()));
                    comboStep = 0;
                    break;
            }
        }
    }

    IEnumerator RunAttack(IEnumerator routine)
    {
        isAttacking = true;
        yield return StartCoroutine(routine);
        isAttacking = false;
    }

    IEnumerator BodySwing(float angle)
    {
        float startZ = transform.eulerAngles.z;
        float targetZ = startZ + angle;

        float elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(startZ, targetZ, elapsed / swingDuration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, targetZ);

        elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.LerpAngle(targetZ, startZ, elapsed / swingDuration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, startZ);
    }

    IEnumerator SpinAttack()
    {
        float rotated = 0f;
        while (rotated < 360f)
        {
            float step = spinSpeed * Time.deltaTime;
            rotated += step;
            transform.Rotate(0, 0, -step);
            yield return null;
        }
    }
}