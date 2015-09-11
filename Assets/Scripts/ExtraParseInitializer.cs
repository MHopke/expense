using UnityEngine;

using Parse;

public class ExtraParseInitializer : MonoBehaviour 
{
	#region Unity Methods
	void Awake()
	{
		ParseObject.RegisterSubclass<Client>();
		ParseObject.RegisterSubclass<Project>();
		ParseObject.RegisterSubclass<Expense>();
	}
	#endregion
}
