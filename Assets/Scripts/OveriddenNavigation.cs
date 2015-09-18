using UnityEngine;
using System.Collections;

using gametheory.UI;

using Parse;

public class OveriddenNavigation : AppNavigationController 
{
	#region Public Vars
	public LoginPage Login;
	public HomePage Home;
	#endregion

	#region Overidden Methods
	protected override void OnActivate ()
	{
		_homeView = Home;

		base.OnActivate ();

		//ParseUser.LogOut();

		if(ParseUser.CurrentUser == null)
		{
			PresentLogin();
		}
		else
		{
			LoggedIn();
		}
	}
	#endregion

	#region Methods
	public void PopToLogin()
	{
		if(_viewStack.Count >= 1)
			BackButton.Remove();
		_viewStack.Clear();

		RemoveUIView(CurrentView);

		PresentLogin();
	}
	void PresentLogin()
	{
		Login.deactivatedEvent += LoggedIn;
		PresentUIView(Login);
	}
	#endregion

	#region Event Listeners
	void LoggedIn()
	{
		Login.deactivatedEvent -= LoggedIn;

		PresentUIView(_homeView);

		User.CurrentUser.GetData();
		//_viewStack.Push(_homeView);
	}
	#endregion
}
