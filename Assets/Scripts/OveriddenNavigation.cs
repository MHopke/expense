﻿using UnityEngine;
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
			Login.deactivatedEvent += LoggedIn;
			PresentUIView(Login);
		}
		else
		{
			//initialize the user here if Parse is already logged in
			User.Init();
			LoggedIn();
		}
	}
	#endregion

	#region Event Listeners
	void LoggedIn()
	{
		Login.deactivatedEvent -= LoggedIn;

		PresentUIView(_homeView);

		//_viewStack.Push(_homeView);
	}
	#endregion
}
