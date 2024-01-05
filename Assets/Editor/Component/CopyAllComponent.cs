using UnityEngine;
using UnityEditor;
using System.Collections;

public class CopyAllComponent : EditorWindow
{
    static Component[] copiedComponents;
    [MenuItem("GameObject/复制当前对象组件")]
    static void Copy()
    {
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("GameObject/粘贴当前对象组件")]
    static void Paste()
    {
        foreach (var targetGameObject in Selection.gameObjects)
        {
            if (!targetGameObject || copiedComponents == null) continue;
            foreach (var copiedComponent in copiedComponents)
            {
                if (!copiedComponent) continue;
                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
            }
        }
    }

}
