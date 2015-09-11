using UnityEngine;
using System.Collections;

using Parse;

[ParseClassName("Project")]
public class Project : ParseObject 
{
	#region Constants
	const string NAME = "Name";
	const string DESCRIPTION = "Description";
	const string COMPANY = "Company";
	#endregion

	#region Constructors
	public Project(){}
	#endregion

	#region Properties
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

	[ParseFieldName("company")]
	public Client Company
	{
		get { return GetProperty<Client>(COMPANY); }
		set { SetProperty<Client>(value,COMPANY); }
	}
	#endregion
}
