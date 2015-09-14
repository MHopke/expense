using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Collections;

using gametheory.UI;

public class ImageUploadAlert : UIView 
{
	#region Events
	public static event System.Action<byte[]> uploadFile;
	#endregion

	#region Public Vars
	public InputField FileName;
	public RawImage Temp;

	public static ImageUploadAlert Instance = null;
	#endregion

	#region Private Vars
	byte[] _data;
	#endregion

	#region Overridden Methods
	protected override void OnInit ()
	{
		base.OnInit ();
		Instance = this;
	}
	protected override void OnCleanUp ()
	{
		Instance = null;
		base.OnCleanUp ();
	}
	#endregion

	#region UI Methods
	public void Confirm()
	{
		Deactivate();

		if(uploadFile != null)
			uploadFile(_data);

		_data = null;
	}
	#endregion

	#region Methods
	public void Open(string filePath)
	{
		UIAlertController.Instance.PresentAlert(this);

		string[] arr = filePath.Split(System.IO.Path.DirectorySeparatorChar);

		if(arr.Length > 0)
			FileName.text = arr[arr.Length - 1];

		try
		{
			if(File.Exists(filePath))
			{
				_data = File.ReadAllBytes(filePath);
				
				Texture2D tex = new Texture2D(2,2);
				tex.LoadImage(_data);

				Temp.texture = tex;
			}
		}
		catch(Exception e)
		{
		}
	}
	#endregion
}
