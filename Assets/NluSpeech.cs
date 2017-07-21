using FullSerializer;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class NluSpeech : MonoBehaviour
{
	static NaturalLanguageUnderstanding m_NaturalLanguageUnderstanding = new NaturalLanguageUnderstanding();
	private static fsSerializer sm_Serializer = new fsSerializer();
	public Text topics;


	public static void nluSpeech()
	{
		LogSystem.InstallDefaultReactors();

		Log.Debug("ExampleNaturalLanguageUnderstandingV1", "attempting to get models...");
		if (!m_NaturalLanguageUnderstanding.GetModels(OnGetModels))
			Log.Debug("ExampleNaturalLanguageUnderstandingV1", "Failed to get models.");

		Parameters parameters = new Parameters()
		{
			//text = "I am so mad right now, this demon is devouring me with the utmost perseverence. I am bound as a man of honor and valor to respect and commend his fastidiousness but in reality, I am not amused at all. Frankly, this whole ordeal has made me realize how power is a mere illusion.",
			text = RecordSpeech.speechText,
			return_analyzed_text = true,
			language = "en",
			features = new Features()
			{
				entities = new EntitiesOptions()
				{
					limit = 100,
					sentiment = true,
					emotion = true
				},
				keywords = new KeywordsOptions()
				{
					limit = 50,
					sentiment = true,
					emotion = true
				},
				concepts = new ConceptsOptions()
				{
					limit = 50,
				}

			}


		};

		AnalysisResults analysisResults = new AnalysisResults () 
		{
			analyzed_text = parameters.text,
			language = "en",
			sentiment = new SentimentResult() 
			{
				document = new DocumentSentimentResults()
				{
				}
			}
		};
				
		Log.Debug("ExampleNaturalLanguageUnderstandingV1", "attempting to analyze...");
		if (!m_NaturalLanguageUnderstanding.Analyze (OnAnalyze, parameters))
			Log.Debug ("ExampleNaturalLanguageUnderstandingV1", "Failed to get models.");
		//else
			//Log.Debug ("NLU", "Nicole: " + analysisResults.sentiment.document.score);
//			Log.Debug ("NLU", "NicoleFuck: " + analysisResults.keywords [0]);
	}

	private static void OnGetModels(ListModelsResults resp, string customData)
	{
		fsData data = null;
		sm_Serializer.TrySerialize(resp, out data).AssertSuccess();
		Log.Debug("ExampleNaturalLanguageUnderstandingV1", "ListModelsResult: {0}", data.ToString());
	}

	private static void OnAnalyze(AnalysisResults resp, string customData)
	{
		//Log.Debug ("NLU", "Nicole: " + resp.entities[0]);
		//Log.Debug ("NLU", "NicoleFuck: " + resp.keywords [0]);
		fsData data = null;
		sm_Serializer.TrySerialize(resp, out data).AssertSuccess();
		Log.Debug("ExampleNaturalLanguageUnderstandingV1", "AnalysisResults: {0}", data.ToString());
		JsonData obj = JsonMapper.ToObject (data.ToString ());
		Debug.Log (obj["concepts"]);
		string allText = "Topics: "+ "\n";
		if (RecordSpeech.JsonContainsKey (obj, "concepts")) {
			for (int j = 0; j < obj ["concepts"].Count; j++) {
				//Debug.Log (obj ["concepts"] [j] ["text"]);
				if (RecordSpeech.JsonContainsKey (obj ["concepts"] [j], "text")) {
				
					allText += (string) (obj ["concepts"] [j] ["text"] + "\n");
				}
			}
		}
		allText += "Keywords: " + "\n";
		if (RecordSpeech.JsonContainsKey(obj, "keywords")) {
			for (int j = 0; j < obj ["keywords"].Count; j++) {
				//Debug.Log (obj ["concepts"] [j] ["text"]);
				if (RecordSpeech.JsonContainsKey (obj ["keywords"] [j], "text")) {
					allText += (string) (obj ["keywords"] [j] ["text"] + "\n");
				}
			}
		}
		GameObject.Find("TopicsText").GetComponent<Text>().text = allText;
	}
}
