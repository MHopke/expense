using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class ProjectAlert : UIView 
{
	#region Events
	public static event System.Action<Project> projectCreated;
	#endregion

	#region Public Vars
	public ExtendedDropdown ClientDropdown;

	public static ProjectAlert Instance = null;
	#endregion

	#region Private Vars
	Project _project;
	#endregion

	#region Overidden Methods
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
	public void NameChanged(string name)
	{
		_project.Name = name;
	}
	public void DescriptionChanged(string description)
	{
		_project.Description = description;
	}
	public void ClientChanged(int selection)
	{
		_project.Client = Database.Instance.Clients[selection];
	}
	public void Submit()
	{
		StartCoroutine(CreateProject());
	}
	#endregion

	#region Methods
	public void Open(Client client)
	{
		_project = new Project() { ItemCount = 0, Client = client };
		_project.ACL = new ParseACL(ParseUser.CurrentUser);
		_project.ACL.SetRoleReadAccess(User.CurrentUser.CompanyName,true);
		//_project.ACL.SetRoleWriteAccess();
		_project.ProjectLeader = ParseUser.CurrentUser;

		int selection = -1;
		List<string> names = new List<string>();

		for(int index = 0; index < Database.Instance.Clients.Count; index++)
		{
			names.Add(Database.Instance.Clients[index].Name);

			if(selection < 0 && Database.Instance.Clients[index].Name == client.Name)
				selection = index;
		}

		ClientDropdown.Initialize(names,selection);

		UIAlertController.Instance.PresentAlert(this);
	}
	#endregion

	#region Coroutines
	IEnumerator CreateProject()
	{
		Task task = _project.SaveAsync();

		LoadAlert.Instance.StartLoad("Creating Project...",null,-1);

		while(!task.IsCompleted)
			yield return null;

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error:\n"+task.Exception);//handle the error
		}
		else
		{
			LoadAlert.Instance.SetText("Updating " + _project.Client.Name + "'s records.");
			_project.Client.ProjectCount++;

			Task save = _project.Client.SaveAsync();

			while(!save.IsCompleted)
				yield return null;

			LoadAlert.Instance.Done();
			
			Deactivate();

			Database.Instance.AddProject(_project);
			
			if(projectCreated != null)
				projectCreated(_project);

			_project = null;
			
			if(save.IsFaulted || save.Exception != null)
			{
				Debug.Log("error occured. UI may not be updated:\n"+task.Exception);
			}
		}
	}
	#endregion
}
