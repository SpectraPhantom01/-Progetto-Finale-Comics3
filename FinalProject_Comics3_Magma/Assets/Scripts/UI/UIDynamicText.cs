using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDynamicText : MonoBehaviour
{
    [SerializeField] string mouseAndKeyboardText;
    [SerializeField] string joyStickText;
    [SerializeField] string gamePadText;
    TextMeshProUGUI textMesh;
    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        textMesh.text = LevelManager.Instance.JoyStickInputAvailable 
            ? joyStickText 
            : LevelManager.Instance.GamePadInputAvailable 
            ? gamePadText
            : mouseAndKeyboardText;
    }
}
