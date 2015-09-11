﻿using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class ExpenseAlert : UIView 
{
	#region Events
	public static event Action<ExpenseItem> createdExpense;
	#endregion

	#region Public Vars
	public static ExpenseAlert Instance = null;
	#endregion

	#region Private Vars
	ExpenseItem _item;
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

		Debug.Log(temp);
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
	#endregion

	#region Methods
	public void Open(Project project)
	{
		_item = new ExpenseItem() { Project = project, User = ParseUser.CurrentUser };
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
	#endregion
}