using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyPanel : MonoBehaviour{

    [SerializeField] List<Image> DailyCircles = new List<Image>();
    public Color completedColor;
    public Color incompletedColor;
    private int day;


    private void Start() {
        int savedDate = PlayerPrefs.GetInt("SavedDate",0);
        int day = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
        if(savedDate != day) {
            Debug.Log("new day resetting daily panel " + day);
            savedDate = day;
            PlayerPrefs.SetInt("SavedDate", savedDate);
            PlayerPrefs.SetInt("DailyPuzzlesWon", 0);
        }
        ColorImages();
    }

    public void ColorImages() {
        int puzzlesWon = PlayerPrefs.GetInt("DailyPuzzlesWon",0);
        for (int i = 0; i < DailyCircles.Count; i++) {
            if (puzzlesWon-1 >= i) {
                DailyCircles[i].color = completedColor;
            }
            else {
                DailyCircles[i].color = incompletedColor;
            }
        }
    }
}
