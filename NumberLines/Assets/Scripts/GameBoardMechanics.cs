using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardMechanics : MonoBehaviour{

    [Header("GameBoard Settings")]
    public int gameBoardWidth = 8;
    public int gameBoardHeight = 8;
    public float borderSize = 0.25f;
    public bool touchEnabled = true;
    public Texture2D currentLevel;
    public List<Texture2D> allLevels = new List<Texture2D>();

    [Header("Game Objects")]
    [SerializeField] ColorToNumber[] colorNumbering = default;
    [SerializeField] GameObject cameraHolder = default;
    [SerializeField] GameObject square = default;
    [SerializeField] RaycastMouse raycast = default;

    public List<GameObject> gameBoard_squares = new List<GameObject>();
    public List<GameObject> gameBoard_squares_locked = new List<GameObject>();
    private List<GameObject> gameBoard_squares_keys = new List<GameObject>();


    public void StartGame() {
        GetLevel();
        SetUpCamera();
        CreateGameBoardSquares();
    }

    private void GetLevel() {
        currentLevel = allLevels[UnityEngine.Random.Range(0, allLevels.Count)];
        gameBoardWidth = currentLevel.width;
        gameBoardHeight = currentLevel.height;
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

        Color pixelColor = currentLevel.GetPixel(Convert.ToInt32(x), Convert.ToInt32(y));
        foreach (ColorToNumber colorNumber in colorNumbering) {
            if (colorNumber.color == pixelColor) {
                newSquare.GetComponent<SquareMechanics>().gameNumber = colorNumber.number;
                if (colorNumber.number > 1) {
                    gameBoard_squares_locked.Add(newSquare);
                    newSquare.GetComponent<SquareMechanics>().lockedSquareIndex = gameBoard_squares_locked.Count;
                }
                else if (colorNumber.number == 1) {
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
}
