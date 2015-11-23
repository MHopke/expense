using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class ProjectListElement : VisualElement 
{
	#region Constants
	const string COUNT_SUFFIX = " Expenses";
	const string EDIT_WARNING_HEADER = "Warning";
	const string EDIT_WARNING_BODY = "This project is closed and " +
		"you cannot edit it. Please contact the Project Leader.";
	#endregion
	
	#region Public Vars
	//public Text Name;
	//public Text Description;
	public Text ItemCount;
	public Text ViewText;
	public Text NewText;
	public Text ReportText;

	public InputField Namefield;
	public InputField Descriptionfield;

	public Image Background;
	
	public Button ViewButton;
	public Button ReportButton;
	public Button NewButton;
	#endregion
	
	#region Private Vars
	string _previousName, _previousDescription;
	Project _project;
	#endregion
	
	#region Overidden Methods
	protected override void OnInit ()
	{
		base.OnInit ();
		ExpenseAlert.createdExpense += ExpenseCreated;
	}
	protected override void OnCleanUp ()
	{
		base.OnCleanUp ();
		ExpenseAlert.createdExpense -= ExpenseCreated;
	}
	public override void PresentVisuals (bool display)
	{
		base.PresentVisuals (display);

		if(Namefield)
		{
			Namefield.enabled = display;
			//Namefield.placeholder.enabled = display;
		}

		if(Descriptionfield)
		{
			Descriptionfield.enabled = display;
			//Descriptionfield.placeholder.enabled = display;
		}
		
		if(ItemCount)
			ItemCount.enabled = display;
		
		if(ViewText)
			ViewText.enabled = display;
		
		if(NewText)
			NewText.enabled = display;

		if(ReportText)
			ReportText.enabled = display;

		if(Background)
			Background.enabled = display;

		if(ViewButton)
		{
			ViewButton.enabled = display;
			
			if(ViewButton.image)
				ViewButton.image.enabled = display;
		}
		
		if(ReportButton)
		{
			ReportButton.enabled = display;
			
			if(ReportButton.image)
				ReportButton.image.enabled = display;
		}

		if(NewButton)
		{
			NewButton.enabled = display;
			
			if(NewButton.image)
				NewButton.image.enabled = display;
		}
	}
	protected override void Disabled ()
	{
		base.Disabled ();
		
		if(ViewButton)
			ViewButton.interactable = false;
		
		if(ReportButton)
			ReportButton.interactable = false;

		if(NewButton)
			NewButton.interactable = false;

		if(!_project.Closed)
		{
			if(Namefield)
				Namefield.interactable = false;

			if(Descriptionfield)
				Descriptionfield.interactable = false;
		}
	}
	protected override void Enabled ()
	{
		base.Enabled ();
		
		if(ViewButton)
			ViewButton.interactable = true;
		
		if(ReportButton)
			ReportButton.interactable = true;

		if(NewButton)
			NewButton.interactable = true;

		if(!_project.Closed)
		{
			if(Namefield)
				Namefield.interactable = true;
			
			if(Descriptionfield)
				Descriptionfield.interactable = true;
		}
	}
	#endregion
	
	#region UI Methods
	public void GenerateReport()
	{
		OveriddenNavigation.Navigation.PushViewOnToStack(ReportPage.Instance);
		ReportPage.Instance.Setup(_project);
		//ProjectAlert.Instance.Open(_project);
	}
	public void ViewProject()
	{
		OveriddenNavigation.Navigation.PushViewOnToStack(ProjectPage.Instance);
		ProjectPage.Instance.Setup(_project);
		//ClientPage.Instance.Setup(_project);
	}
	public void CreateExpense()
	{
		ExpenseAlert.Instance.Open(_project);
	}
	public void NameEdited(string name)
	{
		/*if(_project.Closed)
		{
			DefaultAlert.Present(EDIT_WARNING_HEADER,EDIT_WARNING_BODY);
			return;
		}*/
		_previousName = _project.Name;
		_project.Name = name;

		StartCoroutine(SaveChanges());
	}
	public void DescriptionEdited(string description)
	{
		/*if(_project.Closed)
		{
			DefaultAlert.Present(EDIT_WARNING_HEADER,EDIT_WARNING_BODY);
			return;
		}*/
		_previousDescription = _project.Description;
		_project.Description = description;

		StartCoroutine(SaveChanges());
	}
	#endregion
	
	#region Methods
	public void Setup(Project project)
	{
		_project = project;
		
		Namefield.text = project.Name;
		Descriptionfield.text = project.Description;

		if(_project.Closed)
		{
			Namefield.interactable = false;
			Descriptionfield.interactable = false;
		}

		SetProjectCount();
	}
	void SetProjectCount()
	{
		ItemCount.text = _project.ItemCount + COUNT_SUFFIX;
	}
	#endregion

	#region Coroutines
	IEnumerator SaveChanges()
	{
		LoadAlert.Instance.StartLoad("Saving Changes...",null,-1);
		Task save = _project.SaveAsync();

		while(!save.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(save.IsFaulted || save.Exception != null)
		{
			DefaultAlert.Present("Sorry","There was an error when updating the project. Please try again later");

			_project.Name = _previousName;
			_project.Description = _previousDescription;
		}
		else
		{
			_previousName = _project.Name;
			_previousDescription = _project.Description;
		}
	}
	#endregion

	#region Event Listeners
	void ExpenseCreated(Expense item)
	{
		if(item.Project == _project)
			SetProjectCount();
	}
	#endregion

}
