using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] CarState carState;
    [SerializeField] Image healthBar;
    [SerializeField] Transform healthBarAnchor;

    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    private float carHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        carHealth = 1.0f - carState.GetHealth01();

        healthBarAnchor.localScale = new Vector3(Mathf.Lerp(1.0f, 0.0f, carHealth), 1.0f, 1.0f);
        healthBar.color = Color.Lerp(startColor, endColor, carHealth);
    }
}
