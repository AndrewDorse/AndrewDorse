using UnityEngine;
using UnityEngine.UI;

public class LineBetweenObjects : MonoBehaviour
{
    
    public GameObject leftButton;
    public GameObject mousePosition;

    private RectTransform object1;
    private RectTransform object2;
    private Image image;
    private RectTransform rectTransform;

    public Color color;
    public bool finish = false;

    void Start()
    {

        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        SetObjects(leftButton);

        image.color = color;
    }



    public void SetObjects(GameObject one)
    {
        object1 = one.GetComponent<RectTransform>();
        object2 = mousePosition.GetComponent<RectTransform>();

    }
   
    void Update()
    {
        if (finish == false)
        {
            Vector3 сhangePos = Input.mousePosition;
            
            object2.position = сhangePos;

            rectTransform.localPosition = (object1.localPosition + object2.localPosition) / 2;
            Vector3 dif = object2.localPosition - object1.localPosition;
            rectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
           
        }
    }

    public void FinishLine(RectTransform finishPoint)
    {
        if (finish == false)
        {
            object2.position = finishPoint.position;
            rectTransform.localPosition = (object1.localPosition + object2.localPosition) / 2;
            Vector3 dif = object2.localPosition - object1.localPosition;
            rectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
            finish = true;
        }
    }
}
