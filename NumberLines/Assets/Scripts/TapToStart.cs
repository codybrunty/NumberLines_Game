using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToStart : MonoBehaviour {

    [SerializeField] GameObject midPanel = default;
    [SerializeField] GameBoardMechanics gameboard = default;
 
    public void TapToStartOnClick() {
        midPanel.SetActive(false);
        gameboard.StartGame();
    }

}
