using UnityEngine;
using System.Collections;

using Parse;

[ParseClassName("Client")]
public class Client : ParseObject 
{
	#region Constants
	const string NAME = "Name";
	const string PROJECT_COUNT = "ProjectCount";
	#endregion

	#region Constructors
	public Client(){}
	#endregion

	#region Properties
	[ParseFieldName("name")]
	public string Name
	{
		get { return GetProperty<string>(NAME); }
		set { SetProperty<string>(value,NAME); }
	}

	[ParseFieldName("projectCount")]
	public int ProjectCount
	{
		get { return GetProperty<int>(PROJECT_COUNT); }
		set { SetProperty<int>(value,PROJECT_COUNT); }
	}
	#endregion
}
