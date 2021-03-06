﻿using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using LitJson;

public class RecordSpeech: MonoBehaviour
{
	private int m_RecordingRoutine = 0;

	private Mapper mapper;
	private string m_MicrophoneID = null;
	private AudioClip m_Recording = null;
	private int m_RecordingBufferSize = 2;
	private int m_RecordingHZ = 22050;
	ToneAnalyzer m_ToneAnalyzer = new ToneAnalyzer();
	public static double[] faceNormalizeVector;
	public static int numInstances = 0;
	// set public array
	public static Dictionary<string, double> tone_dict;
	public Text emotionText;
	public static string speechText;

	private SpeechToText m_SpeechToText = new SpeechToText();

	public Image emojiImage;

	public Sprite happySprite;
	public Sprite sadSprite;
	public Sprite angrySprite;
	public Sprite disgustSprite;
	public Sprite excitedSprite;
	public Sprite confidentSprite;
	public Sprite frustratedSprite;
	public Sprite fearSprite;

	void Start()
	{
		LogSystem.InstallDefaultReactors();
		mapper = new Mapper(emojiImage, happySprite, sadSprite, angrySprite, disgustSprite, excitedSprite, confidentSprite, frustratedSprite, fearSprite, tone_dict);
		Log.Debug("ExampleStreaming", "Start();");
		faceNormalizeVector = new double[24];
		Active = true;
		tone_dict = new Dictionary<string, double>();
		StartRecording();

	}

	public bool Active
	{
		get { return m_SpeechToText.IsListening; }
		set {
			if ( value && !m_SpeechToText.IsListening )
			{
				m_SpeechToText.DetectSilence = true;
				m_SpeechToText.EnableWordConfidence = false;
				m_SpeechToText.EnableTimestamps = false;
				m_SpeechToText.SilenceThreshold = 0.03f;
				m_SpeechToText.MaxAlternatives = 1;
				m_SpeechToText.EnableContinousRecognition = true;
				m_SpeechToText.EnableInterimResults = true;
				m_SpeechToText.OnError = OnError;
				m_SpeechToText.StartListening( OnRecognize );
			}
			else if ( !value && m_SpeechToText.IsListening )
			{
				m_SpeechToText.StopListening();
			}
		}
	}

	private void StartRecording()
	{
		if (m_RecordingRoutine == 0)
		{
			UnityObjectUtil.StartDestroyQueue();
			m_RecordingRoutine = Runnable.Run(RecordingHandler());
		}
	}

	private void StopRecording()
	{
		if (m_RecordingRoutine != 0)
		{
			Microphone.End(m_MicrophoneID);
			Runnable.Stop(m_RecordingRoutine);
			m_RecordingRoutine = 0;
		}
	}

	private void OnError( string error )
	{
		Active = false;

		Log.Debug("ExampleStreaming", "Error! {0}", error);
	}

	private IEnumerator RecordingHandler()
	{
		m_Recording = Microphone.Start(m_MicrophoneID, true, m_RecordingBufferSize, m_RecordingHZ);
		yield return null;      // let m_RecordingRoutine get set..

		if (m_Recording == null)
		{
			StopRecording();
			yield break;
		}

		bool bFirstBlock = true;
		int midPoint = m_Recording.samples / 2;
		float[] samples = null;

		while (m_RecordingRoutine != 0 && m_Recording != null)
		{
			int writePos = Microphone.GetPosition(m_MicrophoneID);
			if (writePos > m_Recording.samples || !Microphone.IsRecording(m_MicrophoneID))
			{
				Log.Error("MicrophoneWidget", "Microphone disconnected.");

				StopRecording();
				yield break;
			}

			if ((bFirstBlock && writePos >= midPoint)
				|| (!bFirstBlock && writePos < midPoint))
			{
				// front block is recorded, make a RecordClip and pass it onto our callback.
				samples = new float[midPoint];
				m_Recording.GetData(samples, bFirstBlock ? 0 : midPoint);

				AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(samples);
				record.Clip = AudioClip.Create("Recording", midPoint, m_Recording.channels, m_RecordingHZ, false);
				record.Clip.SetData(samples, 0);

				m_SpeechToText.OnListen(record);

				bFirstBlock = !bFirstBlock;
			}
			else
			{
				// calculate the number of samples remaining until we ready for a block of audio, 
				// and wait that amount of time it will take to record.
				int remaining = bFirstBlock ? (midPoint - writePos) : (m_Recording.samples - writePos);
				float timeRemaining = (float)remaining / (float)m_RecordingHZ;

				yield return new WaitForSeconds(timeRemaining);
			}

		}

		yield break;
	}

	private void OnRecognize(SpeechRecognitionEvent result)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach (var res in result.results)
			{
				foreach (var alt in res.alternatives)
				{
					string text = alt.transcript;
					Log.Debug("Streaming", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
					if (res.final) {
						speechText = text;
						NluSpeech.nluSpeech ();
						faceNormalizeVector = new double[24];
						numInstances++;
						for (int i = 0; i < 24; i++) {
							faceNormalizeVector [i] += FacialEmotion.faceVector [i];
							faceNormalizeVector [i] = faceNormalizeVector [i] / (double) numInstances;
						}
						numInstances = 0;
						m_ToneAnalyzer.GetToneAnalyze (OnGetToneAnalyze, text, text);

					} else {
						numInstances++;
						for (int i = 0; i < 24; i++) {
							faceNormalizeVector [i] += FacialEmotion.faceVector [i];
						}
					}
				}
			}
		}
	}

	private void OnGetToneAnalyze( ToneAnalyzerResponse resp , string data)
	{
		/*
		Response: [ToneAnalyzerResponse: document_tone=[DocumentTone: tone_categories=[ToneCategory: category_id=
		emotion_tone, category_name=Emotion Tone, tones=[Tone: score=0, tone_id=anger, tone_name=Anger][Tone: score=
		0, tone_id=disgust, tone_name=Disgust][Tone: score=0, tone_id=fear, tone_name=Fear][Tone: score=0, tone_id=
		joy, tone_name=Joy][Tone: score=0, tone_id=sadness, tone_name=Sadness]][ToneCategory: category_id=
		writing_tone, category_name=Writing Tone, tones=[Tone: score=0, tone_id=analytical, tone_name=Analytical][Tone: score=
		0, tone_id=confident, tone_name=Confident][Tone: score=0, tone_id=tentative, tone_name=Tentative]][ToneCategory: category_id=
		social_tone, category_name=Social Tone, tones=[Tone: score=0.288567, tone_id=openness_big5, tone_name=Openness][Tone: score=
		0.274434, tone_id=conscientiousness_big5, tone_name=Conscientiousness][Tone: score=0.543322, tone_id=extraversion_big5, tone_name=
		Extraversion][Tone: score=0.599483, tone_id=agreeableness_big5, tone_name=Agreeableness][Tone: score=0.292921, tone_id=
		neuroticism_big5, tone_name=Emotional Range]]], sentenceTone=No Sentence Tone] - okay 

		*/
		Debug.Log("Response: " +resp + " - " + data);

		// regex score=([0-1]\.*[0-9]*), tone_id=(\S+),
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
			tone_dict [m.Groups [2].Value] = x;
		}
		tone_dict ["Surprise"] = faceNormalizeVector [0];
		tone_dict ["Joy"] = faceNormalizeVector [1];
		tone_dict ["Sadness"] = faceNormalizeVector [2];
		tone_dict ["Anger"] = faceNormalizeVector [3];
		tone_dict ["Contempt"] = faceNormalizeVector [4];
		tone_dict ["Disgust"] = faceNormalizeVector [5];
		tone_dict ["Fear"] = faceNormalizeVector [6];
		tone_dict ["Valence"] = faceNormalizeVector [7];
		tone_dict ["Engagement"] = faceNormalizeVector [8];
		tone_dict ["Smile"] = faceNormalizeVector [9];
		tone_dict ["LipSuck"] = faceNormalizeVector [10];
		tone_dict ["Attention"] = faceNormalizeVector [11];
		tone_dict ["BrowFurrow"] = faceNormalizeVector [12];
		tone_dict ["BrowRaise"] = faceNormalizeVector [13];
		tone_dict ["ChinRaise"] = faceNormalizeVector [14];
		tone_dict ["EyeClosure"] = faceNormalizeVector [15];
		tone_dict ["InnerEyeBrowRaise"] = faceNormalizeVector [16];
		tone_dict ["LipCornerDepressor"] = faceNormalizeVector [17];
		tone_dict ["LipPress"] = faceNormalizeVector [18];
		tone_dict ["LipPucker"] = faceNormalizeVector [19];
		tone_dict ["MouthOpen"] = faceNormalizeVector [20];
		tone_dict ["NoseWrinkler"] = faceNormalizeVector [21];
		tone_dict ["Smirk"] = faceNormalizeVector [22];
		tone_dict ["UpperLipRaise"] = faceNormalizeVector [23];

		mapper.emotDict = tone_dict;
		mapper.map ();

		int y = 0;
		double currentMax = 0;
		string maxEmotWatson = "Happy";
		string maxBig5 = null;
		string maxBigThree = null;
		string maxEmotFace = null;
		foreach (KeyValuePair<string, double> entry in tone_dict) {
			Debug.Log (entry.Key + " : " + entry.Value);
			if ((y == 0 && y == 5) && ((y == 8 && y == 13) && y == 20)){
				currentMax = 0;
			}
			if (y < 5) {
				if (entry.Value > currentMax) {
					currentMax = entry.Value;
					maxEmotWatson = entry.Key;
				} 
			} else if (y < 8) {
				if (entry.Value > currentMax) {
					currentMax = entry.Value;
					maxBig5 = entry.Key;
				}
			} else if (y < 13) {
				if (entry.Value > currentMax) {
					currentMax = entry.Value;
					maxBigThree = entry.Key;
				}
			} else if (y < 20) {
				if (entry.Value > currentMax) {
					currentMax = entry.Value;
					maxEmotFace = entry.Key;
				}
			}
			y++;
		}
		emotionText.text = "Primary Emotion: " + "\n" + maxEmotWatson + "\n" + maxBig5 + "\n" + maxBigThree + "\n" + maxEmotFace;
	}

	static public bool JsonContainsKey(JsonData data,string key)
	{
		bool result = false;
		if(data == null)
			return result;
		if(!data.IsObject)
		{
			return result;
		}
		IDictionary tdictionary = data as IDictionary;
		if(tdictionary == null)
			return result;
		if(tdictionary.Contains(key))
		{
			result = true;
		}
		return result;
	}
}