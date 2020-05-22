using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastMouse : MonoBehaviour{

    [SerializeField] GameBoardMechanics gameboard = default;
    public int clickedColorNumber = 0;
    public int clickedLockIndex = 0;
    private GameObject clickedSquare;

    public List<GameObject> mainSquares = new List<GameObject>();
    public List<SquareLockedIndexList> indexLists;
    private GameObject moveSquare; 
    private GameObject previousSquare;
    private bool backTracked = false;
    private bool firstSquare = false;

    public Color goodColor;
    public Color badColor;

    private void Update() {
        if (gameboard.touchEnabled == true) {
            RayCastForSquare();
        }
    }

    private void RayCastForSquare() {
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask_square = LayerMask.NameToLayer(layerName: "Square");
            RaycastHit2D hit_onSquare = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << layerMask_square);
            if (hit_onSquare.collider != null) {
                clickedSquare = hit_onSquare.collider.gameObject;
                SquareClicked(hit_onSquare.collider.gameObject);
                firstSquare = true;
            }
        }

        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask_square = LayerMask.NameToLayer(layerName: "Square");
            RaycastHit2D hit_onSquare = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << layerMask_square);
            if (hit_onSquare.collider != null) {
                if (clickedColorNumber != 0 && moveSquare != hit_onSquare.collider.gameObject /*&& clickedSquare != hit_onSquare.collider.gameObject*/) { // moved

                    previousSquare = moveSquare;
                    moveSquare = hit_onSquare.collider.gameObject;

                    //ignore move function on click down
                    if (firstSquare == true) {
                        firstSquare = false;
                    }
                    else {
                        Debug.Log("mouse over " + moveSquare.name);
                        MouseOverSquare(hit_onSquare.collider.gameObject);
                    }

                    CurrentListColorCheck();
                    ResetMainSquaresStatusCheck();
                    CheckGameWin();
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask_square = LayerMask.NameToLayer(layerName: "Square");
            RaycastHit2D hit_onSquare = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << layerMask_square);
            if (hit_onSquare.collider != null) {
                MouseUpSquare(hit_onSquare.collider.gameObject);
            }

            CurrentListColorCheck();
            ResetMainSquaresStatusCheck();
            CheckGameWin();

            clickedLockIndex = 0;
            clickedColorNumber = 0;
            clickedSquare = null;
            moveSquare = null;
            previousSquare = null;
            backTracked = false;
        }

    }


    private void SquareClicked(GameObject square) {
        Debug.Log("mouse down " + square.name);

        if (square.GetComponent<SquareMechanics>().gameNumber != 0) {
            clickedColorNumber = square.GetComponent<SquareMechanics>().gameNumber;
            clickedLockIndex = square.GetComponent<SquareMechanics>().lockedSquareIndex;
        }
    }

    private void MouseOverSquare(GameObject square) {

        //regular squares that have not been colored
        if (square.GetComponent<SquareMechanics>().gameNumber == 0) {

            int mainStatus = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus;

            if (mainStatus == 1) {
                MainStatusVertical(square);
            }
            else if (mainStatus == 2) {
                MainStatusHorizontal(square);
            }
            else {
                MainStatusNone(square);
            }
        }
        else if (square.GetComponent<SquareMechanics>().lockedSquareIndex == clickedLockIndex) {
            BackTrackToSquare(square,false);
        }
    }

    private void BackTrackToSquare(GameObject square, bool clickedSquare) {
        int squareIndex = square.GetComponent<SquareMechanics>().gamePositionIndex;
        int mainIndex = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionIndex;

        List<GameObject> ResetList = new List<GameObject>();

        if (squareIndex > mainIndex) {
            for (int i = 0; i < indexLists[clickedLockIndex - 1].squares.Count; i++) {
                if(indexLists[clickedLockIndex - 1].squares[i].GetComponent<SquareMechanics>().gamePositionIndex > squareIndex) {
                    Debug.Log("BackTrack Greater Reset "+ indexLists[clickedLockIndex - 1].squares[i].name);
                    ResetList.Add(indexLists[clickedLockIndex - 1].squares[i]);
                }
            }
        }
        else if(squareIndex < mainIndex) {
            for (int i = 0; i < indexLists[clickedLockIndex - 1].squares.Count; i++) {
                if (indexLists[clickedLockIndex - 1].squares[i].GetComponent<SquareMechanics>().gamePositionIndex < squareIndex) {
                    Debug.Log("BackTrack Lesser Reset " + indexLists[clickedLockIndex - 1].squares[i].name);
                    ResetList.Add(indexLists[clickedLockIndex - 1].squares[i]);
                }
            }
        }
        else {
            if (previousSquare != null) {
                ResetSquare(previousSquare);
            }
        }

        if(ResetList.Count > 0 && ResetList != null) {
            for (int i = 0; i < ResetList.Count; i++) {
                backTracked = true;
                ResetSquare(ResetList[i]);
            }
        }
        else {
            if (clickedSquare) {
                ResetSquare(square);
            }
        }
    }

    private void SetSquareInfo(GameObject square) {
        Debug.Log("Square Info Set " + square.name);
        indexLists[clickedLockIndex - 1].squares.Add(square);
        square.GetComponent<SquareMechanics>().gameNumber = clickedColorNumber;
        square.GetComponent<SquareMechanics>().lockedSquareIndex = clickedLockIndex;
    }

    private void MainStatusVertical(GameObject square) {
        int mainX = mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().gamePositionX;
        int squareX = square.GetComponent<SquareMechanics>().gamePositionX;

        int mainIndex = mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().gamePositionIndex;
        int squareIndex = square.GetComponent<SquareMechanics>().gamePositionIndex;

        if (mainX == squareX) {
            Debug.Log("Square is in the same vertical row as Main");
            if (squareIndex > mainIndex) {
                if (CheckIfAnyEmptyBetweenVertical(mainIndex, squareIndex) == 0) {
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("There is an Empty Square between this one and main");
                    Debug.Log("This Square: " + square.name);
                    Debug.Log("Main Square: " + mainSquares[clickedLockIndex-1].name);
                }
            }
            else {
                if (CheckIfAnyEmptyBetweenVertical(squareIndex, mainIndex) == 0) {
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("There is an Empty Square between this one and main");
                    Debug.Log("This Square: " + square.name);
                    Debug.Log("Main Square: " + mainSquares[clickedLockIndex-1].name);
                }
            }
        }
        else {
            Debug.Log("Square is not in the same vertical row as Main");
            CheckIfNextToMainHorizontal(square);
        }
    }

    private int CheckIfAnyEmptyBetweenVertical(int start, int end) {
        int emptyCounter = 0;
        for (int i = start + 1; i < end; i++) {
            Debug.Log("Checking index "+ i);
            if (gameboard.gameBoard_squares[i].GetComponent<SquareMechanics>().gameNumber == 0) {
                Debug.Log("fail check");
                emptyCounter++;
            }
            else if(gameboard.gameBoard_squares[i].GetComponent<SquareMechanics>().lockedSquareIndex != clickedLockIndex && gameboard.gameBoard_squares[i].GetComponent<SquareMechanics>().lockedSquareIndex != 0) {
                Debug.Log("fail check");
                emptyCounter++;
            }
            else {
                Debug.Log("pass check");
            }
        }

        return emptyCounter;
    }

    private void MainStatusHorizontal(GameObject square) {
        int mainY = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionY;
        int squareY = square.GetComponent<SquareMechanics>().gamePositionY;

        int mainIndex = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionIndex;
        int squareIndex = square.GetComponent<SquareMechanics>().gamePositionIndex;

        if (mainY == squareY) {
            Debug.Log("Square is in the same Horizontal row as Main");
            if (squareIndex > mainIndex) {
                if (CheckIfAnyEmptyBetweenHorizontal(mainIndex, squareIndex) == 0) {
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("There is an Empty Square between this one and main");
                    Debug.Log("This Square: " + square.name);
                    Debug.Log("Main Square: " + mainSquares[clickedLockIndex - 1].name);
                }
            }
            else {
                if (CheckIfAnyEmptyBetweenHorizontal(squareIndex, mainIndex) == 0) {
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("There is an Empty Square between this one and main");
                    Debug.Log("This Square: " + square.name);
                    Debug.Log("Main Square: " + mainSquares[clickedLockIndex - 1].name);
                }
            }
        }
        else {
            Debug.Log("Square is not in the same vertical row as Main");
            CheckIfNextToMainVert(square);
        }
        
    }

    private void CheckIfNextToMainVert(GameObject square) {
        Debug.Log("Checking to see if its vertical adjescent");
        int mainX = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionX;
        int squareX = square.GetComponent<SquareMechanics>().gamePositionX;
        int mainIndex = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionIndex;
        int squareindex = square.GetComponent<SquareMechanics>().gamePositionIndex;

        if (mainX == squareX) {
            if (squareindex == mainIndex + 1 || squareindex == mainIndex -1) {
                Debug.Log("yes its vertical adjescent");
                square.transform.parent.GetComponent<GameBoardMechanics>().ResetAllSquaresOnGameBoardWithLockedIndex(clickedLockIndex);
                RemoveAllSquaresInClickedIndexListExceptMain();
                mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus = 1;
                MainStatusVertical(square);
            }
        }
    }

    private void CheckIfNextToMainHorizontal(GameObject square) {
        Debug.Log("Checking to see if its horizontal adjescent");
        int mainY = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionY;
        int squareY = square.GetComponent<SquareMechanics>().gamePositionY;
        int mainIndex = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gamePositionIndex;
        int squareindex = square.GetComponent<SquareMechanics>().gamePositionIndex;

        if (mainY == squareY) {
            if (squareindex == mainIndex + gameboard.gameBoardHeight || squareindex == mainIndex - gameboard.gameBoardHeight) {
                Debug.Log("yes its horizontal adjescent");
                square.transform.parent.GetComponent<GameBoardMechanics>().ResetAllSquaresOnGameBoardWithLockedIndex(clickedLockIndex);
                RemoveAllSquaresInClickedIndexListExceptMain();
                mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus = 2;
                MainStatusHorizontal(square);
            }
        }
    }

    private int CheckIfAnyEmptyBetweenHorizontal(int start, int end) {
        int emptyCounter = 0;
        for (int i = start + gameboard.gameBoardHeight; i < end; i+=gameboard.gameBoardHeight) {
            Debug.Log("Checking index " + i);
            if (gameboard.gameBoard_squares[i].GetComponent<SquareMechanics>().gameNumber == 0) {
                Debug.Log("fail check");
                emptyCounter++;
            }
            else if (gameboard.gameBoard_squares[i].GetComponent<SquareMechanics>().lockedSquareIndex != clickedLockIndex && gameboard.gameBoard_squares[i].GetComponent<SquareMechanics>().lockedSquareIndex != 0) {
                Debug.Log("fail check");
                emptyCounter++;
            }
            else {
                Debug.Log("pass check");
            }
        }

        return emptyCounter;
    }

    private void MainStatusNone(GameObject square) {
        int mainX = mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().gamePositionX;
        int squareX = square.GetComponent<SquareMechanics>().gamePositionX;
        int mainY = mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().gamePositionY;
        int squareY = square.GetComponent<SquareMechanics>().gamePositionY;
        int mainIndex = mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().gamePositionIndex;
        int squareindex = square.GetComponent<SquareMechanics>().gamePositionIndex;

        if (mainX == squareX ) {
            Debug.Log("Square is in the same vertical row as the Main");

            if (squareindex > mainIndex) {
                if(squareindex == mainIndex+1) {
                    Debug.Log("Square "+ square.name+"is adjescent above Main "+ mainSquares[clickedLockIndex-1].name);
                    mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().lockStatus = 1;
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("Square is not adjescent above Main");
                }
            }
            else {
                if (squareindex == mainIndex-1) {
                    Debug.Log("Square is adjescent below Main");
                    mainSquares[clickedLockIndex-1].GetComponent<SquareMechanics>().lockStatus = 1;
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("Square is not adjescent below Main");
                }
            }
        }
        else if (mainY == squareY) {
            Debug.Log("Square is in the same horizontal row as the Main");

            if (squareindex > mainIndex) {
                if (squareindex == mainIndex + gameboard.gameBoardHeight) {
                    Debug.Log("Square " + square.name + "is adjescent Right Main " + mainSquares[clickedLockIndex - 1].name);
                    mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus = 2;
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("Square is not adjescent right Main");
                }
            }
            else {
                if (squareindex == mainIndex - gameboard.gameBoardHeight) {
                    Debug.Log("Square " + square.name + "is adjescent Left Main " + mainSquares[clickedLockIndex - 1].name);
                    mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus = 2;
                    SetSquareInfo(square);
                }
                else {
                    Debug.Log("Square is not adjescent left Main");
                }
            }
        }
        else {
            Debug.Log("Square not in the same vertical row or horizontal row as the Main");
        }
    }

    private void MouseUpSquare(GameObject square) {
        Debug.Log("mouse up " + square.name);

        //regular squares that have been colored
        if (square == clickedSquare && !square.GetComponent<SquareMechanics>().locked && square.GetComponent<SquareMechanics>().gameNumber > 1) {
            BackTrackToSquare(square,true);
        }
        
        //locked numbers are clicked resets them
        else if (square == clickedSquare && square.GetComponent<SquareMechanics>().locked) {
            square.transform.parent.GetComponent<GameBoardMechanics>().ResetAllSquaresOnGameBoardWithLockedIndex(clickedLockIndex);
            RemoveAllSquaresInClickedIndexListExceptMain();
        } 
    }

    private void RemoveAllSquaresInClickedIndexListExceptMain() {
        indexLists[clickedLockIndex - 1].squares.Clear();
        indexLists[clickedLockIndex - 1].squares.Add(mainSquares[clickedLockIndex - 1]);
    }

    public void ResetSquare(GameObject square) {
        Debug.Log("reset "+square.name);
        if (square != mainSquares[clickedLockIndex - 1]) {
            indexLists[clickedLockIndex - 1].squares.Remove(square);
        }
        square.GetComponent<SquareMechanics>().ResetSquare();
    }

    public void SetUpLockedIndexLists() {
        indexLists.Clear();
        mainSquares.Clear();
        indexLists = new List<SquareLockedIndexList>();
        for (int i = 0; i < gameboard.gameBoard_squares_locked.Count; i++) {
            indexLists.Add(new SquareLockedIndexList());
            indexLists[i].squares.Add(gameboard.gameBoard_squares_locked[i]);
            mainSquares.Add(gameboard.gameBoard_squares_locked[i]);
        }
    }

    private void ResetMainSquaresStatusCheck() {
        if (clickedLockIndex != 0) {
            if (indexLists[clickedLockIndex - 1].squares.Count == 1 && mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus != 0) {
                mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockStatus = 0;
            }
        }
    }

    private void CurrentListColorCheck() {
        if (clickedLockIndex != 0) {
            int goodNumber = mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().gameNumber;

            if (indexLists[clickedLockIndex - 1].squares.Count == goodNumber) {
                for (int i = 0; i < indexLists[clickedLockIndex - 1].squares.Count; i++) {
                    if (indexLists[clickedLockIndex - 1].squares[i].GetComponent<SquareMechanics>().spriteColor.color != goodColor) {
                        indexLists[clickedLockIndex - 1].squares[i].GetComponent<SquareMechanics>().SetSquareColorDisplay(goodColor);
                    }
                }
                mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockCompleted = true;
            }
            else {
                for (int i = 0; i < indexLists[clickedLockIndex - 1].squares.Count; i++) {
                    if (indexLists[clickedLockIndex - 1].squares[i].GetComponent<SquareMechanics>().spriteColor.color != badColor) {
                        indexLists[clickedLockIndex - 1].squares[i].GetComponent<SquareMechanics>().SetSquareColorDisplay(badColor);
                    }
                }
                mainSquares[clickedLockIndex - 1].GetComponent<SquareMechanics>().lockCompleted = false;
            }



        }
    }

    private void CheckGameWin() {
        gameboard.CheckGameWin();
    }

}

