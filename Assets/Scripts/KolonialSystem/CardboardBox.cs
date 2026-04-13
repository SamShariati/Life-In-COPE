using UnityEngine;
using UnityEngine.Playables;

public class CardboardBox
{
    public string boxID;
    public int nrOfGoods;
    //public Sprite crateImage;
    

    public CardboardBox(CardboardBoxData data)
    {
        boxID = data.boxID;
        nrOfGoods = data.nrOfGoods;
        //crateImage = data.crateImage;
    }
}
