using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Collections;

using gametheory.UI;

using Prime31;

public class ImageUploadAlert : UIView 
{
	#region Events
	public static event Action<byte[]> uploadFile;
	#endregion

	#region Constants
	const string UPLOAD_IMAGE = "Upload Image";
	const string CONFIRM = "Confirm";
	const string SORRY = "Sorry";

	const float RADIUS_PERCENT = 0.48f;
	#endregion

	#region Public Vars
	public Text ConfirmText;
	public Text Title;
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
#if UNITY_IPHONE
		EtceteraManager.imagePickerChoseImageEvent += OpenFile;
        #elif UNITY_ANDROID
        EtceteraAndroidManager.albumChooserSucceededEvent += Open;
#endif
	}
	protected override void OnCleanUp ()
	{
		Instance = null;
#if UNITY_IPHONE
			EtceteraManager.imagePickerChoseImageEvent -= OpenFile;
        #elif UNITY_ANDROID
        EtceteraAndroidManager.albumChooserSucceededEvent -= Open;
#endif
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
	public void Open()
	{
		#if UNITY_EDITOR
		UniFileBrowser.use.OpenFileWindow(ImageUploadAlert.Instance.OpenFile);
		#elif UNITY_IPHONE
		EtceteraBinding.promptForPhoto(0.2f);
		#elif UNITY_ANDROID
		EtceteraAndroid.promptForPictureFromAlbum("");
		#endif
	}
	void OpenFile(string filePath)
	{
		UIAlertController.Instance.PresentAlert(this);
        Texture2D tex = LoadImage(filePath);

        if(tex != null)
        {
			//treat the image as a square
			int dim = 0;
			float halfDim =0f;

			if(tex.width < tex.height)
				dim = tex.width;
			else
				dim = tex.height;

			halfDim = (float)dim / 2f;

			tex = CalculateTexture(dim,dim,(float)dim * RADIUS_PERCENT,halfDim,halfDim,tex);

			//convert new texture back to data
			_data = tex.EncodeToPNG();
			/*if(filePath.EndsWith(".png"))
				_data = tex.EncodeToPNG();
			else
				_data = tex.EncodeToJPG();*/

			Temp.texture = tex;
        }
	}
#if UNITY_ANDROID
    public Texture2D LoadImage( string imagePath )
    {
        // scale the image down to a reasonable size before loading
        EtceteraAndroid.scaleImageAtPath( imagePath, 0.1f );
        return EtceteraAndroid.textureFromFileAtPath( imagePath );
    }
#else
    public Texture2D LoadImage(string filePath)
    {
        string[] arr = filePath.Split(System.IO.Path.DirectorySeparatorChar);
        
        try
        {
            if(File.Exists(filePath))
            {
                _data = File.ReadAllBytes(filePath);
                
                Texture2D tex = new Texture2D(2,2);
                tex.LoadImage(_data);
                
                return tex;
            }
            else
                Debug.LogError(filePath + " doesn't exist");
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        
        return null;
    }
#endif
	Texture2D CalculateTexture(
		int h, int w,float r,float cx,float cy,Texture2D sourceTex
		)
	{
		Color [] c= sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);

		Texture2D b=new Texture2D(h,w);

		for (int i = 0 ; i<(h*w);i++)
		{
			int y=Mathf.FloorToInt(((float)i)/((float)w));
			int x=Mathf.FloorToInt(((float)i-((float)(y*w))));

			if (r*r>=(x-cx)*(x-cx)+(y-cy)*(y-cy))
				b.SetPixel(x, y, c[i]);
			else
				b.SetPixel(x, y, Color.clear);
		}

		b.Apply ();

		return b;
	}
	#endregion

	#region Event Listeners
	void LanguageChanged()
	{
		Title.text = UPLOAD_IMAGE;
		ConfirmText.text= CONFIRM;
	}
	#endregion
}
