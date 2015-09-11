using UnityEngine;

using System.Collections;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class LoginPage : UIView 
{
	#region Public Vars
	public ExtendedInputField UsernameField;
	public ExtendedInputField PasswordField;
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
		Task<ParseUser> task = ParseUser.LogInAsync(UsernameField.Text,PasswordField.Text);

		LoadAlert.Instance.StartLoad("Logging in...");

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error");
		}
		else
		{
			PasswordField.Text = UsernameField.Text = "";

			Deactivate();
		}
	}
	#endregion
}
