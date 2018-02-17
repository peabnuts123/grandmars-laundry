using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndDialog : MonoBehaviour
{
    public bool isSuccess
    {
        get
        {
            return GetComponent<Animator>().GetBool("isSuccess");
        }
        set
        {
            GetComponent<Animator>().SetBool("isSuccess", value);
        }
    }

    public void TransitionToNextLevel()
    {
        GameManager._instance.TransitionToNextLevel(this.isSuccess);
        gameObject.SetActive(false);
    }
}
