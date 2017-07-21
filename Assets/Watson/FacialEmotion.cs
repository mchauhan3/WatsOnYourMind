using Affdex;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FacialEmotion : ImageResultsListener
{
	public float currentEngagement;
	public float currentContempt;
	public float currentSurprise;
	public float currentValence;
	public float currentAnger;
	public float currentFear;
	public float currentJoy;
	public float currentSadness;
	public float currentDisgust;
	public Image emoji;

	//expressions
	public float currentSmile;
	public float currentInnerEyeBrowRaise;
	public float currentBrowRaise;
	public float currentBrowFurrow;
	public float currentNoseWrinkler;
	public float currentUpperLipRaise;
	public float currentLipCornerDepressor;
	public float currentChinRaise;
	public float currentLipPucker;
	public float currentLipPress;
	public float currentLipSuck;
	public float currentMouthOpen;
	public float currentSmirk;
	public float currentEyeClosure;
	public float currentAttention;
	public static double[] faceVector;

	public Sprite happy;
	public Sprite sad;
	public Sprite surprise;

	public FeaturePoint[] featurePointsList;

	void Start() {
		faceVector = new double[24];
	}

	public override void onFaceFound(float timestamp, int faceId)
	{
		Debug.Log("Found the face");
	}

	public override void onFaceLost(float timestamp, int faceId)
	{
		Debug.Log("Lost the face");
	}

	public override void onImageResults(Dictionary<int, Face> faces)
	{
		//Debug.Log("Got face results");

		foreach (KeyValuePair<int, Face> pair in faces)
		{
			int FaceId = pair.Key;  // The Face Unique Id.
			Face face = pair.Value;    // Instance of the face class containing emotions, and facial expression values.

			//Retrieve the Emotions Scores
			face.Emotions.TryGetValue(Emotions.Surprise, out currentSurprise);
			faceVector [0] = currentSurprise/100;

			face.Emotions.TryGetValue(Emotions.Joy, out currentJoy);
			faceVector [1] = currentJoy/100;

			face.Emotions.TryGetValue(Emotions.Sadness, out currentSadness);
			faceVector [2] = currentSadness/100;

			face.Emotions.TryGetValue(Emotions.Anger, out currentAnger);
			faceVector [3] = currentAnger/100;

			face.Emotions.TryGetValue(Emotions.Contempt, out currentContempt);
			faceVector [4] = currentContempt/100;

			face.Emotions.TryGetValue(Emotions.Disgust, out currentDisgust);
			faceVector [5] = currentDisgust/100;

			face.Emotions.TryGetValue(Emotions.Fear, out currentFear);
			faceVector [6] = currentFear/100;

			face.Emotions.TryGetValue(Emotions.Valence, out currentValence);
			faceVector [7] = currentValence/100;

			face.Emotions.TryGetValue(Emotions.Engagement, out currentEngagement);
			faceVector [8] = currentEngagement/100;





			if (currentSurprise > currentJoy && currentSurprise > currentSadness) {
				emoji.sprite = surprise;
			} else if (currentJoy > currentSurprise && currentJoy > currentSadness) {
				emoji.sprite = happy;
			} else {
				emoji.sprite = sad;
			}

			face.Expressions.TryGetValue(Expressions.Smile, out currentSmile);
			faceVector [9] = currentSmile/100;

			face.Expressions.TryGetValue (Expressions.LipSuck, out currentLipSuck);
			faceVector [10] = currentLipSuck/100;

			face.Expressions.TryGetValue (Expressions.Attention, out currentAttention);
			faceVector [11] = currentAttention/100;

			face.Expressions.TryGetValue (Expressions.BrowFurrow, out currentBrowFurrow);
			faceVector [12] = currentBrowFurrow/100;

			face.Expressions.TryGetValue (Expressions.BrowRaise, out currentBrowRaise);
			faceVector [13] = currentBrowRaise/100;

			face.Expressions.TryGetValue (Expressions.ChinRaise, out currentChinRaise);
			faceVector [14] = currentChinRaise/100;

			face.Expressions.TryGetValue (Expressions.EyeClosure, out currentEyeClosure);
			faceVector [15] = currentEyeClosure/100;

			face.Expressions.TryGetValue (Expressions.InnerBrowRaise, out currentInnerEyeBrowRaise);
			faceVector [16] = currentInnerEyeBrowRaise/100;

			face.Expressions.TryGetValue (Expressions.LipCornerDepressor, out currentLipCornerDepressor);
			faceVector [17] = currentLipCornerDepressor/100;

			face.Expressions.TryGetValue (Expressions.LipPress, out currentLipPress);
			faceVector [18] = currentLipPress/100;

			face.Expressions.TryGetValue (Expressions.LipPucker, out currentLipPucker);
			faceVector [19] = currentLipPucker/100;

			face.Expressions.TryGetValue (Expressions.MouthOpen, out currentMouthOpen);
			faceVector [20] = currentMouthOpen/100;

			face.Expressions.TryGetValue (Expressions.NoseWrinkle, out currentNoseWrinkler);
			faceVector [21] = currentNoseWrinkler/100;

			face.Expressions.TryGetValue (Expressions.Smirk, out currentSmirk);
			faceVector [22] = currentSmirk/100;

			face.Expressions.TryGetValue (Expressions.UpperLipRaise, out currentUpperLipRaise);
			faceVector [23] = currentUpperLipRaise/100;

			//Retrieve the Interocular distance, the distance between two outer eye corners.
			//currentInterocularDistance = face.Measurements.interOcularDistance;


			//Retrieve the coordinates of the facial landmarks (face feature points)
			featurePointsList = face.FeaturePoints;
		}
	}
}