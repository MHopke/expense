using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class LoginPage : UIView 
{
	#region Constants
	const string LOGIN = "Log In";
	const string CREATE = "Create User";
	Color OFF = new Color(0.8f,0.8f,0.8f,0.8f);
	#endregion

	#region Public Vars
	public Text Title;

	public ExtendedInputField NameField;
	public ExtendedInputField UsernameField;
	public ExtendedInputField PasswordField;

	public ExtendedButton LoginToggle;
	public ExtendedButton CreateToggle;
	#endregion

	#region Private Vars
	bool _signup;
	#endregion

	#region UI Methods
	public void ToggleLogin()
	{
		Title.text = LOGIN;
		CreateToggle.Button.image.color = OFF;
		LoginToggle.Button.image.color = Color.white;
		LoginToggle.Disable();
		CreateToggle.Enable();
		NameField.transform.parent.gameObject.SetActive(false);
		_signup = false;
	}
	public void ToggleCreate()
	{
		Title.text = CREATE;
		CreateToggle.Button.image.color = Color.white;
		CreateToggle.Disable();
		LoginToggle.Enable();
		LoginToggle.Button.image.color = OFF;
		NameField.transform.parent.gameObject.SetActive(true);
		_signup = true;
	}

	public void Submit()
	{
		StartCoroutine(Login());
	}
	#endregion
	
	#region Coroutines
	IEnumerator Login()
	{
		Task task = null;

		if(!_signup)
		{
			task = ParseUser.LogInAsync(UsernameField.Text,PasswordField.Text);
			LoadAlert.Instance.StartLoad("Logging in...");
		}
		else
		{
			string username = UsernameField.Text.ToLower();
			ParseUser user = new ParseUser()
			{
				Username = username,
				Email = username,
				Password = PasswordField.Text
			};
			user[User.NAME] = NameField.Text;

			task = user.SignUpAsync();

			LoadAlert.Instance.StartLoad("Creating user...");
		}

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error:\n" + task.Exception.ToString());
		}
		else
		{
			NameField.Text = PasswordField.Text = UsernameField.Text = "";

			Deactivate();
		}
	}
	#endregion
}
