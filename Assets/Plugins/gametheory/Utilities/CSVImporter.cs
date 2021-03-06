﻿using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace gametheory.Utilities
{
    public static class CSVImporter 
    {
		#region Public Vars
		public static bool LogInfo;
		#endregion

    	#region Methods
        public static List<T> GenerateList<T>(string filePath)
        {
            CSVMap map = new CSVMap(filePath);
			List<T> list = new List<T>();
			
			string header = "";
			
			System.Type type = typeof(T);
			PropertyInfo info = null;
			PropertyInfo[] properties = type.GetProperties();
            CSVColumn attribute = null;

            for (int index = 0; index < map.Contents.Count; index++)
            {
				T obj = Activator.CreateInstance<T>();
				
				for(int sub = 0; sub < properties.Length; sub++)
				{
					info = properties[sub];
					
					header = "";
					
                    object[] att = info.GetCustomAttributes(typeof(CSVColumn),false);
                    
                    if(att.Length > 0)
                    {
                        header = (att[0] as CSVColumn).Name;
                        if(map.Headers.Contains(header))
                        {
                            info.SetValue(obj,Convert.ChangeType(map.Contents[index].GetField(header),info.PropertyType) ,null);
                        }
                    }
				}
				
				if(LogInfo)
					Debug.Log(obj.ToString());
				
				list.Add(obj);
            }

            return list;
        }
		public static IEnumerator GenerateListAsync<T>(string filePath, Action<List<T>> callback)
		{
			CSVMap map = new CSVMap(filePath);
			List<T> list = new List<T>();

			string header = "";

			System.Type type = typeof(T);
			PropertyInfo info = null;
			PropertyInfo[] properties = type.GetProperties();

			for (int index = 0; index < map.Contents.Count; index++)
			{
				T obj = Activator.CreateInstance<T>();

				for(int sub = 0; sub < properties.Length; sub++)
				{
					info = properties[sub];

					header = "";

					object[] att = info.GetCustomAttributes(typeof(CSVColumn),false);

					if(att.Length > 0)
					{
						header = (att[0] as CSVColumn).Name;
						if(map.Headers.Contains(header))
						{
                            info.SetValue(obj,Convert.ChangeType(map.Contents[index].GetField(header),info.PropertyType) ,null);
						}
					}
				}

				if(LogInfo)
					Debug.Log(obj.ToString());

				list.Add(obj);

				yield return null;
			}

			if(callback != null)
				callback(list);
		}
        public static List<JsonObject> GetJSONList(string filePath)
        {
            return new CSVMap(filePath).Contents;
        }
    	#endregion
    }
    public class CSVMap
    {
    	#region Constants
    	const char ROW = '\n';
    	const char COLUMN = ',';
    	#endregion

    	#region Public Vars
        public List<string> Headers;
        public List<JsonObject> Contents;
    	#endregion

    	#region Constructors
    	public CSVMap()
    	{
			Headers = new List<string>();
            Contents = new List<JsonObject>();
    	}
        public CSVMap(string filePath)
    	{
			Contents = new List<JsonObject>();

    		string fileContent = "";
    		string[] rows = null;

            /*text = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
            fileContent = text.text;*/
			StreamReader streamReader = new StreamReader(filePath);
			
			fileContent = streamReader.ReadToEnd();

            rows = fileContent.Split(ROW);

            if (rows.Length > 0)
            {
                Headers = new List<string>(rows[0].Split(COLUMN));

                //trim return character
                Headers[Headers.Count-1] = Headers[Headers.Count-1].Replace("\r","");
            }

            if (rows.Length > 1)
            {
                for (int index = 1; index < rows.Length; index++)
                {
					string[] row = rows[index].Split(COLUMN);
					
                    JsonObject json = new JsonObject();

					for(int sub = 0; sub < Headers.Count; sub++)
					{
						json.SetField(Headers[sub],row[sub]);
					}
					Contents.Add(json);
                }
            }
    	}
    	#endregion
    }

	[AttributeUsage(AttributeTargets.Property)]
	public class CSVColumn : Attribute
	{
		#region Public Vars
        public string Name;
		#endregion
		
		#region Constructors
		public CSVColumn(string name)
		{
			Name = name;
		}
		#endregion
	}
}