using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// attached to the element with TextMeshProUGUI component
public class TypingWriter : MonoBehaviour
{
    [SerializeField] private float characterDuration = .5f;

    private TextMeshProUGUI textMesh;
    private string textBuffer = "";
    private float lastPushTime;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null ) 
        {
            Debug.LogWarning("TypingWriter won't work without TextMeshProUGUI component");
        }
        else
        {
            textMesh.text = "";
        }
    }

    private void Update()
    {
        if (textBuffer.Length > 0 && Time.timeSinceLevelLoad - lastPushTime >= characterDuration)
        {
            lastPushTime = Time.timeSinceLevelLoad;
            char c = textBuffer[0];
            textMesh.text = textMesh.text + c;
            textBuffer = textBuffer.Substring(1);
        }
    }

    public void PushText(string text)
    {
        textBuffer += text;
    }
}
