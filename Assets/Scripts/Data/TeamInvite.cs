
using Parse;

[ParseClassName("TeamInvite")]
public class TeamInvite : ParseObject 
{
	#region Constants
	const string EMAIL = "Email";
	const string TEAM = "Team";
	#endregion

	#region Constructors
	public TeamInvite(){}
	#endregion

	#region Properties
	public string Email
	{
		get { return GetProperty<string>(EMAIL); }
		set { SetProperty<string>(value,EMAIL); }
	}
	public string Team
	{
		get { return GetProperty<string>(TEAM); }
		set { SetProperty<string>(value,TEAM); }
	}
	#endregion
}
