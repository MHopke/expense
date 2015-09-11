using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using gametheory.UI;

public class ClientListElement : VisualElement 
{
	#region Constants
	const string COUNT_SUFFIX = " Projects";
	#endregion

	#region Public Vars
	public Text Name;
	public Text ProjectCount;
	public Text ViewText;
	public Text NewText;

	public Button ViewButton;
	public Button NewButton;
	#endregion

	#region Private Vars
	Client _client;
	#endregion

	#region Overidden Methods
	protected override void OnInit ()
	{
		ProjectAlert.projectCreated += ProjectCreated;
		base.OnInit ();
	}
	protected override void OnCleanUp ()
	{
		ProjectAlert.projectCreated -= ProjectCreated;
		base.OnCleanUp ();
	}
	public override void PresentVisuals (bool display)
	{
		base.PresentVisuals (display);

		if(Name)
			Name.enabled = display;

		if(ProjectCount)
			ProjectCount.enabled = display;

		if(ViewText)
			ViewText.enabled = display;

		if(NewText)
			NewText.enabled = display;

		if(ViewButton)
		{
			ViewButton.enabled = display;

			if(ViewButton.image)
				ViewButton.image.enabled = display;
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

		if(NewButton)
			NewButton.interactable = false;
	}
	protected override void Enabled ()
	{
		base.Enabled ();

		if(ViewButton)
			ViewButton.interactable = true;
		
		if(NewButton)
			NewButton.interactable = true;
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

		SetProjectCount();
	}
	void SetProjectCount()
	{
		ProjectCount.text = _client.ProjectCount + COUNT_SUFFIX;
	}
	#endregion

	#region Event Listeners
	void ProjectCreated(Client client)
	{
		if(client == _client)
			SetProjectCount();
	}
	#endregion
}
