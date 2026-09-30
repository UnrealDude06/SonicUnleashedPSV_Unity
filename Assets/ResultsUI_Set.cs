using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class ResultsUI_Set : MonoBehaviour {
timerRetain sonic;
[SerializeField]private TMP_Text time,recordTime;
public Animator SonicResultsAnim;

string timer;
public int rank;
public Image RankImg;

public Sprite[] ranking_images;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (sonic == null)
        {
            sonic = GameObject.FindWithTag("time_retain").GetComponent<timerRetain>();
        }

        timer = sonic.timerToShow;  // Update every frame

		time.SetText(timer);
		recordTime.SetText(timer);

      

        // Correct format string to match the formatted time
    	TimeSpan timeSpan = TimeSpan.ParseExact(timer, "m':'ss'.'ff", null);

    	rank = GetRankBasedOnTime(timeSpan);

		// from 0 till 1'10" - S
		// 1'10" - A
		//1'20" - B
		//2'00" - C
		// 2'40" - D
		if(SonicResultsAnim != null)
		{
			SonicResultsAnim.SetInteger("Rank",rank);
		}

		RankImg.sprite = ranking_images[rank];

	}
	 int GetRankBasedOnTime(TimeSpan time)
    {
        // Define the thresholds for each rank
        TimeSpan sRankThreshold = new TimeSpan(0, 1, 10); // 1'10"
        TimeSpan aRankThreshold = new TimeSpan(0, 1, 20); // 1'20"
        TimeSpan bRankThreshold = new TimeSpan(0, 2, 0);  // 2'00"
        TimeSpan cRankThreshold = new TimeSpan(0, 2, 40); // 2'40"

        if (time <= sRankThreshold)
        {
            return 0; // S rank
        }
        else if (time <= aRankThreshold)
        {
            return 1; // A rank
        }
        else if (time <= bRankThreshold)
        {
            return 2; // B rank
        }
        else if (time <= cRankThreshold)
        {
            return 3; // C rank
        }
        else
        {
            return 4; // D rank
        }
    }
}
