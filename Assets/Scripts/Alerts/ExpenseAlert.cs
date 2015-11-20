using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

using Prime31;

public class ExpenseAlert : UIView 
{
	#region Events
	public static event Action<Expense> createdExpense;
	#endregion

	#region Public Vars
	public static ExpenseAlert Instance = null;
	#endregion

	#region Private Vars
	Expense _item;
	#endregion

	#region Overidden Methods
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
	protected override void OnActivate ()
	{
		base.OnActivate ();
		ImageUploadAlert.uploadFile += FileSelected;
	}
	protected override void OnDeactivate ()
	{
		ImageUploadAlert.uploadFile -= FileSelected;
		base.OnDeactivate ();
	}
	#endregion

	#region UI Methods
	public void NameChanged(string name)
	{
		_item.Name = name;
	}
	public void DescriptionChanged(string description)
	{
		_item.Description = description;
	}
	public void DateChanged(string date)
	{
		DateTime temp = DateTime.Now;
		if(DateTime.TryParse(date, out temp))
			_item.Date = temp;
	}
	public void ValueChaned(string value)
	{
		double temp  = 0.0;
		if(double.TryParse(value,out temp))
			_item.Value = temp;
	}

	public void BillableChanged(bool toggled)
	{
		_item.Billable = toggled;
	}
	public void ReimbursementChanged(bool toggled)
	{
		_item.Reimbursement = toggled;
	}
	public void TagsChanged(string tagsStr)
	{
		string[] tags = tagsStr.Split(',');

		_item.Tags = tags.ToList();
	}
	public void Submit()
	{
		StartCoroutine(CreateExpense());
	}
	public void UploadImage()
	{
		#if UNITY_EDITOR
		UniFileBrowser.use.OpenFileWindow(ImageUploadAlert.Instance.Open);
		#elif UNITY_IPHONE
		EtceteraBinding.promptForPhoto(0.2f);
		#elif UNITY_ANDROID
		EtceteraAndroid.promptForPictureFromAlbum("");
		#endif
	}
	#endregion

	#region Methods
	public void Open(Project project)
	{
		_item = new Expense() { Project = project, User = ParseUser.CurrentUser };
		_item.ACL = new ParseACL();
		_item.ACL.SetRoleReadAccess(User.CurrentUser.CompanyName,true);
		_item.ACL.SetRoleWriteAccess(User.CurrentUser.CompanyName,true);

		UIAlertController.Instance.PresentAlert(this);
	}
	#endregion

	#region Coroutines
	IEnumerator CreateExpense()
	{
		Task task = _item.SaveAsync();

		LoadAlert.Instance.StartLoad("Creating Expense...",null,-1);

		while(!task.IsCompleted)
			yield return null;

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error:\n"+task.Exception);//handle the error
		}
		else
		{
			LoadAlert.Instance.SetText("Updating " + _item.Project.Name + "'s records.");
			_item.Project.ItemCount++;
			
			Task save = _item.Project.SaveAsync();
			
			while(!save.IsCompleted)
				yield return null;
			
			LoadAlert.Instance.Done();
			
			Deactivate();
			
			if(createdExpense != null)
				createdExpense(_item);
			
			_item = null;
			
			if(save.IsFaulted || save.Exception != null)
			{
				Debug.Log("error occured. UI may not be updated:\n"+task.Exception);
			}
		}
	}
	IEnumerator UploadFile(byte[] data)
	{
		ParseFile file = new ParseFile(_item.Name +".png",data);
		Task task = file.SaveAsync();

		while(!task.IsCompleted)
			yield return null;

		_item.Image = file;

		LoadAlert.Instance.Done();
	}
	#endregion

	#region EventListeners
	void FileSelected(byte[] data)
	{
		LoadAlert.Instance.StartLoad("Uploading Image...",null,-1);
		StartCoroutine(UploadFile(data));
	}
	#endregion
}
