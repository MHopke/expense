using UnityEngine;

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Parse;

public class Database : MonoBehaviour 
{
	#region Events
	public static event System.Action initialized;
	#endregion

	#region Public Vars
	public static Database Instance = null;
	#endregion

	#region Private Vars
	List<Client> _clients;
	List<Project> _projects;
	#endregion

	#region Unity Methods
	void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			DestroyObject(gameObject);
	}
	#endregion

	#region Methods
	public void Init()
	{
		_clients = new List<Client>();
		_projects = new List<Project>();

		StartCoroutine(GetClients());
	}

	public void AddProject(Project project)
	{
		_projects.Add(project);
	}

	public List<Project> GetProjects(Client client)
	{
		return new List<Project>(from p in _projects where p.Client.ObjectId == client.ObjectId select p);
	}

	public void AddClient(Client client)
	{
		_clients.Add(client);
	}
	#endregion

	#region Coroutines
	IEnumerator GetClients()
	{
		LoadAlert.Instance.StartLoad("Loading Clients...",null,-1);

		ParseQuery<Client> query = new ParseQuery<Client>();
		
		Task<IEnumerable<Client>> task = query.FindAsync();
		
		while(!task.IsCompleted)
			yield return null;
		
		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error while finding clients:\n" + task.Exception.ToString());
		}
		else
		{
			_clients = task.Result.ToList();
		}

		StartCoroutine(GetProjects());
	}
	IEnumerator GetProjects()
	{
		LoadAlert.Instance.SetText("Loading Projects...");

		ParseQuery<Project> query = new ParseQuery<Project>();
		
		Task<IEnumerable<Project>> task = query.FindAsync();
		
		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error while finding clients:\n" + task.Exception.ToString());
		}
		else
		{
			_projects = task.Result.ToList();
		}

		if(initialized != null)
			initialized();
	}
	#endregion

	#region Properties
	public List<Client> Clients
	{
		get { return _clients; }
	}
	public List<Project> Projects
	{
		get { return _projects; }
	}
	#endregion
}
