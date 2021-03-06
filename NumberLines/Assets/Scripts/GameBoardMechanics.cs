﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class GameBoardMechanics : MonoBehaviour{

    [Header("GameBoard Settings")]
    public int gameBoardWidth = 8;
    public int gameBoardHeight = 8;
    public float borderSize = 0.25f;
    public bool touchEnabled = false;
    public string currentLevel;
    [SerializeField] TextAsset allPuzzlesFile;
    public List<Texture2D> allLevels = new List<Texture2D>();

    [Header("Game Objects")]
    [SerializeField] SymbolToNumber[] symbolNumbering = default;
    [SerializeField] GameObject cameraHolder = default;
    [SerializeField] GameObject square = default;
    [SerializeField] RaycastMouse raycast = default;
    [SerializeField] GameObject tapPanel = default;
    [SerializeField] TextMeshProUGUI displayText = default;

    public List<GameObject> gameBoard_squares = new List<GameObject>();
    public List<GameObject> gameBoard_squares_locked = new List<GameObject>();
    private List<GameObject> gameBoard_squares_keys = new List<GameObject>();

    private string[] puzzleLines;
    private int currentLevelIndex;

    private void Awake() {
        LoadPuzzles();
    }

    public void StartGame() {
        touchEnabled = true;
        GetLevel();
        SetUpCamera();
        CreateGameBoardSquares();
    }

    private void GetLevel() {
        currentLevelIndex = UnityEngine.Random.Range(0, puzzleLines.Length);
        currentLevel = puzzleLines[currentLevelIndex].Trim();
        currentLevel = currentLevel.Substring(0, (currentLevel.Length - 1));
        string[] rows=currentLevel.Split(',');
        currentLevel = string.Join("", rows);
        gameBoardWidth = rows[0].Length;
        gameBoardHeight = rows.Length;
        displayText.text = (currentLevelIndex+1).ToString();
    }

    private void LoadPuzzles() {
        puzzleLines = allPuzzlesFile.text.Split('\n');
    }

    private void SetUpCamera() {
        cameraHolder.transform.position = new Vector3((float)(gameBoardWidth - 1) / 2f, (float)(gameBoardHeight - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)gameBoardHeight / 2f + (borderSize);
        float horizontalSize = ((float)gameBoardWidth / 2f + (borderSize)) / aspectRatio;

        if (verticalSize > horizontalSize) {
            Camera.main.orthographicSize = verticalSize;
        }
        else {
            Camera.main.orthographicSize = horizontalSize;
        }

        cameraHolder.transform.localPosition = cameraHolder.transform.localPosition + new Vector3(0f, 0f, 0f);
    }

    private void CreateGameBoardSquares() {
        for (int x = 0; x < gameBoardWidth; x++) {
            for (int y = 0; y < gameBoardHeight; y++) {
                Generate_GameboardSquare(x, y);
            }
        }
        raycast.SetUpLockedIndexLists();
    }

    private void Generate_GameboardSquare(float x, float y) {
        Vector3 squareSpawnPoint = new Vector3(x, y, 0);
        GameObject newSquare = Instantiate(square, squareSpawnPoint, Quaternion.identity, gameObject.transform);

        newSquare.name = "Square_" + (Convert.ToInt32(x)).ToString() + "," + (Convert.ToInt32(y)).ToString();
        newSquare.GetComponent<SquareMechanics>().gamePositionX = Convert.ToInt32(x);
        newSquare.GetComponent<SquareMechanics>().gamePositionY = Convert.ToInt32(y);
        newSquare.GetComponent<SquareMechanics>().gamePositionIndex = gameBoard_squares.Count;

        int currentLevelIndex = (((gameBoardHeight - 1) - (int)y) * gameBoardWidth) + (int)x;
        char textSymbol = currentLevel[currentLevelIndex];
        //Debug.Log(textSymbol);

        foreach (SymbolToNumber symbolNumber in symbolNumbering) {
            if (symbolNumber.symbol == textSymbol) {
                newSquare.GetComponent<SquareMechanics>().gameNumber = symbolNumber.number;
                if (symbolNumber.number > 1) {
                    gameBoard_squares_locked.Add(newSquare);
                    newSquare.GetComponent<SquareMechanics>().lockedSquareIndex = gameBoard_squares_locked.Count;
                }
                else if (symbolNumber.number == 1) {
                    gameBoard_squares_keys.Add(newSquare);
                }
            }
        }
        
        newSquare.GetComponent<SquareMechanics>().SquareSetup();
        gameBoard_squares.Add(newSquare);
    }

    public void ResetAllSquaresOnGameBoardWithLockedIndex(int lockedIndex) {
        for (int i = 0; i < gameBoard_squares.Count; i++) {
            if (gameBoard_squares[i].GetComponent<SquareMechanics>().lockedSquareIndex == lockedIndex && gameBoard_squares[i].GetComponent<SquareMechanics>().locked == false) {
                gameBoard_squares[i].GetComponent<SquareMechanics>().ResetSquare();
            }
        }
    }

    public void CheckGameWin() {
        int badConditions = 0;

        for (int i = 0; i < gameBoard_squares_keys.Count; i++) {
            if (gameBoard_squares_keys[i].GetComponent<SquareMechanics>().gameNumber == 0) {
                badConditions++;
            }
        }

        for (int i = 0; i < gameBoard_squares_locked.Count; i++) {
            if (gameBoard_squares_locked[i].GetComponent<SquareMechanics>().lockCompleted != true) {
                badConditions++;
            }
        }

        if (badConditions == 0) {
            PuzzleWin();
        }

    }

    private void PuzzleWin() {
        Debug.Log("Puzzle Completed!");
        touchEnabled = false;
        ClearGameItems();
        TapToStartOn();
    }

    private void TapToStartOn() {
        tapPanel.SetActive(true);
    }

    private void ClearGameItems() {
        gameBoard_squares.Clear();
        gameBoard_squares_locked.Clear();
        gameBoard_squares_keys.Clear();
        foreach (Transform child in gameObject.transform){
            Destroy(child.gameObject);
        }
    }

}