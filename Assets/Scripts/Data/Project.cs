using UnityEngine;
using System.Collections;

using Parse;

using gametheory;

[ParseClassName("Project")]
public class Project : ParseObject, IBindingContext
{
	#region Events
	public event System.Action<object,string> propertyChanged;
	#endregion

	#region Constants
	const string NAME = "Name";
	const string DESCRIPTION = "Description";
	const string ITEM_COUNT = "ItemCount";
	const string CLIENT = "Client";
	const string PROJECT_LEADER = "ProjectLeader";
	const string CLOSED = "Closed";
	#endregion

	#region Constructors
	public Project(){}
	#endregion

	#region Methods
	public bool IsProjectLeader(ParseUser User)
	{
		return ProjectLeader.ObjectId == User.ObjectId;
	}
	#endregion

	#region Properties
	[ParseFieldName("closed")]
	public bool Closed
	{
		get { return GetProperty<bool>(CLOSED); }
		set { SetProperty<bool>(value,CLOSED); }
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

	[ParseFieldName("itemCount")]
	public int ItemCount
	{
		get { return GetProperty<int>(ITEM_COUNT); }
		set { SetProperty<int>(value,ITEM_COUNT); }
	}

	[ParseFieldName("client")]
	public Client Client
	{
		get { return GetProperty<Client>(CLIENT); }
		set { SetProperty<Client>(value,CLIENT); }
	}

	[ParseFieldName("projectLead")]
	public ParseUser ProjectLeader
	{
		get { return GetProperty<ParseUser>(PROJECT_LEADER); }
		set { SetProperty<ParseUser>(value,PROJECT_LEADER); }
	}
	#endregion
}
