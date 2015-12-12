using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class ProjectPage : UIView 
{
	#region Public Vars
	public Text Name;
	public Text Description;

	public UIList ExpenseList;

	public ExpenseListElement ExpensePrefab;
	
	public static ProjectPage Instance = null;
	#endregion

	#region Private Vars
	Project _project;
	#endregion

	#region Overidden Methods
	protected override void OnInit ()
	{
		ExpenseAlert.createdExpense += AddExpense;
		base.OnInit ();
		
		Instance = this;
	}
	protected override void OnCleanUp ()
	{
		Instance = null;
		ExpenseAlert.createdExpense -= AddExpense;
		base.OnCleanUp ();
	}
	protected override void OnActivate ()
	{
		ExpenseListElement.remove += ExpenseRemoved;
		ExpenseListElement.switchedProject += ExpenseSwitchProject;
		base.OnActivate ();

		for(int index = 0; index < ExpenseList.ListItems.Count; index++)
		{
			(ExpenseList.ListItems[index] as ExpenseListElement).SetEditable(_project.Closed);
		}
	}
	protected override void OnDeactivate ()
	{
		ExpenseListElement.remove -= ExpenseRemoved;
		ExpenseListElement.switchedProject -= ExpenseSwitchProject;
		base.OnDeactivate ();
	}
	#endregion

	#region UI Methods
	public void CreateExpense()
	{
		ExpenseAlert.Instance.Open(_project);
	}
	public void InviteUser()
	{
		SingleInputAlert.Instance.Present("Add User","Add a user to this project.",
			"Enter email...",UserEntered,null,true);
		//_project.ACL.SetReadAccess(
	}
	#endregion

	#region Methods
	public void Setup(Project project)
	{
		if(_project == null || _project.ObjectId != project.ObjectId)
		{
			ExpenseList.ClearElements();
			_project = project;

			Name.text = project.Name;
			Description.text = project.Description;

			StartCoroutine(ProcessExpenses());
		}
	}
	#endregion

	#region Coroutines
	IEnumerator ProcessExpenses()
	{
		yield return StartCoroutine(Database.Instance.GetExpenses(_project));

		for(int index =0; index < Database.Instance.Expenses.Count; index++)
		{
			AddExpense(Database.Instance.Expenses[index]);
			yield return null;
		}
	}
	IEnumerator SwitchCoroutine(ExpenseListElement element, Project prevProject)
	{
		ExpenseList.RemoveListElement(element);

		prevProject.ItemCount--;
		element.Item.Project.ItemCount++;

		List<Project> projects = new List<Project>();
		projects.Add(prevProject);
		projects.Add(element.Item.Project);

		Task task = projects.SaveAllAsync();

		while(!task.IsCompleted)
			yield return null;

		if(task.Exception != null)
		{
			DefaultAlert.Present("Sorry!","An error occured and we " +
				"could not update the projects' item count");
		}
	}
	IEnumerator AddUserCoroutine(string email)
	{
		LoadAlert.Instance.StartLoad("Adding " + email,null,-1);

		ParseQuery<ParseUser> userQuery = new ParseQuery<ParseUser>().WhereEqualTo("email",email);
		Task<ParseUser> fetch = userQuery.FirstAsync();

		while(!fetch.IsCompleted)
			yield return null;

		if(fetch.Exception != null)
		{
			LoadAlert.Instance.Done();
			DefaultAlert.Present("Sorry!","We could not find that user. " +
				"Make sure you spelled their email correctly.");
		}
		else
		{
			_project.ACL.SetReadAccess(fetch.Result,true);
			_project.ACL.SetWriteAccess(fetch.Result,true);

			Task save = _project.SaveAsync();

			while(!save.IsCompleted)
				yield return null;

			LoadAlert.Instance.Done();

			if(save.Exception != null)
			{
				DefaultAlert.Present("Sorry!","The database failed " +
					"to add that user. Please try again later.");
			}
		}
		//_project.ACL.SetReadAccess(Data
	}
	#endregion

	#region Event Listeners
	void AddExpense(Expense expense)
	{
		ExpenseListElement element = (ExpenseListElement)
			GameObject.Instantiate(ExpensePrefab,Vector3.zero,Quaternion.identity);

		element.Setup(expense);
		element.SetEditable(_project.Closed);

		ExpenseList.AddListElement(element);
	}
	void ExpenseRemoved(ExpenseListElement element)
	{
		ExpenseList.RemoveListElement(element);
	}
	void ExpenseSwitchProject(ExpenseListElement element, Project prevProject)
	{
		StartCoroutine(SwitchCoroutine(element,prevProject));
	}
	void UserEntered(string email)
	{
		StartCoroutine(AddUserCoroutine(email));
	}
	#endregion
}
