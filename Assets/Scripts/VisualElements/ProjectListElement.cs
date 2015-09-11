using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using gametheory.UI;

public class ProjectListElement : VisualElement 
{
	#region Constants
	const string COUNT_SUFFIX = " Expenses";
	#endregion
	
	#region Public Vars
	public Text Name;
	public Text Description;
	public Text ItemCount;
	public Text ViewText;
	public Text NewText;
	public Text ReportText;
	
	public Button ViewButton;
	public Button ReportButton;
	public Button NewButton;
	#endregion
	
	#region Private Vars
	Project _project;
	#endregion
	
	#region Overidden Methods
	protected override void OnInit ()
	{
		base.OnInit ();
	}
	protected override void OnCleanUp ()
	{
		base.OnCleanUp ();
	}
	public override void PresentVisuals (bool display)
	{
		base.PresentVisuals (display);
		
		if(Name)
			Name.enabled = display;
		
		if(ItemCount)
			ItemCount.enabled = display;
		
		if(ViewText)
			ViewText.enabled = display;
		
		if(NewText)
			NewText.enabled = display;

		if(ReportText)
			ReportText.enabled = display;
		
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
	}
	#endregion
	
	#region UI Methods
	public void GenerateReport()
	{
		//ProjectAlert.Instance.Open(_project);
	}
	public void ViewProject()
	{
		//ClientPage.Instance.Setup(_project);
		OveriddenNavigation.Navigation.PushViewOnToStack(ProjectPage.Instance);
	}
	public void CreateExpense()
	{
	}
	#endregion
	
	#region Methods
	public void Setup(Project project)
	{
		_project = project;
		
		Name.text = project.Name;
		Description.text = project.Description;
		
		SetProjectCount();
	}
	void SetProjectCount()
	{
		ItemCount.text = _project.ItemCount + COUNT_SUFFIX;
	}
	#endregion
	
	#region Event Listeners
	void ExpenseCreated(Project project)
	{
		if(project == _project)
			SetProjectCount();
	}
	#endregion

}
