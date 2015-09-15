using UnityEngine;

using System.Collections;

using Parse;

public class User
{
	#region Constants
	public const string COMPANY_NAME = "roleName";
	public const string NAME = "name";
	#endregion

	#region Public Vars
	public ParseRole CurrentTeam;
	public static User CurrentUser;
	#endregion

	#region Methods
	public static void Init()
	{
		CurrentUser = new User();
	}
	#endregion

	#region Properties
	public string CompanyName
	{
		get { return (string)ParseUser.CurrentUser[COMPANY_NAME]; }
		set { ParseUser.CurrentUser[COMPANY_NAME] = value; }
	}
	public string Name
	{
		get { return (string)ParseUser.CurrentUser[NAME]; }
		set { ParseUser.CurrentUser[NAME] = value; }
	}
	#endregion
}
