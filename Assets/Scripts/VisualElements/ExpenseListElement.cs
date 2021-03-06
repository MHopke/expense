﻿using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class ExpenseListElement : VisualElement 
{
	#region Events
	public static event System.Action<ExpenseListElement, Project> switchedProject;
	public static event System.Action<ExpenseListElement> remove;
	#endregion

	#region Constants
	const string VALUE_PREFIX = "$";
	const string USER_PREFIX = "Submitted by: ";
	#endregion
	
	#region Public Vars
	public Text UserText;
	public Text ValueLabel;

	public RawImage Image;
	
	public Image Background;

	public Button PhotoButton;
	public Button DeleteButton;

	public Toggle Billable;
	public Toggle Reimbursement;
	public Toggle Reimbursed;

	public InputField Namefield;
	public InputField Descriptionfield;
	public InputField Datefield;
	public InputField Valuefield;

	public Dropdown ProjectDropdown;
	#endregion
	
	#region Private Vars
	bool _billable, _reimbursed, _reimbursable;

	int _projectIndex;

	string _name,_description,_valueString, _dateString;

	DateTime _date;

	double _value;

	Expense _item;
	#endregion
	
	#region Overidden Methods
	public override void PresentVisuals (bool display)
	{
		base.PresentVisuals (display);

		if(ValueLabel)
			ValueLabel.enabled = display;

		if(Namefield)
			Namefield.enabled = display;
		
		if(Descriptionfield)
			Descriptionfield.enabled = display;
		
		if(Datefield)
			Datefield.enabled = display;
		
		if(UserText)
			UserText.enabled = display;

		if(Image)
			Image.enabled = display;
		
		if(Billable)
		{
			Billable.enabled = display;

			Billable.targetGraphic.enabled = display;
			Billable.graphic.enabled = display;
		}
		
		if(Reimbursement)
		{
			Reimbursement.enabled = display;
			
			Reimbursement.targetGraphic.enabled = display;
			Reimbursement.graphic.enabled = display;
		}

		if(Datefield)
			Datefield.enabled = display;

		if(Valuefield)
			Valuefield.enabled = display;
		
		if(Background)
			Background.enabled = display;

		if(PhotoButton)
		{
			PhotoButton.enabled = display;
			PhotoButton.image.enabled = display;
		}

		if(DeleteButton)
		{
			DeleteButton.enabled = display;
			DeleteButton.image.enabled = display;
		}

		if(ProjectDropdown)
			ProjectDropdown.enabled = display;
	}
	protected override void OnPresent ()
	{
		base.OnPresent ();
		ImageUploadAlert.uploadFile += FileChosen;
	}
	protected override void OnRemove ()
	{
		base.OnRemove ();
		ImageUploadAlert.uploadFile -= FileChosen;
	}
	#endregion

	#region UI Methods
	public void NameEdited(string name)
	{
		if(_name != name)
		{
			_item.Name = name;

			StartCoroutine(SaveChanges());
		}
	}
	public void DescriptionEdited(string description)
	{
		if(_description != _description)
		{
			_item.Description = description;
			StartCoroutine(SaveChanges());
		}
	}
	public void DateEdited(string date)
	{
		if(_dateString != date && DateTime.TryParse(date, out _date))
		{
			_item.Date = _date;
			StartCoroutine(SaveChanges());
		}
	}
	public void ValueEdited(string value)
	{
		if(_valueString != value && double.TryParse(value, out _value))
		{
			_item.Value = _value;
		   StartCoroutine(SaveChanges());
		}
	}
	public void BillableToggled(bool toggled)
	{
		if(_billable != toggled)
		{
			_item.Billable = toggled;
			StartCoroutine(SaveChanges());
		}
	}
	public void ReimbursementToggled(bool toggled)
	{
		if(_reimbursable != toggled)
		{
			_item.Reimbursement = toggled;
			StartCoroutine(SaveChanges());
		}
	}
	public void ReimbursedToggled(bool toggled)
	{
		if(_reimbursed != toggled)
		{
			_item.Reimbursed = toggled;
			StartCoroutine(SaveChanges());
		}
	}
	public void UploadPhoto()
	{
		ImageUploadAlert.Instance.Open();
	}
	public void Delete()
	{
		StartCoroutine(DeleteCoroutine());
	}
	public void ProjectChanged(int choice)
	{
		Project project = Database.Instance.Projects[choice];

		if(_item.Project.ObjectId != project.ObjectId)
			StartCoroutine(SwitchCoroutines(project));
	}
	#endregion

	#region Methods
	public void SetEditable(bool projectClosed)
	{
		if(projectClosed)
		{
			Billable.interactable = false;
			Reimbursement.interactable = false;
			Namefield.interactable = false;
			Descriptionfield.interactable = false;
			Valuefield.interactable = false;
			Datefield.interactable = false;
			PhotoButton.interactable = false;
			
			if(_item.IsOwner(ParseUser.CurrentUser))
			{
				ProjectDropdown.interactable = false;
				DeleteButton.interactable = false;
			}
		}
		else
		{
			Billable.interactable = true;
			Reimbursement.interactable = true;
			Namefield.interactable = true;
			Descriptionfield.interactable = true;
			Valuefield.interactable = true;
			Datefield.interactable = true;
			PhotoButton.interactable = true;
			
			if(_item.IsOwner(ParseUser.CurrentUser))
			{
				Reimbursed.interactable = true;
				ProjectDropdown.interactable = true;
				DeleteButton.interactable = true;
			}
		}
	}
	void SetData()
	{
		_reimbursed = _item.Reimbursed;
		_reimbursable = _item.Reimbursement;
		_billable = _item.Billable;
		
		_name = _item.Name;
		_description = _item.Description;
		_value = _item.Value;
		_valueString = _item.Value.ToString();
		_date = _item.Date;
		_dateString = _item.Date.ToShortDateString();

		_projectIndex = Database.Instance.GetProjectIndex(_item.Project);
	}
	public void Setup(Expense item)
	{
		_item = item;

		SetData();

		Namefield.text = item.Name;
		Descriptionfield.text = item.Description;

		Datefield.text = item.Date.ToShortDateString();
		Valuefield.text = VALUE_PREFIX + item.Value.ToString();

		UserText.text = USER_PREFIX + item.User[User.NAME];

		Billable.isOn = item.Billable;
		Reimbursement.isOn = item.Reimbursement;
		Reimbursed.isOn = item.Reimbursed;

		if(item.IsOwner(ParseUser.CurrentUser))
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for(int index = 0; index < Database.Instance.Projects.Count; index++)
			{
				options.Add(new Dropdown.OptionData(Database.Instance.Projects[index].Name));
			}
			ProjectDropdown.options = options;
			ProjectDropdown.value = Database.Instance.GetProjectIndex(_item.Project);
		}
		else
		{
			DeleteButton.gameObject.SetActive(false);
			Reimbursed.interactable = false;
			ProjectDropdown.gameObject.SetActive(false);
		}

		if(item.Image != null)
			StartCoroutine(LoadImage());
	}
	#endregion

	#region Coroutines
	IEnumerator LoadImage()
	{
		WWW request = new WWW(_item.Image.Url.AbsoluteUri);

		while(!request.isDone)
			yield return null;

		Texture2D tex = new Texture2D(1,1);

		request.LoadImageIntoTexture(tex);

		Image.texture = tex;
	}
	IEnumerator UploadImage(byte[] data)
	{
		LoadAlert.Instance.StartLoad("Uploading photo...",null,-1);

		ParseFile file = new ParseFile(_item.Name +".png",data);
		Task task = file.SaveAsync();
		
		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.Exception == null)
			DefaultAlert.Present("Sorry!", "An error occurred while uploading your photo. Please try again");
		else
		{
			_item.Image = file;

			task = _item.SaveAsync();

			while(!task.IsCompleted)
				yield return null;

			if(task.Exception == null)
			{
				Texture2D tex = new Texture2D(1,1);
				tex.LoadImage(data);

				Image.texture = tex;
			}
			else
				DefaultAlert.Present("Sorry!", "An error occurred while uploading your photo. Please try again");
		}
	}
	IEnumerator SaveChanges()
	{
		LoadAlert.Instance.StartLoad("Saving changes...",null,-1);
		Task task = _item.SaveAsync();

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.Exception == null)
			SetData();
		else
		{
			_item.Name = _name;
			_item.Description = _description;
			_item.Date = DateTime.Parse(_dateString);
			_item.Value = double.Parse(_valueString);

			_item.Billable = _billable;
			_item.Reimbursement = _reimbursable;
			_item.Reimbursed = _reimbursed;
		}
	}
	IEnumerator DeleteCoroutine()
	{
		LoadAlert.Instance.StartLoad("Deleting expense...",null,-1);

		Task task = _item.DeleteAsync();

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.Exception == null)
		{
			if(remove != null)
				remove(this);
		}
		else
			DefaultAlert.Present("Sorry!", "An error occured while trying " +
				"to delete this expense. Pleae try again later");
	}
	IEnumerator SwitchCoroutines(Project project)
	{
		Project prevProj = _item.Project;
		_item.Project = project;

		Task task = _item.SaveAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.Exception == null)
		{
			if(switchedProject != null)
				switchedProject(this,prevProj);
		}
		else
		{
			DefaultAlert.Present("Sorry!", "An error occured while trying " +
			             "to delete this expense. Pleae try again later");
			_item.Project = prevProj;
		}
	}
	#endregion

	#region Event Listeners
	public void FileChosen(byte[] data)
	{
		StartCoroutine(UploadImage(data));
	}
	#endregion

	#region Properties
	public Expense Item
	{
		get { return _item; }
	}
	#endregion
}
