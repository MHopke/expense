using UnityEngine;

using System.Collections;
using System.Collections.Generic;

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
	const string USER_COUNT = "UserCount";
	const string CLIENT = "Client";
	const string PROJECT_LEADER = "ProjectLeader";
	const string CLOSED = "Closed";
	const string SUBMITTED_USERS = "SubmittedUsers";
	const string INVITED_USERS = "InvitedUsers";
	#endregion

	#region Constructors
	public Project(){}
	#endregion

	#region Methods
	public bool IsProjectLeader(ParseUser User)
	{
		return ProjectLeader.ObjectId == User.ObjectId;
	}
	public bool CanClose()
	{
		return IsProjectLeader(ParseUser.CurrentUser) 
			&& SubmittedUsers.Count == InvitedUsers.Count;
	}
	#endregion

	#region Properties
	[ParseFieldName("closed")]
	public bool Closed
	{
		get { return GetProperty<bool>(CLOSED); }
		set { SetProperty<bool>(value,CLOSED); }
	}

	public bool CanGenerateReport
	{
		get { return UserCount == SubmittedUsers.Count; }
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

	[ParseFieldName("userCount")]
	public int UserCount
	{
		get { return GetProperty<int>(USER_COUNT); }
		set { SetProperty<int>(value,USER_COUNT); }
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

	[ParseFieldName("invitedUsers")]
	public IList<string> InvitedUsers
	{
		get { return GetProperty<IList<string>>(INVITED_USERS); }
		set { SetProperty<IList<string>>(value,INVITED_USERS); }
	}

	[ParseFieldName("submittedUsers")]
	public IList<string> SubmittedUsers
	{
		get { return GetProperty<IList<string>>(SUBMITTED_USERS); }
		set { SetProperty<IList<string>>(value,SUBMITTED_USERS); }
	}
	#endregion
}
