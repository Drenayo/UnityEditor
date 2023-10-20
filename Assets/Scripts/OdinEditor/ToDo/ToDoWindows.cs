using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace Test
{
	public class ToDoWindows : OdinEditorWindow
	{
        [MenuItem("≤‚ ‘/≤‚ ‘¥∞ø⁄")]
        private static void OpenWindow()
        {
            GetWindow<ToDoWindows>().Show();
        }

		//public List<ItemData> itemData;
	}

	//[System.Serializable]
	//public class ItemData
	//{
	//	[ToggleGroup("Toggle", "$itemName")]
	//	public bool Toggle;
	//	[ToggleGroup("Toggle")]
	//	public string itemName;
	//	[ToggleGroup("Toggle")]
	//	public string itemInfo;
	//	[ToggleGroup("Toggle")]
	//	public float floatNumber;
	//}

}