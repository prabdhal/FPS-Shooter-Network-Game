using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    private void OnEnable()
    {
        anim.Play("FadeToBlack");
    }
}
