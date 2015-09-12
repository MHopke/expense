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

	#region Private Vars
	bool _signup = true;
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
		Task task = null;

		if(_signup)
		{
			task = ParseUser.LogInAsync(UsernameField.Text,PasswordField.Text);
			LoadAlert.Instance.StartLoad("Logging in...");
		}
		else
		{
			ParseUser user = new ParseUser()
			{
				Username = UsernameField.Text,
				Password = PasswordField.Text
			};

			task = user.SignUpAsync();

			LoadAlert.Instance.StartLoad("Creating user...");
		}

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

			if(!_signup)
				Deactivate();
			else
				StartCoroutine(GetRole());
		}
	}
	IEnumerator GetRole()
	{
		ParseQuery<ParseRole> query = new ParseQuery<ParseRole>();

		Task<ParseRole> task = query.FirstAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.IsFaulted || task.Exception != null)
		{
		}
		else
		{
			task.Result.Users.Add(ParseUser.CurrentUser);

			Task save = task.Result.SaveAsync();

			while(!task.IsCompleted)
				yield return null;

			if(task.IsFaulted || task.Exception != null)
			{
			}
			else
			{
				Debug.Log("updated company");

				Deactivate();
			}
		}
	}
	#endregion
}
