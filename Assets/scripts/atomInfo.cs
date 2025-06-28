using UnityEngine;
using TMPro;

public class atomInfo : MonoBehaviour
{
    public string elementString;
    public string elementState;
    public float elementMass;
    private TextMeshPro tmp;

    void Start()
    {
        // Get the TextMeshPro component and set its text to the elementString
        tmp = GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = elementString;
        }
        else
        {
            Debug.LogWarning("TextMeshPro component not found on " + gameObject.name);
        }
    }
}
