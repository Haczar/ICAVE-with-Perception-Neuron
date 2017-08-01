using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace getReal3D
{
	public class SceneChecker
		: EditorWindow
	{
		static bool needsUpdate = true;
		static bool foundCameraUpdater = false;
		static bool foundNavigationScript = false;
        static bool foundEventSystem = false;
        static bool foundWandEventModule = false;
        static bool foundVRMenu = false;
        static bool eventCameraMissing = false;
        static bool eventCameraNotOnWand = false;

        static bool ArrayEmpty(object[] array)
		{
			return array == null || array.Length == 0;
		}
	
		[MenuItem("getReal3D/Scene Checker", false, 50)]
		static void CreateChecker()
		{
			UpdateSceneStatus();
			EditorWindow.GetWindow(typeof(SceneChecker), false, "Scene Checker");
		}

        static bool isInScene(MonoBehaviour monoBehaviour)
        {
            if(monoBehaviour.hideFlags == HideFlags.NotEditable || monoBehaviour.hideFlags == HideFlags.HideAndDontSave)
                return false;
            else if(!EditorUtility.IsPersistent(monoBehaviour.transform.root.gameObject))
                return false;
            else
                return true;
        }

        static T[] FindObjectsOfTypeInScene<T>() where T: Behaviour
        {
            List<T> res = new List<T>();
            T[] allComponents = Resources.FindObjectsOfTypeAll<T>();
            foreach(var monoBehaviour in allComponents) {
                if(monoBehaviour.hideFlags != HideFlags.None) {
                    continue;
                }
                res.Add(monoBehaviour);
            }
            return res.ToArray();
        }

        static void UpdateSceneStatus()
		{
			getRealCameraUpdater[] cu = Resources.FindObjectsOfTypeAll(typeof(getRealCameraUpdater)) as getRealCameraUpdater[];
			getRealAimAndGoController[] ag = Resources.FindObjectsOfTypeAll(typeof(getRealAimAndGoController)) as getRealAimAndGoController[];
			getRealWalkthruController[] wt = Resources.FindObjectsOfTypeAll(typeof(getRealWalkthruController)) as getRealWalkthruController[];
			getRealWandLook[] wl = Resources.FindObjectsOfTypeAll(typeof(getRealWandLook)) as getRealWandLook[];
			getRealJoyLook[] jl = Resources.FindObjectsOfTypeAll(typeof(getRealJoyLook)) as getRealJoyLook[];
			
			
			foundCameraUpdater = !ArrayEmpty(cu);
			foundNavigationScript = !(ArrayEmpty(ag) && ArrayEmpty(wt) && ArrayEmpty(wl) && ArrayEmpty(jl));

            foundEventSystem = false;
            foundWandEventModule = false;
            foundVRMenu = false;
            eventCameraMissing = false;
            eventCameraNotOnWand = false;

            UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfTypeInScene<UnityEngine.EventSystems.EventSystem>();
            foundEventSystem = eventSystems.Length != 0;
            foreach(var eventSystem in eventSystems) {
                WandEventModule wandEventModule = eventSystem.GetComponent<WandEventModule>();
                foundWandEventModule |= wandEventModule != null;
            }

            UnityEngine.Canvas[] canvases = FindObjectsOfTypeInScene<UnityEngine.Canvas>();
            foreach(var canvas in canvases) {
                if(canvas.renderMode == RenderMode.WorldSpace){
                    foundVRMenu = true;
                    Camera eventCamera = canvas.worldCamera;
                    if(eventCamera) {
                        eventCameraNotOnWand |= eventCamera.GetComponentInParent<getRealWandUpdater>() != null;
                    }
                    else {
                        eventCameraMissing = true;
                    }
                }
            }

            needsUpdate = false;
		}
		
		void OnGUI()
		{
			if (needsUpdate) UpdateSceneStatus();
			if (!foundCameraUpdater)
				EditorGUILayout.HelpBox("No getRealCameraUpdater script found. You probably want a getRealCameraUpdater attached to a GameObject with a Camera.", MessageType.Warning, true);
			else
				EditorGUILayout.HelpBox("Found getRealCameraUpdater.", MessageType.Info, true);
	
			if (!foundNavigationScript)
				EditorGUILayout.HelpBox("No getReal3D navigation scripts found. You probably want a navigation script (getRealAimAndGoController, getRealWalkthruController, getRealWandLook, getRealJoyLook) attached to a GameObject.", MessageType.None, true);
			else
				EditorGUILayout.HelpBox("Found getReal3D navigation scripts.", MessageType.Info, true);
	
			EditorGUILayout.HelpBox("Prefer getReal3D.Input over UnityEngine.Input.", MessageType.Info, true);

            if(foundVRMenu) {
                if(!foundEventSystem) {
                    EditorGUILayout.HelpBox("EventSystem script not found. VR menu won't receive any input.", MessageType.Warning, true);
                }
                else if(!foundWandEventModule) {
                    EditorGUILayout.HelpBox("EventSystem found, but no WandEventModule associated. VR menu won't receive any input.", MessageType.Warning, true);
                }
                else if(eventCameraMissing) {
                    EditorGUILayout.HelpBox("World space rendering canvas found, but no camera is associated to it. VR menu won't receive any input.", MessageType.Warning, true);
                }
                else if(!eventCameraNotOnWand) {
                    EditorGUILayout.HelpBox("World space rendering canvas found, but attached camera is not a child of the wand. VR menu won't receive any input.", MessageType.Warning, true);
                }
            }
			
			if (GUILayout.Button("Update"))
			{
				UpdateSceneStatus();
			}
		}
	}
}
