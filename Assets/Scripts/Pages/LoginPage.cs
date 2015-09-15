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

	#region Methods
	void CreateTeam(string name)
	{
		StartCoroutine(CreateTeamCoroutine(name));
	}
	void TeamUpdated(ParseRole role)
	{
		User.CurrentUser.CurrentTeam = role;
		
		Debug.Log("updated company");
		
		Deactivate();
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
			User.Init();

			NameField.Text = PasswordField.Text = UsernameField.Text = "";

			StartCoroutine(GetRole());
		}
	}
	IEnumerator GetRole()
	{
		LoadAlert.Instance.StartLoad("Checking for existing team...",null,-1);
		ParseQuery<ParseRole> query = new ParseQuery<ParseRole>();

		Task<ParseRole> task = query.FirstAsync();

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted || task.Exception != null)
		{
			using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator()) 
			{
				if (enumerator.MoveNext()) {
					ParseException error = (ParseException) enumerator.Current;
					// error.Message will contain an error message
					// error.Code will return "OtherCause"

					if(error.Code == ParseException.ErrorCode.ObjectNotFound)
						SingleInputAlert.Instance.Present("Create Team","It looks like your not on " +
							"a team, please take the time to make one.","",CreateTeam,null);
				}
			}
		}
		else
		{
			Task saveRole = null;
			if(task.Result.Count() > 0)
			{
				//if(task.Result.Users.

				task.Result.Users.Add(ParseUser.CurrentUser);

				saveRole = task.Result.SaveAsync();

				while(!saveRole.IsCompleted)
					yield return null;
				
				if(saveRole.IsFaulted || saveRole.Exception != null)
				{
					Debug.Log("error");
				}
				else
				{
					TeamUpdated(task.Result);
				}
			}
		}
	}
	IEnumerator CreateTeamCoroutine(string name)
	{
		LoadAlert.Instance.StartLoad("Creating " + name + "...",null,-1);

		ParseACL acl = new ParseACL(ParseUser.CurrentUser);
		
		ParseRole role = new ParseRole(name,acl);
		role.Users.Add(ParseUser.CurrentUser);

		User.CurrentUser.CompanyName = name;

		Task task = role.SaveAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error:\n" + task.Exception.ToString());
		}
		else
		{
			task = ParseUser.CurrentUser.SaveAsync();

			while(!task.IsCompleted)
				yield return null;

			if(task.IsFaulted || task.Exception != null)
			{
				Debug.Log("error:\n" + task.Exception.ToString());
			}
			else
			{
				TeamUpdated(role);
			}
		}
	}
	#endregion
}
