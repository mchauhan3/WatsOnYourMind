using Affdex;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FacialEmotion : ImageResultsListener
{
	public float currentSmile;
	public float currentInterocularDistance;
	public float currentSurprise;
	public float currentValence;
	public float currentAnger;
	public float currentFear;
	public float currentJoy;
	public float currentSadness;
	public float currentLipSuck;
	public Image emoji;

	public Sprite happy;
	public Sprite sad;
	public Sprite surprise;

	public FeaturePoint[] featurePointsList;

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
		Debug.Log("Got face results");

		foreach (KeyValuePair<int, Face> pair in faces)
		{
			int FaceId = pair.Key;  // The Face Unique Id.
			Face face = pair.Value;    // Instance of the face class containing emotions, and facial expression values.

			//Retrieve the Emotions Scores
			face.Emotions.TryGetValue(Emotions.Surprise, out currentSurprise);
			face.Emotions.TryGetValue(Emotions.Joy, out currentJoy);
			face.Emotions.TryGetValue(Emotions.Sadness, out currentSadness);
			if (currentSurprise > currentJoy && currentSurprise > currentSadness) {
				emoji.sprite = surprise;
			} else if (currentJoy > currentSurprise && currentJoy > currentSadness) {
				emoji.sprite = happy;
			} else {
				emoji.sprite = sad;
			}
//			Debug.Log("Anger: " + face.Emotions.TryGetValue(Emotions.Anger, out currentAnger));
//			Debug.Log("Joy: " + face.Emotions.TryGetValue(Emotions.Joy, out currentJoy));
//			face.Emotions.TryGetValue(Emotions.Fear, out currentFear);

			//Retrieve the Smile Score
			//face.Expressions.TryGetValue(Expressions.Smile, out currentSmile);
			//face.Expressions.TryGetValue (Expressions.LipSuck, out currentLipSuck);
			//Debug.Log("LipSuck Value: " + currentLipSuck);


			//Retrieve the Interocular distance, the distance between two outer eye corners.
			currentInterocularDistance = face.Measurements.interOcularDistance;


			//Retrieve the coordinates of the facial landmarks (face feature points)
			featurePointsList = face.FeaturePoints;

		}
	}
}