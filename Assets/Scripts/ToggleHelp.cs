using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHelp : MonoBehaviour
{
    public static ToggleHelp _instance;

    public GameObject helpOverlay;

    public ToggleHelp()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

	public void HideHelpOverlay()
	{
		helpOverlay.SetActive(false);
	}

    public void ToggleHelpOverlay()
    {
        helpOverlay.SetActive(!helpOverlay.activeSelf);
    }
}
