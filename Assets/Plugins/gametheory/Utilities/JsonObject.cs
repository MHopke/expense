using UnityEngine;
using System.Collections.Generic;
using MiniJSON;

public class JsonObject 
{
    #region Constants
    public const string INVALID = "invalid";
    #endregion

    #region Private Variables
    Dictionary<string,object> _fields;
    #endregion

    #region Constructors
    public JsonObject()
    {
        _fields = new Dictionary<string, object>();
    }
    public JsonObject(string json)
    {
        //Debug.Log("data: " + json);
        if (json != null)
        {
            if (json != "")
            {
                _fields = Json.Deserialize(json) as Dictionary<string,object>;

                if(_fields != null)
                    return;
            }
        }

        _fields = new Dictionary<string, object>();
    }
    public JsonObject(object obj)
    {
        if(obj != null)
        {
            _fields = obj as Dictionary<string,object>;
            return;
        }

        _fields = new Dictionary<string, object>();
    }
    #endregion

    #region Methods
    public bool HasField(string key)
    {
        return _fields.ContainsKey(key);
    }
    public T GetField<T>(string key)
    {
        if (_fields.ContainsKey(key))
        {
            return (T)_fields[key];
        }

        return default(T);
    }
    public object GetField(string key)
    {
        if (_fields.ContainsKey(key))
            return _fields[key];

        return INVALID;
    }
    public void SetField(string key, object field)
    {
        if (_fields.ContainsKey(key))
            _fields[key] = field;
        else
            _fields.Add(key, field);
    }
    public override string ToString()
    {
        return Json.Serialize(_fields);
    }
    #endregion

    #region Accessors
    public object this[string key]
    {
        get { return _fields[key]; }
    }
    public Dictionary<string,object> Fields
    {
        get { return _fields; }
    }
    #endregion
}
