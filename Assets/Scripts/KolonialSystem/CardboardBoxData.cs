using UnityEngine;

[CreateAssetMenu(fileName = "CardboardBoxData", menuName = "Kolonial/Box Data")]
public class CardboardBoxData : ScriptableObject
{
    public string boxID;
    public int nrOfGoods;
    //public Sprite crateImage;
    public GameObject prefab;
}
