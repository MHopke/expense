using UnityEngine;

using System.Collections;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class LoginPage : UIView 
{
	#region Public Vars
	#endregion

	#region UI Methods
	public void Submit()
	{
		StartCoroutine(Login());
	}
	#endregion

	#region Coroutines
	IEnumerator Login()
	{
		Task<ParseUser> task = ParseUser.LogInAsync("anyafrans","momentum1");

		while(!task.IsCompleted)
			yield return null;

		/*ParseACL acl = new ParseACL();
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = true;
		
		ParseRole role = new ParseRole("Momentum Media PR",acl);

		role.Users.Add(ParseUser.CurrentUser);

		Task save = role.SaveAsync();

		while(!save.IsCompleted)
			yield return null;

		Debug.Log("saved");*/
	}
	#endregion
}
