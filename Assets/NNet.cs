using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Xml.Serialization;

public class NNet : MonoBehaviour {

	public static NeuralNetwork.NeuralNet net;
	public static List<DataSet> dataSets; 
	public static bool isTest = false;

	//Initialize Network and Dataset
	void Awake()
	{
		int numInputs, numHiddenLayers, numOutputs;
		if (File.Exists ("data.txt")) {
			net = ReadFromXmlFile<NeuralNet>("data.txt");
			Debug.Log ("THIS WORKS");
		} else {
			net = new NeuralNet (37, 2, 10);
			Debug.Log ("reached here");
		}
		dataSets = new List<DataSet>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public static void Train() {
		net.Train(dataSets, 0.05);

	}

	void OnApplicationQuit() {
		WriteToXmlFile<NeuralNet> ("data.txt", net, false);
	}

	public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
	{
		TextWriter writer = null;
		try
		{
			var serializer = new XmlSerializer(typeof(T));
			writer = new StreamWriter(filePath, append);
			serializer.Serialize(writer, objectToWrite);
		}
		finally
		{
			if (writer != null)
				writer.Close();
		}
	}

	/// <summary>
	/// Reads an object instance from an XML file.
	/// <para>Object type must have a parameterless constructor.</para>
	/// </summary>
	/// <typeparam name="T">The type of object to read from the file.</typeparam>
	/// <param name="filePath">The file path to read the object instance from.</param>
	/// <returns>Returns a new instance of the object read from the XML file.</returns>
	public static T ReadFromXmlFile<T>(string filePath) where T : new()
	{
		TextReader reader = null;
		try
		{
			var serializer = new XmlSerializer(typeof(T));
			reader = new StreamReader(filePath);
			return (T)serializer.Deserialize(reader);
		}
		finally
		{
			if (reader != null)
				reader.Close();
		}
	}
		
}
