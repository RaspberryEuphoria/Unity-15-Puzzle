using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuTextStyle : MonoBehaviour
{
    public bool isHover;
    public Color hovercolor = new Color(1, 0.5f, 1, 1);

    private TextMesh textmesh;
    private Color defaultColor;

    private void Start()
    {
        textmesh = GetComponent<TextMesh>();
        defaultColor = textmesh.color;
    }

    private void OnMouseEnter()
    {
        isHover = true;
        //textmesh.fontStyle = FontStyle.Bold;
        // textmesh.color = TextMesh.color

        StartCoroutine(SetTextTransparency(hovercolor));
    }

    private void OnMouseExit()
    {
        isHover = false;
        //textmesh.fontStyle = FontStyle.Normal;

        StartCoroutine(SetTextTransparency(defaultColor));
    }

    IEnumerator SetTextTransparency(Color endColor)
    {
        float t = 0;

        while (t < 1)
        {
            textmesh.color = Color32.Lerp(textmesh.color, endColor, t);
            t += Time.deltaTime / 0.5f;

            yield return null;
        }

    }
}
