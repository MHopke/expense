using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Parse;

using gametheory.UI;

public class HomePage : UIView 
{
	#region Constants
	const string FROM_EMAIL = "gametheoryco@gmail.com";
	const string MENU_IN = "MenuIn";
	const string MENU_OUT = "MenuOut";

	const int PASSWORD_LENGTH = 8;
	#endregion

	#region Public Vars
	public Text CompanyName;

	public UIList ClientList;
	public ClientPage ClientPage;

	public ClientListElement ClientPrefab;
	#endregion

	#region Private Vars
	bool _pulledData, _menuIn;
	#endregion

	#region Overidden Methods
	protected override void OnInit ()
	{
		base.OnInit ();
		Database.initialized += DatabaseInitialized;
		ClientAlert.createdClient += AddClient;
	}
	protected override void OnCleanUp ()
	{
		base.OnCleanUp ();
		Database.initialized -= DatabaseInitialized;
		ClientAlert.createdClient -= AddClient;
	}
	#endregion

	#region UI Methods
	public void CreateClient()
	{
		ClientAlert.Instance.Open();
	}
	public void Logout()
	{
		StartCoroutine(LogoutCoroutine());
	}
	public void InviteTeammate()
	{
		SingleInputAlert.Instance.Present("Invite Teammate","Enter the " +
			"email of the person you'd like to invite.","",EmailEntered,null,true);
	}
	public void Menu()
	{
		if(_menuIn)
			Animator.SetTrigger(MENU_OUT);
		else
			Animator.SetTrigger(MENU_IN);

		_menuIn = !_menuIn;
	}
	#endregion

	#region Methods
	public void CreateCompany()
	{
		/*ParseACL acl = new ParseACL();
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = true;

		ParseRole role = new ParseRole("Momentum Media PR",acl);*/
	}
	string GenerateTempPassword()
	{
		return System.Guid.NewGuid().ToString().Substring(0,PASSWORD_LENGTH);
	}
	#endregion

	#region Coroutines
	IEnumerator CreateTeammate(string email)
	{
		string pass = GenerateTempPassword();

		IDictionary<string,object> userDict = new Dictionary<string, object>();
		userDict.Add("email",email);
		userDict.Add("password",pass);
		userDict.Add("team",User.CurrentUser.CompanyName);
		Task<IDictionary<string,object>> inviteTask = ParseCloud.CallFunctionAsync<IDictionary<string,object>>("inviteUser",userDict);

		while(!inviteTask.IsCompleted)
			yield return null;

		if(inviteTask.IsFaulted)
		{
			LoadAlert.Instance.Done();
			Debug.Log("creating teammate failed:\n" + inviteTask.Exception.ToString());
		}
		else
		{
			StartCoroutine(GetNewUser(email,pass));
		}
	}
	IEnumerator GetNewUser(string email, string pass)
	{
		ParseQuery<ParseUser> query = new ParseQuery<ParseUser>().WhereEqualTo("username",email);

		Task<ParseUser> task = query.FirstAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.IsFaulted)
		{
			Debug.Log("couldn't find user:\n" + task.Exception.ToString());
		}
		else
		{
			User.CurrentUser.CurrentTeam.Users.Add(task.Result);
			User.CurrentUser.CurrentTeam.ACL.SetReadAccess(task.Result,true);
			User.CurrentUser.CurrentTeam.ACL.SetWriteAccess(task.Result,true);

			Task save = User.CurrentUser.CurrentTeam.SaveAsync();

			while(!save.IsCompleted)
				yield return null;

			if(task.IsFaulted)
			{
				Debug.Log("failed to update role:\n" + task.Exception.ToString());
			}
			else
				StartCoroutine(SendEmail(email,pass));
		}
	}
	IEnumerator GetUser(string email)
	{
		ParseQuery<ParseUser> query = new ParseQuery<ParseUser>().WhereEqualTo("username",email);
		
		Task<ParseUser> task = query.FirstAsync();
		
		while(!task.IsCompleted)
			yield return null;
		
		if(task.IsFaulted)
		{
			Debug.Log("couldn't find user:\n" + task.Exception.ToString());
			StartCoroutine(CreateTeammate(email));
		}
		else
		{
			User.CurrentUser.CurrentTeam.Users.Add(task.Result);
			User.CurrentUser.CurrentTeam.ACL.SetReadAccess(task.Result,true);
			User.CurrentUser.CurrentTeam.ACL.SetWriteAccess(task.Result,true);
			
			Task save = User.CurrentUser.CurrentTeam.SaveAsync();
			
			while(!save.IsCompleted)
				yield return null;
			
			if(task.IsFaulted)
			{
				Debug.Log("failed to update role:\n" + task.Exception.ToString());
			}
			else
				StartCoroutine(SendEmail(email,""));
		}
	}
	IEnumerator SendEmail(string email,string pass)
	{
		IDictionary<string,object> dict = new Dictionary<string, object>();
		dict.Add("toEmail",email);
		dict.Add("fromEmail",FROM_EMAIL);

		string body = "You've been invited to join " + User.CurrentUser.CompanyName + ".\n" +
			"Please download the Expense.It app, ";

		if(!string.IsNullOrEmpty(pass))
		{
			body += "and use the following temporary credentials:\n" +
					"Account: " + email + "\nPassword: " + pass;
		}
		else
			body += "and login to your user account.";

		dict.Add("text",body);
		dict.Add("subject","Expense.It - You've been invited to join " + User.CurrentUser.CompanyName);
		
		Task cloudTask = ParseCloud.CallFunctionAsync<IDictionary<string,object>>("sendMail",dict);
		
		while(!cloudTask.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(cloudTask.IsFaulted)
		{
			Debug.Log("sending invite email failed:\n" + cloudTask.Exception.ToString());
		}
		else
		{
			Debug.Log("email sent!");
		}
	}
	IEnumerator LogoutCoroutine()
	{
		LoadAlert.Instance.StartLoad("Logging out...",null,-1);
		Task task = ParseUser.LogOutAsync();

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted)
		{
			Debug.Log("failed to logout:\n" + task.Exception.ToString());
		}
		else
		{
			ClientList.ClearElements();
			(OveriddenNavigation.Instance as OveriddenNavigation).PopToLogin();
		}
	}
	#endregion

	#region Event Listeners
	void DatabaseInitialized()
	{
		CompanyName.text = User.CurrentUser.CompanyName;

		for(int index = 0; index < Database.Instance.Clients.Count; index++)//foreach(Client client in task.Result)
		{
			AddClient(Database.Instance.Clients[index]);
		}
	}
	void AddClient(Client client)
	{
		ClientListElement element = (ClientListElement)
			GameObject.Instantiate(ClientPrefab,Vector3.zero,Quaternion.identity);
		
		element.Setup(client);
		
		ClientList.AddListElement(element);
	}
	void EmailEntered(string email)
	{
		LoadAlert.Instance.StartLoad("Inviting Teammate...",null,-1);
		StartCoroutine(GetUser(email));
		//StartCoroutine(CreateTeammate(email.ToLower()));
	}
	//void ClientsUpdate(Clien
	#endregion
}
