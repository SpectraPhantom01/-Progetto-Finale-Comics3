using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] SaveSO saveAsset;
    [SerializeField] Button continueButton;
    private void Awake()
    {
        if (saveAsset.SceneName != string.Empty)
            continueButton.interactable = true;
    }

    public void ContinueGame()
    {
        LevelManager.Instance.LoadScene(saveAsset.SceneName);
    }
}
