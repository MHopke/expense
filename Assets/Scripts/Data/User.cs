using UnityEngine;

using System.Collections;

using Parse;

public class User
{
	#region Constants
	const string COMPANY_NAME = "roleName";
	#endregion

	#region Public Vars
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
	#endregion
}
