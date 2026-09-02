using NUnit.Framework;
using TMPro;
using UnityEngine;

public class GenerateSign : MonoBehaviour
{

    [SerializeField] private string sectionNr;
    [SerializeField] private string firstRow, secondRow, thirdRow;
    

    private void Awake()
    {
        GenerateAllTextMeshes();
    }

    private void GenerateAllTextMeshes()
    {
        TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text t in allTexts)
        {
            switch (t.gameObject.name)
            {
                case "sectionText": t.text = sectionNr; break;
                case "firstRowText": t.text = firstRow; break;
                case "secondRowText": t.text = secondRow; break;
                case "thirdRowText": t.text = thirdRow; break;
                default: t.text = ""; break;
            }
        }
    }

}
