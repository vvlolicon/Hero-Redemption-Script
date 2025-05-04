using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveOption : MonoBehaviour
{
    [HideInInspector] public string filePath;

    public TMP_Text fileNameText;
    public TMP_Text lastModifiedText;

    public void SetOption(string filePath, string fileName, string modifiedDateTime)
    {
        this.filePath = filePath;
        fileNameText.text = fileName;
        lastModifiedText.text = modifiedDateTime;
    }
}
