using UnityEngine;

public class Shelf : MonoBehaviour
{

    Transform existingProduct;
    Transform emptyProduct;

    Transform firstLayer;
    Transform secondLayer;
    Transform thirdLayer;
    Transform fourthLayer;

    private void Start()
    {
        firstLayer = transform.Find("firstLayer");
        secondLayer = transform.Find("secondLayer");
        thirdLayer = transform.Find("thirdLayer");
        fourthLayer = transform.Find("fourthLayer");
    }
}
