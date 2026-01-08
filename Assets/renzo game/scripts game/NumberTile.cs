using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumberTile : MonoBehaviour
{
    public int number;
    public Image image;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (image != null)
        {
            image.color = normalColor;
        }
    }

    public void OnClick()
    {
        gameManager.OnTileClicked(number);
    }

    public void Highlight()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        if (image != null)
        {
            image.color = highlightColor;
            yield return new WaitForSeconds(0.4f);
            image.color = normalColor;
        }
    }
}
