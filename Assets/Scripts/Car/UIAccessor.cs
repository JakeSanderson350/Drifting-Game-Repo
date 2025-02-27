using TMPro;
using UnityEngine;

public class GenericUIAccessor : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    void Awake()
    {
        // Auto-assign the TextMeshProUGUI component if not manually assigned
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    /// Sets the text to the string
    public void SetText(string value)
    {
        if (textComponent)
            textComponent.text = value;
    }

    /// Sets the text to an integer value.
    public void SetText(int value)
    {
        if (textComponent)
            textComponent.text = value.ToString();
    }

    /// Sets the text to a float
    public void SetText(float value, string format = "F2")
    {
        if (textComponent)
            textComponent.text = value.ToString(format);
    }
}
