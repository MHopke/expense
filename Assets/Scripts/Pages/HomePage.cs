using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Parse;

using gametheory.UI;

public class HomePage : UIView 
{
	#region Public Vars
	public UIList ClientList;
	public ClientPage ClientPage;

	public ClientListElement ClientPrefab;
	#endregion

	#region Private Vars
	bool _pulledData;
	#endregion

	#region Overidden Methods
	protected override void OnInit ()
	{
		base.OnInit ();
		Database.initialized += DatabaseInitialized;
		ClientAlert.createdClient += AddClient;
	}
	protected override void OnCleanUp ()
	{
		base.OnCleanUp ();
		Database.initialized -= DatabaseInitialized;
		ClientAlert.createdClient -= AddClient;
	}
	protected override void OnActivate ()
	{
		base.OnActivate ();

		if(!_pulledData)
		{
			Database.Instance.Init();
			_pulledData = true;
		}
	}
	#endregion

	#region UI Methods
	public void CreateClient()
	{
		ClientAlert.Instance.Open();
	}
	#endregion

	#region Methods
	public void CreateCompany()
	{
		/*ParseACL acl = new ParseACL();
		acl.PublicReadAccess = true;
		acl.PublicWriteAccess = true;

		ParseRole role = new ParseRole("Momentum Media PR",acl);*/
	}
	#endregion

	#region Event Listeners
	void DatabaseInitialized()
	{
		for(int index = 0; index < Database.Instance.Clients.Count; index++)//foreach(Client client in task.Result)
		{
			AddClient(Database.Instance.Clients[index]);
		}
	}
	void AddClient(Client client)
	{
		ClientListElement element = (ClientListElement)
			GameObject.Instantiate(ClientPrefab,Vector3.zero,Quaternion.identity);
		
		element.Setup(client);
		
		ClientList.AddListElement(element);
	}
	#endregion
}
