using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

using gametheory.UI;

using Parse;

public class ClientPage : UIView 
{
	#region Public Vars
	public Text Name;

	public UIList ProjectList;

	public ProjectListElement ProjectPrefab;

	public static ClientPage Instance = null;
	#endregion

	#region Private Vars
	Client _client;
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
	protected override void OnActivate ()
	{
		ProjectAlert.projectCreated += AddProject;
		ProjectListElement.switchedClient += ProjectSwitchedClients;
		ProjectListElement.deleteProject += DeleteProject;

		Database.Instance.ClearExpenses();

		base.OnActivate();
	}
	protected override void OnDeactivate ()
	{
		ProjectAlert.projectCreated -= AddProject;
		ProjectListElement.switchedClient -= ProjectSwitchedClients;
		ProjectListElement.deleteProject -= DeleteProject;
		base.OnDeactivate ();
	}
	#endregion

	#region UI Methods
	public void CreateProject()
	{
		ProjectAlert.Instance.Open(_client);
	}
	#endregion

	#region Methods
	public void Setup(Client client)
	{
		ProjectList.ClearElements();

		_client = client;

		Name.text = client.Name;

		List<Project> projects = Database.Instance.GetProjects(client);
		for(int index =0; index < projects.Count; index++)
		{
			AddProject(projects[index]);
		}
	}
	void AddProject(Project project)
	{
		ProjectListElement element = (ProjectListElement)
			GameObject.Instantiate(ProjectPrefab,Vector3.zero,Quaternion.identity);

		element.Setup(project);

		ProjectList.AddListElement(element);
	}
	#endregion

	#region Coroutines
	IEnumerator UpdateClients(ProjectListElement element, Client previous)
	{
		LoadAlert.Instance.StartLoad("Updating Clients...",null,-1);
		
		List<Client> updates = new List<Client>();
		
		previous.ProjectCount--;
		element.Project.Client.ProjectCount++;
		
		updates.Add(previous);
		updates.Add(element.Project.Client);
		
		Task updateClient = updates.SaveAllAsync();
		
		while(!updateClient.IsCompleted)
			yield return null;
		
		LoadAlert.Instance.Done();
		
		if(updateClient.Exception == null)
			ProjectList.RemoveListElement(element);
	}
	IEnumerator DeleteCoroutine(ProjectListElement element)
	{
		LoadAlert.Instance.StartLoad("Removing Project...",null,-1);

		Client client = element.Project.Client;

		Database.Instance.RemoveProject(element.Project);

		Task task = element.Project.DeleteAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.Exception != null)
		{
			LoadAlert.Instance.Done();
			//display alert here?
		}
		else
		{
			ProjectList.RemoveListElement(element);

			client.ProjectCount--;

			task = client.SaveAsync();

			while(!task.IsCompleted)
				yield return null;
			
			LoadAlert.Instance.Done();
		}
	}
	#endregion

	#region Event Listeners
	void ProjectSwitchedClients(ProjectListElement element, Client previous)
	{
		StartCoroutine(UpdateClients(element,previous));
	}
	void DeleteProject(ProjectListElement element)
	{
		StartCoroutine(DeleteCoroutine(element));
	}
	#endregion
}
