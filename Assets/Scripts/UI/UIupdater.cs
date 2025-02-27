using TMPro;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    [SerializeField]
    private GameObject car;
    private Rigidbody rb;

    void Start()
    {
        rb = car.GetComponent<Rigidbody>();
        if (car == null)
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (rb != null && textUI != null)
        {
            float speed = Vector3.Dot(car.transform.forward, rb.linearVelocity) * 3.6f;
            textUI.text = "Speed: " + speed.ToString("F1") + " km/h";
        }
    }
}
