using UnityEngine;

using System.Collections;

using Parse;

using gametheory;

[ParseClassName("Client")]
public class Client : ParseObject, IBindingContext 
{
	#region Events
	public event System.Action<object,string> propertyChanged;
	#endregion

	#region Constants
	const string NAME = "Name";
	const string PROJECT_COUNT = "ProjectCount";
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

	[ParseFieldName("projectCount")]
	public int ProjectCount
	{
		get { return GetProperty<int>(PROJECT_COUNT); }
		set 
		{ 
			if(ProjectCount != value)
			{
				SetProperty<int>(value,PROJECT_COUNT); 

				if(propertyChanged != null)
					propertyChanged(this, "ProjectCount");
			}
		}
	}
	#endregion
}