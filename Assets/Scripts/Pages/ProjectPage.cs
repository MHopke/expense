using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using gametheory.UI;

public class ProjectPage : UIView 
{
	#region Public Vars
	public Text Name;
	public Text Description;
	
	public static ProjectPage Instance = null;
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
	
	#region Methods
	public void Setup(Project project)
	{
		Name.text = project.Name;
		Description.text = project.Description;
	}
	#endregion
}
