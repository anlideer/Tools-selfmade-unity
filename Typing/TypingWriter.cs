using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

// attached to the element with TextMeshProUGUI component
public class TypingWriter : MonoBehaviour
{
    [SerializeField] private float characterDuration = .1f;
    [SerializeField] private float textAliveDuration = 6f;

    private TextMeshProUGUI textMesh;
    private string textBuffer = "";
    private float lastPushTime;
    private readonly object bufferLock = new object();
    private readonly object displayedTextLock = new object();

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null) 
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
        lock (bufferLock)
        {
            while (textBuffer.Length > 0 && Time.timeSinceLevelLoad - lastPushTime >= characterDuration)
            {
                lastPushTime = Time.timeSinceLevelLoad;
                char c = textBuffer[0];

                // support html tag
                if (c == '<')
                {
                    string tag = "<";
                    int i = 1;
                    while (i < textBuffer.Length && textBuffer[i] != '>')
                    {
                        tag += textBuffer[i];
                        i++;
                    }
                    // add one more character
                    if (i < textBuffer.Length)
                    {
                        tag += textBuffer[i];
                    }
                    lock (displayedTextLock)
                    {
                        textMesh.text = textMesh.text + tag;
                        textBuffer = textBuffer.Substring(i+1);
                    }
                }
                else
                {
                    lock (displayedTextLock)
                    {
                        textMesh.text = textMesh.text + c;
                        textBuffer = textBuffer.Substring(1);
                    }
                }
            }
        }
    }

    public void PushText(string text)
    {
        lock (bufferLock)
        {
            textBuffer += text;
            StartCoroutine(EraseString(text, textAliveDuration));
        }
    }

    private IEnumerator EraseString(string str, float aliveDuration)
    {
        yield return new WaitForSeconds(aliveDuration);
        lock (displayedTextLock)
        {
            if (textMesh.text.StartsWith(str))
            {
                int len = str.Length;
                textMesh.text = textMesh.text.Substring(len);
            }
        }
    }
}
