using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using gametheory.UI;

public class ReportPage : UIView 
{
	#region Contants
	const string DOLLAR = "$";
	#endregion

	#region Public Vars
	public Text ProjectName;
	public Text GrandTotal;
	public Text BillableTotal;
	public Text ReimbursementTotal;

	public UIList List;

	public ReportItem Prefab;

	public static ReportPage Instance = null;
	#endregion

	#region Private Vars
	double _grandTotal, _billableTotal, _reimbursementTotal;
	#endregion

	#region Overriden Methods
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
	public void SaveToCSV()
	{

	}
	#endregion

	#region Methods
	public void Setup(Project project)
	{
		if(ProjectName.text != project.Name)
		{
			List.ClearElements();

			ProjectName.text = project.Name;

			_grandTotal = _billableTotal = _reimbursementTotal = 0.0;

			StartCoroutine(Database.Instance.GetExpenses(project,ProcessExpenses));
		}
	}

	void ProcessExpenses(IEnumerable<Expense> expenses)
	{
		//Debug.Log(expenses.co
		foreach(Expense expense in expenses)
			AddExpense(expense);


		GrandTotal.text = DOLLAR + _grandTotal;
		BillableTotal.text = DOLLAR + _billableTotal;
		ReimbursementTotal.text = DOLLAR + _reimbursementTotal;
	}
	void AddExpense(Expense expense)
	{
		ReportItem element = (ReportItem)
			GameObject.Instantiate(Prefab,Vector3.zero,Quaternion.identity);
		
		element.Setup(expense);

		_grandTotal += expense.Value;

		if(expense.Billable)
			_billableTotal += expense.Value;

		if(expense.Reimbursement)
			_reimbursementTotal += expense.Value;
		
		List.AddListElement(element);
	}
	#endregion
}
