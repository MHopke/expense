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
	public Client CurrentClient;
	public Project CurrentProject;
	public static Database Instance = null;
	#endregion

	#region Private Vars
	List<Client> _clients;
	List<Project> _projects;
	List<Expense> _expenseList;
	#endregion

	#region Unity Methods
	void Awake()
	{
		if(Instance == null)
		{
			User.dataRefreshed += Init;

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			DestroyObject(gameObject);
	}
	void OnDestroy()
	{
		User.dataRefreshed -= Init;
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
	public void RemoveProject(Project project)
	{
		_projects.Remove(project);
	}
	public List<Project> GetProjects(Client client)
	{
		return new List<Project>(from p in _projects where p.Client.ObjectId == client.ObjectId select p);
	}

	public void AddClient(Client client)
	{
		_clients.Add(client);
	}
	public void RemoveClient(Client client)
	{
		_clients.Remove(client);
	}
	public int GetClientIndex(Client client)
	{
		for(int index =0; index < _clients.Count; index++)
		{
			if(_clients[index].ObjectId == client.ObjectId)
				return index;
		}

		return -1;
	}
	public void ClearExpenses()
	{
		if(_expenseList != null && _expenseList.Count > 0)
			_expenseList.Clear();
	}
	public int GetProjectIndex(Project project)
	{
		for(int index = 0; index < _projects.Count; index++)
		{
			if(_projects[index].ObjectId == project.ObjectId)
			   return index;
		}

		return -1;
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

			int sub = 0;
			Client client = null;
			Project project = null;
			for(int index = 0; index < _projects.Count; index++)
			{
				project = _projects[index];
				for(sub = 0; sub < _clients.Count; sub++)
				{
					client = _clients[sub];
					if(project.Client.ObjectId == client.ObjectId)
					{
						project.Client = client;
						break;
					}
				}
			}
		}

		if(initialized != null)
			initialized();
	}
	public IEnumerator GetExpenses(Project project)
	{
		LoadAlert.Instance.StartLoad("Getting Expenses...",null,-1);

		ParseQuery<Expense> query = new ParseQuery<Expense>().WhereEqualTo("project",project).Include("user");

		query = query.Limit(200);

		Task<IEnumerable<Expense>> task = query.FindAsync();

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted || task.Exception != null)
		{
			Debug.Log("error getting expenses:\n" + task.Exception.ToString());
		}
		else
			_expenseList = task.Result.ToList();

		int sub =0;
		Project proj = null;
		Expense exp = null;
		for(int index =0; index < _projects.Count; index++)
		{
			proj = _projects[index];

			for(sub =0; sub < _expenseList.Count; sub++)
			{
				exp = _expenseList[sub];

				if(exp.Project.ObjectId == proj.ObjectId)
				{
					exp.Project = proj;
					break;
				}
			}
		}
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
	public List<Expense> Expenses
	{
		get { return _expenseList; }
	}
	#endregion
}
