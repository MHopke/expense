using UnityEngine;

using System.Collections;
using System.Threading.Tasks;

using gametheory.UI;

public class ClientAlert : UIView 
{
	#region Events
	public static event System.Action<Client> createdClient;
	#endregion

	#region Public Vars
	public static ClientAlert Instance = null;
	#endregion

	#region Private Vars
	Client _client;
	#endregion

	#region Overidden Methods
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
	public void NameChanged(string name)
	{
		_client.Name = name;
	}
	public void Submit()
	{
		_client.ACL = new Parse.ParseACL();
		_client.ACL.SetRoleReadAccess(User.CurrentUser.CompanyName,true);
		_client.ACL.SetRoleWriteAccess(User.CurrentUser.CompanyName,true);

		StartCoroutine(CreateClient());
	}
	#endregion

	#region Methods
	public void Open()
	{
		_client = new Client() { ProjectCount = 0 };

		UIAlertController.Instance.PresentAlert(this);
	}
	#endregion

	#region Coroutines
	IEnumerator CreateClient()
	{
		Task task = _client.SaveAsync();

		LoadAlert.Instance.StartLoad("Creating Client...",null,-1);

		while(!task.IsCompleted)
			yield return null;

		LoadAlert.Instance.Done();

		if(task.IsFaulted || task.Exception != null)
		{
			DefaultAlert.Present("Sorry!", "There was an error while " +
				"trying to create your client. Please try again later.");
		}
		else
		{
			Deactivate();

			if(createdClient != null)
				createdClient(_client);
			
			_client = null;
		}
	}
	#endregion
}
