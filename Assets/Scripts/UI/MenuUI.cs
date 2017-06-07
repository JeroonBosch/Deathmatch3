using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuUI : MonoBehaviour
{
    private Transform _player1;
    private Transform _player2;

    private void Start()
    {
        RootController.Instance.StartNormalGame();
        _player1 = transform.Find("Canvas").Find("Player1");
        _player2 = transform.Find("Canvas").Find("Player2");
    }

    private void Update()
    {
        UpdatePlayer(0);
        UpdatePlayer(1);
    }

    private void UpdatePlayer(int playerNumber)
    {
        Player player = RootController.Instance.GetPlayer(playerNumber);
        if (player)
        {
            Transform colorSelect;
            if (playerNumber == 0)
                colorSelect = _player1.Find("ColorSelect");
            else
                colorSelect = _player2.Find("ColorSelect");


            List<Transform> colors = new List<Transform>();
            colors.Add(colorSelect.Find("ColorYellow"));
            colors.Add(colorSelect.Find("ColorBlue"));
            colors.Add(colorSelect.Find("ColorGreen"));
            colors.Add(colorSelect.Find("ColorRed"));

            Transform selected1 = null;
            for (int i = 0; i < colors.Count; i++)
            {
                Transform colorImage = colors[i];
                if (colorImage.Find("Selected1"))
                {
                    selected1 = colorImage.Find("Selected1");
                }
            }
    
            for (int i = 0; i < colors.Count; i++)
            {
                Transform colorImage = colors[i];

                if (player.selectedType.Type == TileTypes.ESubState.yellow + i)
                {
                    selected1.SetParent(colorImage, false);
                }

            }
        }
    }

    public void Handle_Engage()
    {
        RootController.Instance.StateController().State = StateBase.ESubState.Playing;
    }

    public void Handle_SelectColor(int index)
    {
        int playerNumber = 0;
        if (EventSystem.current.currentSelectedGameObject.tag == "Player0_UI")
            playerNumber = 0;
        else
            playerNumber = 1;

        Player player = RootController.Instance.GetPlayer(playerNumber);
        player.SelectColorByIndex(index);
    }

//Requirements
    public void ChangeBlueReqValue (string newValue)
    {
        RootController.Instance.GetSettings().BlueFillRequirement = float.Parse(newValue);
    }

    public void ChangeGreenReqValue(string newValue)
    {
        RootController.Instance.GetSettings().GreenFillRequirement = float.Parse(newValue);
    }

    public void ChangeRedReqValue(string newValue)
    {
        RootController.Instance.GetSettings().RedFillRequirement = float.Parse(newValue);
    }

    public void ChangeYellowReqValue(string newValue)
    {
        RootController.Instance.GetSettings().YellowFillRequirement = float.Parse(newValue);
    }

    // Damage/Heal value
    public void ChangeGreenValue(string newValue)
    {
        RootController.Instance.GetSettings().GreenValue = float.Parse(newValue);
    }

    public void ChangeRedValue(string newValue)
    {
        RootController.Instance.GetSettings().RedValue = float.Parse(newValue);
    }

    public void ChangeYellowValue(string newValue)
    {
        RootController.Instance.GetSettings().YellowValue = float.Parse(newValue);
    }

    // Multiplier
    public void ChangeSpecialityMultiplier(string newValue)
    {
        RootController.Instance.GetSettings().SpecialityMultiplier = float.Parse(newValue);
    }

}
