using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
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

	Project _project;
	List<Expense> _expenses;
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
		UniFileBrowser.use.OpenFolderWindow(false,FolderSelected);
	}
	#endregion

	#region Methods
	public void Setup(Project project)
	{
		if(ProjectName.text != project.Name)
		{
			_project = project;

			List.ClearElements();

			ProjectName.text = project.Name;

			_grandTotal = _billableTotal = _reimbursementTotal = 0.0;

			StartCoroutine(ProcessExpenses());
		}
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

	#region Coroutines
	IEnumerator ProcessExpenses()
	{
		_expenses = Database.Instance.Expenses;

		if(_expenses.Count == 0)
			yield return StartCoroutine(Database.Instance.GetExpenses(_project));

		for(int index = 0; index < _expenses.Count; index++)
		{
			AddExpense(_expenses[index]);
			yield return null;
		}
		
		GrandTotal.text = DOLLAR + _grandTotal;
		BillableTotal.text = DOLLAR + _billableTotal;
		ReimbursementTotal.text = DOLLAR + _reimbursementTotal;
	}
	#endregion

	#region EventListeners
	void FolderSelected(string path)
	{
		string fullPath = Path.Combine(path,_project.Name + ".csv");

		FileStream file = new FileStream(fullPath,FileMode.Create);

		using(StreamWriter writer = new StreamWriter(file))
		{
			//write the headers
			writer.WriteLine("ObjectId,Expense Item,Value");

			Expense expense = null;
			for(int index = 0; index < _expenses.Count; index++)
			{
				expense = _expenses[index];
				writer.WriteLine(expense.ObjectId+","+expense.Name+","+expense.Value);
			}

			writer.WriteLine("Grand Total,," + _grandTotal);
			writer.WriteLine("Billable Total,," + _billableTotal);
			writer.WriteLine("Reimbursement Total,," + _reimbursementTotal);
		}
	}
	#endregion
}
