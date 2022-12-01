using UnityEngine;
using UnityEngine.UI;

public class TextObject : MonoBehaviour
{
    public string TextId;
    private Text text;
    public void InitTextObject()    // textleri idlerine gore ve mevcut dile gore guncelleyen fonksiyon.
    {
        text = GetComponent<Text>();
            if (text != null)
                if (TextId == "ISOCode")
                    text.text = I18n.GetLanguage();
                else
                    text.text = I18n.Fields[TextId];
    }
}