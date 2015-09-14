using UnityEngine;

using System;
using System.Collections.Generic;

using Parse;

[ParseClassName("Expense")]
public class Expense : ParseObject 
{
	#region Constants
	const string NAME = "Name";
	const string DESCRIPTION = "Description";
	const string DATE = "Date";
	const string VALUE = "Value";
	public const string TAGS = "Tags";
	const string IMAGE = "Image";
	const string BILLABLE = "Billable";
	const string REIMBURSEMENT = "Reimbursement";
	public const string PROJECT = "Project";
	const string USER = "User";
	#endregion

	#region Constructors
	public Expense(){}
	#endregion

	#region Properties
	[ParseFieldName("billable")]
	public bool Billable
	{
		get { return GetProperty<bool>(BILLABLE); }
		set { SetProperty<bool>(value, BILLABLE); }
	}

	[ParseFieldName("reimbursement")]
	public bool Reimbursement
	{
		get { return GetProperty<bool>(REIMBURSEMENT); }
		set { SetProperty<bool>(value, REIMBURSEMENT); }
	}

	[ParseFieldName("name")]
	public string Name
	{
		get { return GetProperty<string>(NAME); }
		set { SetProperty<string>(value,NAME); }
	}

	[ParseFieldName("description")]
	public string Description
	{
		get { return GetProperty<string>(DESCRIPTION); }
		set { SetProperty<string>(value,DESCRIPTION); }
	}

	[ParseFieldName("value")]
	public double Value
	{
		get { return GetProperty<double>(VALUE); }
		set { SetProperty<double>(value,VALUE); }
	}

	[ParseFieldName("date")]
	public DateTime Date
	{
		get { return GetProperty<DateTime>(DATE); }
		set { SetProperty<DateTime>(value,DATE); }
	}

	[ParseFieldName("tags")]
	public IList<string> Tags
	{
		get { return GetProperty<IList<string>>(TAGS); }
		set { SetProperty<IList<string>>(value,TAGS); }
	}

	[ParseFieldName("image")]
	public ParseFile Image
	{
		get { return GetProperty<ParseFile>(IMAGE); }
		set { SetProperty<ParseFile>(value,IMAGE); }
	}

	[ParseFieldName("project")]
	public Project Project
	{
		get { return GetProperty<Project>(PROJECT); }
		set { SetProperty<Project>(value,PROJECT); }
	}

	[ParseFieldName("user")]
	public ParseUser User
	{
		get { return GetProperty<ParseUser>(USER); }
		set { SetProperty<ParseUser>(value, USER); }
	}
	#endregion
}
