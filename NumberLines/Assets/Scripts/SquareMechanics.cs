using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*<summary> game square attributes*/

public class SquareMechanics : MonoBehaviour{

    [Header("GameBoard Info")]
    public int gamePositionX = 0;
    public int gamePositionY = 0;
    public int gamePositionIndex = 0;

    [Header("Game Info")]
    public int gameNumber = 0;//in game number
    public int lockedSquareIndex = 0;//keep track of the locked square groups per puzzle
    public bool locked = false;//number squares
    public bool key = false;

    [Header("Display")]
    [SerializeField] SpriteRenderer spriteColor = default;
    [SerializeField] SpriteRenderer spriteNumber = default;
    [SerializeField] SpriteRenderer spriteKey = default;
    [SerializeField] List<Sprite> allNumbers = new List<Sprite>();



    public void SquareSetup() {
        Debug.Log(gameNumber);
        if (gameNumber > 1) {
            SetLockedSquare();
        }
        else if (gameNumber == 1) {
            SetKeySquare();
        }
    }

    private void SetKeySquare() {
        key = true;
        spriteKey.enabled = true;
        gameNumber = 0;
    }

    private void SetLockedSquare() {
        locked = true;
        spriteNumber.enabled = true;
        spriteNumber.sprite = allNumbers[gameNumber-2];//all numbers {2,3,4,5,6,7,8}
    }

}
