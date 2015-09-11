using UnityEngine;
using System.Collections;

using Parse;

[ParseClassName("Client")]
public class Client : ParseObject 
{
	#region Constants
	const string NAME = "Name";
	#endregion

	#region Constructors
	public Client(){}
	#endregion

	#region Properties
	[ParseFieldName("name")]
	public string Name
	{
		get { return GetProperty<string>(NAME); }
		set { SetProperty<string>(value,NAME); }
	}
	#endregion
}
