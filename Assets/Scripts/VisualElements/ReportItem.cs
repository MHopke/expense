using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using gametheory.UI;

public class ReportItem : VisualElement 
{
	#region Public Vars
	public Text Name;
	public Text Value;
	#endregion

	#region Overridden Methods
	public override void PresentVisuals (bool display)
	{
		base.PresentVisuals (display);

		if(Name)
			Name.enabled = display;

		if(Value)
			Value.enabled = display;
	}
	#endregion

	#region Methods
	public void Setup(Expense expense)
	{
		Name.text = expense.Name;
		Value.text = "$" + expense.Value;
	}
	#endregion
}
