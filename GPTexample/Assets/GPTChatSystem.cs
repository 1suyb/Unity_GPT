using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GPTChatSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GPTModule module;
    [SerializeField] Transform contentTransform;

    public void GPTInput()
    {
        module.SendChatMessageAsync(inputField.text, GPTOutput);
    }

    public void GPTOutput(string message)
    {
        Debug.LogFormat("getString : {0}",message);
        TextMeshProUGUI contextText = contentTransform.GetComponent<TextMeshProUGUI>();
        string text = contextText.text;
        contextText.text = text + "\n" + message;
    }
}
