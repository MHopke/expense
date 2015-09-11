using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using gametheory.UI;

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
		ProjectList.ClearElements();
		ProjectAlert.projectCreated += AddProject;
		base.Activate();
	}
	protected override void OnDeactivate ()
	{
		ProjectAlert.projectCreated -= AddProject;
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
}
