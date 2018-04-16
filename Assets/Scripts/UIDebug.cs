using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour {

    [SerializeField]
    private RectTransform scrollViewContent;

    [SerializeField]
    private Text text;

    //private bool debugIsVisible = false;

    private GameObject scrollView;

	void Start () {
        scrollView = scrollViewContent.transform.parent.parent.gameObject;

        CommandKeeper.WriteLineDebug += AddNewLine;
	}
	

    public void ChangeVisibility()
    {
        scrollView.SetActive(!scrollView.activeSelf);
    }

    private void AddNewLine(string str)
    {
        text.text = text.text + str + "\n";

        //scrollViewContent.rect.y = scrollViewContent.rect.height;
        //scrollViewContent.rect.top;
    }


}
