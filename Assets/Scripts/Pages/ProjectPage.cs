using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using gametheory.UI;

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
	#endregion

	#region UI Methods
	public void CreateExpense()
	{
		ExpenseAlert.Instance.Open(_project);
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

			StartCoroutine(Database.Instance.GetExpenses(_project,ProcessExpenses));
		}
	}
	#endregion

	#region Methods
	void ProcessExpenses(IEnumerable<Expense> expenses)
	{
		//Debug.Log(expenses.co
		foreach(Expense expense in expenses)
			AddExpense(expense);
	}
	void AddExpense(Expense expense)
	{
		ExpenseListElement element = (ExpenseListElement)
			GameObject.Instantiate(ExpensePrefab,Vector3.zero,Quaternion.identity);

		element.Setup(expense,_project.Closed);

		ExpenseList.AddListElement(element);
	}
	#endregion
}
