using UnityEngine;

[CreateAssetMenu(fileName = "CardboardBoxData", menuName = "Kolonial/Box Data")]
public class CardboardBoxData : ScriptableObject
{
    public string boxID;
    public int nrGoodsInBox;
    //public Sprite crateImage;

    public GameObject stockedPrefab;
    public GameObject placedPrefab;
    public GameObject transparentPrefab;
}

