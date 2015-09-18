﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using gametheory.UI;

public class ExpenseListElement : VisualElement 
{
	#region Constants
	const string VALUE_PREFIX = "$";
	const string USER_PREFIX = "Submitted by: ";
	#endregion
	
	#region Public Vars
	public Text Name;
	public Text Description;
	public Text Date;
	public Text Value;
	public Text UserText;
	public RawImage Image;
	
	public Image Background;

	public Toggle Billable;
	public Toggle Reimbursement;
	#endregion
	
	#region Private Vars
	Expense _item;
	#endregion
	
	#region Overidden Methods
	public override void PresentVisuals (bool display)
	{
		base.PresentVisuals (display);
		
		if(Name)
			Name.enabled = display;
		
		if(Description)
			Description.enabled = display;
		
		if(Date)
			Date.enabled = display;
		
		if(UserText)
			UserText.enabled = display;

		if(Image)
			Image.enabled = display;
		
		if(Billable)
		{
			Billable.enabled = display;

			Billable.targetGraphic.enabled = display;
			Billable.graphic.enabled = display;
		}
		
		if(Reimbursement)
		{
			Reimbursement.enabled = display;
			
			Reimbursement.targetGraphic.enabled = display;
			Reimbursement.graphic.enabled = display;
		}

		if(Date)
			Date.enabled = display;

		if(Value)
			Value.enabled = display;
		
		if(Background)
			Background.enabled = display;
	}
	#endregion

	#region Methods
	public void Setup(Expense item)
	{
		_item = item;
		
		Name.text = item.Name;
		Description.text = item.Description;

		Date.text = item.Date.ToShortDateString();
		Value.text = VALUE_PREFIX + item.Value.ToString();

		UserText.text = USER_PREFIX + item.User[User.NAME];

		Billable.isOn = item.Billable;
		Reimbursement.isOn = item.Reimbursement;

		if(item.Image != null)
			StartCoroutine(LoadImage());
	}
	#endregion

	#region Coroutines
	IEnumerator LoadImage()
	{
		WWW request = new WWW(_item.Image.Url.AbsoluteUri);

		while(!request.isDone)
			yield return null;

		Texture2D tex = new Texture2D(1,1);

		request.LoadImageIntoTexture(tex);

		Image.texture = tex;
	}
	#endregion
}
