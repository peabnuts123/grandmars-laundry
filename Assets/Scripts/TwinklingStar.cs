using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinklingStar : MonoBehaviour
{

    // Use this for initialization
    void OnEnable()
    {
		Animator animator = GetComponent<Animator>();
		animator.SetFloat("cycleOffset", Random.Range(0F,2F));
		animator.speed = Random.Range(0.5F, 2F);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
