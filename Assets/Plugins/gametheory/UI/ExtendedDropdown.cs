using UnityEngine.UI;
using System.Collections.Generic;

namespace gametheory.UI
{
    public class ExtendedDropdown : VisualElement
    {
        #region Public Vars
		public Dropdown Dropdown;
        #endregion
        
        #region Methods
        public void Initialize(List<string> items, int selection)
        {
			//some stuff here
			Dropdown.options = new List<Dropdown.OptionData>();

			for(int index = 0; index < items.Count; index++)
				Dropdown.options.Add(new Dropdown.OptionData(items[index]));

			Dropdown.value = selection;
			Dropdown.captionText.text = items[selection];

			//Dropdown.options[selection].
        }
        #endregion
    }
}