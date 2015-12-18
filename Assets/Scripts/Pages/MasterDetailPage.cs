using UnityEngine;
using System.Collections;

using gametheory.UI;

public class MasterDetailPage : UIView 
{
	#region Constants
	const string MENU_IN = "MenuIn";
	const string MENU_OUT = "MenuOut";
	#endregion

	#region Protected Vars
	bool _menuIn;
	#endregion

	#region UI Methods
	public void Menu()
	{
		if(_menuIn)
			Animator.SetTrigger(MENU_OUT);
		else
			Animator.SetTrigger(MENU_IN);

		_menuIn = !_menuIn;
	}
	#endregion
}
