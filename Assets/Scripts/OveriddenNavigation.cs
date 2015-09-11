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

		if(ParseUser.CurrentUser == null)
		{
			CurrentView = Login;
		}
		else
		{
			CurrentView = _homeView;
		}

		PresentUIView(CurrentView);
	}
	#endregion
}
