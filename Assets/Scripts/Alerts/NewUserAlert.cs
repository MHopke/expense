using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class NewUserAlert : UIView 
{
	#region Public Vars
	public InputField NameField;
	public InputField PasswordField;
	public InputField ConfirmField;

	public static NewUserAlert Instance = null;
	#endregion

	#region Overriden Methods
	protected override void OnInit ()
	{
		base.OnInit ();
		Instance = this;
	}
	protected override void OnCleanUp ()
	{
		Instance = null;
		base.OnCleanUp ();
	}
	#endregion

	#region UI Methods
	public void Submit()
	{
		if(PasswordField.text == ConfirmField.text)
			StartCoroutine(UpdateUser());
		else
			DefaultAlert.Present("Sorry!","You password fields do not match. Please try again.");
	}
	#endregion

	#region Methods
	public void Open()
	{
		UIAlertController.Instance.PresentAlert(this);
	}
	#endregion

	#region Coroutines
	IEnumerator UpdateUser()
	{
		LoadAlert.Instance.StartLoad("Updating User Account...",null,-1);

		ParseUser.CurrentUser.Password = PasswordField.text;
		User.CurrentUser.Name = NameField.text;

		Task task = ParseUser.CurrentUser.SaveAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.IsFaulted)
		{
			LoadAlert.Instance.Done();
			Debug.Log("couldn't update information:\n" + task.Exception.ToString());
		}
		else
		{
			string email = ParseUser.CurrentUser.Email;
			Task logout = ParseUser.LogOutAsync();

			while(!logout.IsCompleted)
				yield return null;

			if(logout.IsFaulted)
			{
			}
			else
			{
				Task login = ParseUser.LogInAsync(email,PasswordField.text);

				while(!login.IsCompleted)
					yield return null;

				if(login.IsFaulted)
				{
				}
				else
				{
					LoadAlert.Instance.Done();
					PasswordField.text = ConfirmField.text = NameField.text = "";
					Deactivate();
				}
			}
		}
	}
	#endregion
}
