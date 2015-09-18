using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Parse;

public class User : MonoBehaviour
{
	#region Constants
	public const string COMPANY_NAME = "roleName";
	public const string NAME = "name";
	#endregion

	#region Public Vars
	public ParseRole CurrentTeam;
	public static User CurrentUser;
	#endregion

	#region Unity Methods
	void Awake()
	{
		if(CurrentUser == null)
		{
			CurrentUser = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}
	#endregion

	#region Methods
	public void GetData()
	{
		StartCoroutine(GetRole());
	}
	void CreateTeam(string name)
	{
		StartCoroutine(CreateTeamCoroutine(name));
	}
	void TeamUpdated(ParseRole role)
	{
		CurrentTeam = role;
	}
	public bool NeedsSetup()
	{
		return (!ParseUser.CurrentUser.ContainsKey(NAME)) || (string.IsNullOrEmpty(Name));
	}
	#endregion

	#region Coroutines
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
			TeamUpdated(task.Result);

			if(NeedsSetup())
				NewUserAlert.Instance.Open();
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

	#region Properties
	public string CompanyName
	{
		get { return (string)ParseUser.CurrentUser[COMPANY_NAME]; }
		set { ParseUser.CurrentUser[COMPANY_NAME] = value; }
	}
	public string Name
	{
		get { return (string)ParseUser.CurrentUser[NAME]; }
		set { ParseUser.CurrentUser[NAME] = value; }
	}
	#endregion
}
