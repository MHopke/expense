using UnityEngine;
using System.Collections;

using Parse;

using gametheory.UI;

public class HomePage : UIView 
{
	#region Methods
	public void CreateCompany()
	{
		ParseACL acl = new ParseACL();
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = true;

		ParseRole role = new ParseRole("Momentum Media PR",acl);
	}
	#endregion
}
