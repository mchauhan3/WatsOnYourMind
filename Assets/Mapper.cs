using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Mapper {
	//RecordSpeech.tonedict
	// Use this for initialization
	public Dictionary<string, double> emotDict;

	public Image emojiImage;

	public Sprite happySprite;
	public Sprite sadSprite;
	public Sprite angrySprite;
	public Sprite disgustSprite;
	public Sprite excitedSprite;
	public Sprite confidentSprite;
	public Sprite frustratedSprite;
	public Sprite fearSprite;

	public Mapper(Image eImage, Sprite happy, Sprite sad, Sprite angry, Sprite disgust, Sprite excited, Sprite confident, Sprite frustrated, Sprite fear, Dictionary<string, double> dict) {
		emojiImage = eImage;
		happySprite = happy;
		angrySprite = angry;
		disgustSprite = disgust;
		excitedSprite = excited;
		confidentSprite = confident;
		frustratedSprite = frustrated;
		fearSprite = fear;
		emotDict = dict;
	}

	/*
	string expr = "score=([0-1]\\.*[0-9]*), tone_id=(\\S+),";
		MatchCollection mc = Regex.Matches(resp.ToString(), expr);
		foreach (Match m in mc)
		{
			//Debug.Log(m.ToString());
			// 1 is score, 2 is tone id name
			//Debug.Log("In match " + m + ", score is " + m.Groups[1].Value);
			//Debug.Log("In match " + m + ", tone_id is " + m.Groups[2].Value);
			double x;
			double.TryParse (m.Groups [1].Value, out x);
			emotDict [m.Groups [2].Value] = x;
		}
		*/

	// Update is called once per frame

	public void map() {
		double happiness = (emotDict ["Joy"] + emotDict ["Surprise"] + emotDict["Attention"] + emotDict["Smile"] + emotDict["openness_big5"] + emotDict["agreeableness_big5"] + emotDict["joy"]) / 7;
		double sadness = (emotDict["sadness"] + emotDict["Contempt"] + emotDict["BrowFurrow"] + emotDict["InnerEyeBrowRaise"] + emotDict["LipCornerDepressor"] + emotDict["Sadness"] + emotDict["neuroticism_big5"]) / 7;
		double anger = (emotDict["Contempt"] + emotDict["Anger"] + emotDict["Disgust"] + emotDict["Attention"] + emotDict["BrowFurrow"] + emotDict["neuroticism_big5"] + emotDict["anger"]) / 7;
		double disgust = (emotDict["Contempt"] + emotDict["Disgust"] + emotDict["Fear"] + emotDict["LipSuck"] + emotDict["EyeClosure"]  + emotDict["neuroticism_big5"] + emotDict["anger"] + emotDict["fear"]) / 8;
		double excitement = (emotDict["Joy"] + emotDict["Surprise"] + emotDict["Smile"] + emotDict["Attention"]  + emotDict["openness_big5"] + emotDict["extraversion_big5"] + emotDict["joy"]) / 7;
		double confident = (emotDict["Joy"] + emotDict["Attention"] + emotDict["Smile"] + emotDict["ChinRaise"] + emotDict["openness_big5"] + emotDict["joy"]) / 6;
		double frustration = (emotDict["Contempt"] + emotDict["Disgust"] + emotDict["Fear"] + emotDict["BrowFurrow"] + emotDict["InnerEyeBrowRaise"] + emotDict["LipPress"] + emotDict["sadness"] + emotDict["neuroticism_big5"] + emotDict["Sadness"] + emotDict["anger"]) / 10;
		double fear = (emotDict["Disgust"] + emotDict["Fear"] + emotDict["LipSuck"] + emotDict["BrowFurrow"] + emotDict["neuroticism_big5"] + emotDict["fear"]) / 6;

		Dictionary<string, double> dictionary = new Dictionary<string, double> ();
		dictionary.Add ("Happiness", happiness);
		dictionary.Add ("Sadness", sadness);
		dictionary.Add ("Anger", anger);
		dictionary.Add ("Disgust", disgust);
		dictionary.Add ("Excitement", excitement);
		dictionary.Add ("Confident", confident);
		dictionary.Add ("Frustration", frustration);
		dictionary.Add ("Fear", fear);

		//int[] emotions = new int[8] { happiness, sadness, anger, disgust, excitement, confident, frustration, fear };
		var top = dictionary.OrderByDescending (pair => pair.Value).Take(1).ToDictionary (pair => pair.Key, pair => pair.Value);
		string emotionToShow = null;
		foreach (KeyValuePair<string, double> entry in top) {
			emotionToShow = entry.Key;
		}
		if (emotionToShow.Equals ("Happiness")) {
			emojiImage.sprite = happySprite;
		} else if (emotionToShow.Equals ("Sadness")) {
			emojiImage.sprite = sadSprite;
		} else if (emotionToShow.Equals ("Anger")) {
			emojiImage.sprite = angrySprite;
		} else if (emotionToShow.Equals ("Disgust")) {
			emojiImage.sprite = disgustSprite;
		}
		else if (emotionToShow.Equals ("Excitement")) {
			emojiImage.sprite = excitedSprite;
		}
		else if (emotionToShow.Equals ("Confident")) {
			emojiImage.sprite = confidentSprite;
		}
		else if (emotionToShow.Equals ("Frustration")) {
			emojiImage.sprite = frustratedSprite;
		}
		else if (emotionToShow.Equals ("Fear")) {
			emojiImage.sprite = fearSprite;
		}
	}
}
