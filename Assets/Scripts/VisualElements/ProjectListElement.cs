using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using gametheory.UI;

using Parse;

public class ProjectListElement : VisualElement 
{
	#region Events
	public static event System.Action<ProjectListElement,Client> switchedClient;
	public static event System.Action<ProjectListElement> deleteProject;
	#endregion

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
	public Button DeleteButton;

	public Dropdown ClientDropdown;

	public Toggle ClosedToggle;
	#endregion
	
	#region Private Vars
	bool _previousClosed;

	string _previousName, _previousDescription;

	Client _previousClient;

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

		if(ClientDropdown)
			ClientDropdown.enabled = display;

		if(ClosedToggle)
			ClosedToggle.enabled = display;

		if(DeleteButton)
			DeleteButton.enabled = display;
	}
	protected override void Disabled ()
	{
		base.Disabled ();

		if(ViewButton)
			ViewButton.interactable = false;

		if(ClosedToggle)
			ClosedToggle.interactable = false;

		if(!_project.Closed)
		{
			if(ReportButton)
				ReportButton.interactable = false;
			
			if(NewButton)
				NewButton.interactable = false;

			if(Namefield)
				Namefield.interactable = false;

			if(Descriptionfield)
				Descriptionfield.interactable = false;

			if(ClientDropdown)
				ClientDropdown.interactable = false;

			if(DeleteButton)
				DeleteButton.interactable = false;
		}
	}
	protected override void Enabled ()
	{
		base.Enabled ();
		
		if(ViewButton)
			ViewButton.interactable = true;
		
		if(ClosedToggle)
			ClosedToggle.interactable = true;

		if(!_project.Closed)
		{
			if(ReportButton)
			ReportButton.interactable = true;
			
			if(NewButton)
				NewButton.interactable = true;

			if(Namefield)
				Namefield.interactable = true;
			
			if(Descriptionfield)
				Descriptionfield.interactable = true;

			if(ClientDropdown)
				ClientDropdown.interactable = true;

			if(DeleteButton)
				DeleteButton.interactable = true;
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
		Database.Instance.CurrentProject = _project;
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
		if(_previousName != name)
		{
			_previousName = _project.Name;
			_project.Name = name;

			StartCoroutine(SaveChanges());
		}
	}
	public void DescriptionEdited(string description)
	{
		/*if(_project.Closed)
		{
			DefaultAlert.Present(EDIT_WARNING_HEADER,EDIT_WARNING_BODY);
			return;
		}*/
		if(_previousDescription != description)
		{
			_previousDescription = _project.Description;
			_project.Description = description;

			StartCoroutine(SaveChanges());
		}
	}
	public void ClientChanged(int choice)
	{
		Client client = Database.Instance.Clients[choice];

		if(client.ObjectId != _previousClient.ObjectId)
		{
			_previousClient = _project.Client;
			_project.Client = client;
			StartCoroutine(SaveChanges());
		}
	}
	public void ClosedToggled(bool closed)
	{
		if(_previousClosed != closed)
		{
			_previousClosed = _project.Closed;
			_project.Closed = closed;
			StartCoroutine(SaveChanges());
		}
	}
	public void Delete()
	{
		if(deleteProject != null)
			deleteProject(this);
	}
	#endregion
	
	#region Methods
	public void Setup(Project project)
	{
		_project = project;

		_previousName = _project.Name;
		_previousDescription = _project.Description;
		_previousClient = _project.Client;

		Namefield.text = project.Name;
		Descriptionfield.text = project.Description;

		if(_project.IsProjectLeader(ParseUser.CurrentUser))
		{
			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
			for(int index = 0; index < Database.Instance.Clients.Count; index++)
			{
				options.Add(new Dropdown.OptionData(Database.Instance.Clients[index].Name));
			}
			ClientDropdown.options = options;
			ClientDropdown.value = Database.Instance.GetClientIndex(_project.Client);

			_previousClosed = project.Closed;
			ClosedToggle.isOn = project.Closed;

			if(!DeleteButton.gameObject.activeSelf)
				DeleteButton.gameObject.SetActive(true);
		}
		else
		{
			ClosedToggle.gameObject.SetActive(false);

			if(DeleteButton.gameObject.activeSelf)
				DeleteButton.gameObject.SetActive(false);

			ClientDropdown.gameObject.SetActive(false);
		}

		SetEditable();

		SetProjectCount();
	}
	void SetProjectCount()
	{
		ItemCount.text = _project.ItemCount + COUNT_SUFFIX;
	}
	void SetEditable()
	{
		if(_project.Closed)
		{
			Namefield.interactable = false;
			Descriptionfield.interactable = false;
			ClientDropdown.interactable = false;
			ReportButton.interactable = false;
			NewButton.interactable = false;
		}
		else
		{
			Namefield.interactable = true;
			Descriptionfield.interactable = true;
			ClientDropdown.interactable = true;
			ReportButton.interactable = true;
			NewButton.interactable = true;
		}
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
			_project.Client = _previousClient;
			_project.Closed = _previousClosed;
		}
		else
		{
			_previousName = _project.Name;
			_previousDescription = _project.Description;
			_previousClosed = _project.Closed;

			SetEditable();

			if(_project.Client != _previousClient && switchedClient != null)
			{
				switchedClient(this,_previousClient);
			}
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

	#region Properties
	public Project Project
	{
		get { return _project; }
	}
	#endregion
}
