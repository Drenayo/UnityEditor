using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;


namespace Test
{
	public class NewBehaviourScript : MonoBehaviour
	{
		[InlineEditor]
		public Transform tran;
    }
}