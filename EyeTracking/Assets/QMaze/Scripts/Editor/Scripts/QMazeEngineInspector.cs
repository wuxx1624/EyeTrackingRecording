using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace qtools.qmaze
{
	[CustomEditor(typeof(QMazeEngine))]
	public class QMazeEngineInspector: Editor 
	{
		private Texture2D mazeIcon;
		private Texture2D pieceIcon;

		private static bool startFoldout 		= false;
		private static bool finishFoldout 		= false;
		private static bool exitFoldout 		= false;
		private static bool obstacleFoldout 	= false;
		private static bool eventFoldout    	= false;

		private static bool startListFoldout 	= false;
		private static bool finishListFoldout 	= false;
		private static bool exitListFoldout 	= false;
		private static bool obstacleListFoldout = false;

		private QMazeEngine mazeEngine;
		private GUIStyle labelCaption;
		private GUIStyle textFieldHC;
		private GUIStyle textFieldVC;
		private GUIStyle foldoutBold;
		private Vector2 scrollPosition;

		private SerializedProperty inited;
		private SerializedProperty mazeGeneratedEvent;
		private SerializedProperty mazePieceGeneratedEvent;
		private SerializedProperty mazeGenerateProgressEvent;

		private void OnEnable() 
		{
			inited = serializedObject.FindProperty("inited");
			mazeGeneratedEvent = serializedObject.FindProperty("mazeGeneratedEvent");
			mazePieceGeneratedEvent = serializedObject.FindProperty("mazePieceGeneratedEvent");
			mazeGenerateProgressEvent = serializedObject.FindProperty("mazeGenerateProgressEvent");

			mazeEngine = (QMazeEngine)target;

			QInspectorUtils.SetIcon(mazeEngine, QInspectorUtils.getAsset<Texture2D>("MazeEngineIcon t:texture2D"));

			mazeIcon   = QInspectorUtils.getAsset<Texture2D>("MazeIcon t:texture2D");
			pieceIcon  = QInspectorUtils.getAsset<Texture2D>("PieceIcon t:texture2D");

			startFoldout    = mazeEngine.getStartPositionList().Count > 0 || mazeEngine.getStartRandomPositionCount() > 0;
			finishFoldout   = mazeEngine.getFinishPositionList().Count > 0 || mazeEngine.getFinishRandomPositionCount() > 0;
			exitFoldout 	= mazeEngine.getExitPositionList().Count > 0;
			obstacleFoldout = mazeEngine.getObstaclePositionList().Count > 0;
			eventFoldout    = mazeEngine.mazeGeneratedEvent.GetPersistentEventCount() + 
							  mazeEngine.mazePieceGeneratedEvent.GetPersistentEventCount() + 
							  mazeEngine.mazeGenerateProgressEvent.GetPersistentEventCount() > 0;
		}

		private void initStyle()
		{
			labelCaption = new GUIStyle(GUI.skin.label); 
			labelCaption.alignment = TextAnchor.MiddleCenter;
			labelCaption.normal.background = null;
			
			textFieldHC = new GUIStyle(GUI.skin.textField); 
			textFieldHC.alignment = TextAnchor.UpperCenter;
			
			textFieldVC = new GUIStyle(GUI.skin.textField); 
            textFieldVC.alignment = TextAnchor.MiddleLeft;

			foldoutBold = new GUIStyle(EditorStyles.foldout);
			foldoutBold.font = EditorStyles.boldFont;
		}

		public override void OnInspectorGUI()
		{
			if (labelCaption == null) initStyle();

			serializedObject.Update();
			if (!inited.boolValue)
			{
				MethodInfo awakeMethod = mazeEngine.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
				awakeMethod.Invoke(mazeEngine, null);
			}

			bool errorFound = false;

			GUI.changed = false;
			Undo.RecordObject(target, "MazeEngineChnaged");

			GUILayout.Space(7);
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(28);
						GUILayout.Label("Maze Size", labelCaption, GUILayout.Width(94));
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					{
						GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(120), GUILayout.Height(50)), mazeIcon, ScaleMode.ScaleToFit);
						mazeEngine.setMazeHeight(EditorGUILayout.IntField(mazeEngine.getMazeHeight(), textFieldVC, GUILayout.Height(50), GUILayout.Width(30)));
						if (mazeEngine.getMazeHeight() < 1) mazeEngine.setMazeHeight(1);
					}
					GUILayout.EndHorizontal();
					mazeEngine.setMazeWidth(EditorGUILayout.IntField(mazeEngine.getMazeWidth(), textFieldHC, GUILayout.Width(97)));
					if (mazeEngine.getMazeWidth() < 1) mazeEngine.setMazeWidth(1);
				}
				GUILayout.EndVertical(); 

				GUILayout.BeginVertical();
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(28);
						GUILayout.Label("Piece Size", labelCaption, GUILayout.Width(94));
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					{
						GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(120), GUILayout.Height(50)), pieceIcon, ScaleMode.ScaleToFit);
						mazeEngine.setMazePieceHeight(EditorGUILayout.FloatField(mazeEngine.getMazePieceHeight(), textFieldVC, GUILayout.Height(50), GUILayout.Width(30)));
					}
					GUILayout.EndHorizontal();
					mazeEngine.setMazePieceWidth(EditorGUILayout.FloatField(mazeEngine.getMazePieceWidth(), textFieldHC, GUILayout.Width(97)));
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			mazeEngine.setMazeScale(EditorGUILayout.FloatField("Maze scale", mazeEngine.getMazeScale(), textFieldHC, GUILayout.Width(200)));

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			mazeEngine.setMazePiecePack((QMazePiecePack)EditorGUILayout.ObjectField("Maze Piece Pack", mazeEngine.getMazePiecePack(), typeof(QMazePiecePack), true));
			if (mazeEngine.getMazePiecePack() == null) 
			{
				EditorGUILayout.HelpBox("Maze Piece Pack is required", MessageType.Warning);
				errorFound = true;
			}

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
					
			startFoldout = EditorGUILayout.Foldout(startFoldout, "List Of Start Piece", (!startFoldout && (mazeEngine.getStartPositionList().Count > 0 || mazeEngine.getStartRandomPositionCount() > 0)) ? foldoutBold : EditorStyles.foldout);
			EditorGUI.indentLevel++;
			if (startFoldout)
			{
				mazeEngine.setStartRandomPosition(EditorGUILayout.Toggle("Random Position", mazeEngine.isStartRandomPosition(), GUILayout.ExpandWidth(true)));

				if (mazeEngine.isStartRandomPosition())				
					mazeEngine.setStartRandomPositionCount(EditorGUILayout.IntField("Count", mazeEngine.getStartRandomPositionCount()));				
				else				
					startListFoldout = showVector2IntDirList(mazeEngine.getStartPositionList(), startListFoldout);
			}
            EditorGUI.indentLevel--;	

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
				
			finishFoldout = EditorGUILayout.Foldout(finishFoldout, "List Of Finish Piece", (!finishFoldout && (mazeEngine.getFinishPositionList().Count > 0 || mazeEngine.getFinishRandomPositionCount() > 0)) ? foldoutBold : EditorStyles.foldout);
			EditorGUI.indentLevel++;	
			if (finishFoldout)
			{
				mazeEngine.setFinishRandomPosition(EditorGUILayout.Toggle("Random Position", mazeEngine.isFinishRandomPosition(), GUILayout.ExpandWidth(true)));

				if (mazeEngine.isFinishRandomPosition())				
					mazeEngine.setFinishRandomPositionCount(EditorGUILayout.IntField("Count", mazeEngine.getFinishRandomPositionCount()));
				else				
					finishListFoldout = showVector2IntDirList(mazeEngine.getFinishPositionList(), finishListFoldout);
			}
			EditorGUI.indentLevel--;

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			exitFoldout = EditorGUILayout.Foldout(exitFoldout, "List Of Exits", (!exitFoldout && mazeEngine.getExitPositionList().Count > 0 ? foldoutBold : EditorStyles.foldout));
			EditorGUI.indentLevel++;	
			if (exitFoldout)
			{
				exitListFoldout = showVector2IntDirList(mazeEngine.getExitPositionList(), exitListFoldout);
			}
			EditorGUI.indentLevel--;
			
			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
				
			obstacleFoldout = EditorGUILayout.Foldout(obstacleFoldout, "List Of Obstacles", (!obstacleFoldout && mazeEngine.getObstaclePositionList().Count > 0 ? foldoutBold : EditorStyles.foldout));
			EditorGUI.indentLevel++;	
			if (obstacleFoldout)
			{
				mazeEngine.setObstacleIsNone(EditorGUILayout.Toggle("Use None pieces in areas of obstacles", mazeEngine.isObstacleIsNone(), GUILayout.ExpandWidth(true)));
				obstacleListFoldout = showVector2IntList(mazeEngine.getObstaclePositionList(), obstacleListFoldout);
			}
			EditorGUI.indentLevel--;

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			if (mazeGeneratedEvent == null) 
			{
				mazeEngine.mazeGeneratedEvent = new QMazeEngine.QMazeGeneratedEvent();
				mazeGeneratedEvent = serializedObject.FindProperty("mazeGeneratedEvent");
			}
			if (mazePieceGeneratedEvent == null) 
			{
				mazeEngine.mazePieceGeneratedEvent = new QMazeEngine.QMazePieceGeneratedEvent();
				mazePieceGeneratedEvent = serializedObject.FindProperty("mazePieceGeneratedEvent");
			}
			if (mazeGenerateProgressEvent == null) 
			{
				mazeEngine.mazeGenerateProgressEvent = new QMazeEngine.QMazeGenerateProgressEvent();
				mazeGenerateProgressEvent = serializedObject.FindProperty("mazeGenerateProgressEvent");
			}

			int eventCount = mazeEngine.mazeGeneratedEvent.GetPersistentEventCount() + 
							 mazeEngine.mazePieceGeneratedEvent.GetPersistentEventCount() +
							 mazeEngine.mazeGenerateProgressEvent.GetPersistentEventCount();
			eventFoldout = EditorGUILayout.Foldout(eventFoldout, "List Of Events", !eventFoldout && eventCount > 0 ? foldoutBold : EditorStyles.foldout);
			EditorGUI.indentLevel++;	
			if (eventFoldout)
			{
				EditorGUILayout.PropertyField(mazeGeneratedEvent);
				EditorGUILayout.PropertyField(mazePieceGeneratedEvent);
				EditorGUILayout.PropertyField(mazeGenerateProgressEvent);
			}
			EditorGUI.indentLevel--;
			
			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);

			mazeEngine.setUseSeed(GUILayout.Toggle(mazeEngine.isUseSeed(), "Use seed for generation"));
			if (mazeEngine.isUseSeed())		
			{
				EditorGUI.indentLevel++;
				mazeEngine.setSeed(EditorGUILayout.IntField("Seed", mazeEngine.getSeed()));
				EditorGUI.indentLevel--;
			}

			GUILayout.Space(5);
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.Space(5);
			
			mazeEngine.setOnlyPathMode(GUILayout.Toggle(mazeEngine.isOnlyPathMode(), "Generation with the only path"));

			if (mazeEngine.gameObject.activeInHierarchy)
			{
				GUILayout.Space(5);
				GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
				GUILayout.Space(5);

				if (mazeEngine.transform.childCount == 0) 
				{
					if (errorFound) GUI.enabled = false;
					if (GUILayout.Button("Generate a Maze", GUILayout.ExpandWidth(true)))
					{
						mazeEngine.generateMaze(); 
					}
					GUI.enabled = true;
				}
				else
				{
					if (GUILayout.Button("Destroy the Maze", GUILayout.ExpandWidth(true)))
					{ 
						mazeEngine.destroyImmediateMazeGeometry();
					}
				}
			}

			GUILayout.Space(5);

			if (GUI.changed) 
			{
				EditorUtility.SetDirty(mazeEngine);
				serializedObject.ApplyModifiedProperties();
			}
		}

		private bool showVector2IntList(SerializedProperty list, bool foldout)
		{
			int listSize = list.arraySize;
			listSize = EditorGUILayout.IntField("Count", listSize);

			if(listSize != list.arraySize)
			{
				while(listSize > list.arraySize)
				{
					list.InsertArrayElementAtIndex(list.arraySize);
				}
				while(listSize < list.arraySize)
				{
					list.DeleteArrayElementAtIndex(list.arraySize - 1);
				}
			}

			if (listSize > 0)
			{
				EditorGUI.indentLevel++;
				foldout = EditorGUILayout.Foldout(foldout, "Position List");
				if (foldout)
				{
					for(int i = 0; i < list.arraySize; i++)
					{
						SerializedProperty element = list.GetArrayElementAtIndex(i);
						SerializedProperty x = element.FindPropertyRelative("x");
						SerializedProperty y = element.FindPropertyRelative("y");
						SerializedProperty direction = element.FindPropertyRelative("direction");

						EditorGUILayout.LabelField("Position " + (i + 1));
						EditorGUI.indentLevel++;	
						EditorGUILayout.PropertyField(x);
						EditorGUILayout.PropertyField(y);
						if (direction != null)
							EditorGUILayout.PropertyField(direction);
						EditorGUI.indentLevel--;	
					}
				}
				EditorGUI.indentLevel--;	
			}

			return foldout;
		}

		private bool showVector2IntDirList(List<QVector2IntDir> list, bool foldout)
		{
			int count = EditorGUILayout.IntField("Count", list.Count);
			if (count < 0) count = 0;
			if (count != list.Count)
			{
				while (count > list.Count) list.Add(new QVector2IntDir(0, 0, QMazeOutputDirection.NotSpecified));				
				if (count < list.Count) list.RemoveRange(count, list.Count - count);
			}

			if (count > 0)
			{
				EditorGUI.indentLevel++;
				foldout = EditorGUILayout.Foldout(foldout, "Position List");
				if (foldout)
				{
					for(int i = 0; i < list.Count; i++)
					{
						QVector2IntDir element = list[i];

						EditorGUILayout.LabelField("Position " + (i + 1));
						EditorGUI.indentLevel++;	

						element.x = EditorGUILayout.IntField("X", element.x);
						element.y = EditorGUILayout.IntField("Y", element.y);
						element.direction = (QMazeOutputDirection)EditorGUILayout.EnumPopup("Direction", element.direction);

						EditorGUI.indentLevel--;	
					}
				}
				EditorGUI.indentLevel--;	
			}
			
			return foldout;
		}

		private bool showVector2IntList(List<QVector2Int> list, bool foldout)
		{
			int count = EditorGUILayout.IntField("Count", list.Count);
			if (count < 0) count = 0;
			if (count != list.Count)
			{
				while (count > list.Count) list.Add(new QVector2IntDir(0, 0, QMazeOutputDirection.NotSpecified));				
				if (count < list.Count) list.RemoveRange(count, list.Count - count);
			}
			
			if (count > 0)
			{
				EditorGUI.indentLevel++;
				foldout = EditorGUILayout.Foldout(foldout, "Position List");
				if (foldout)
				{
					for(int i = 0; i < list.Count; i++)
					{
						QVector2Int element = list[i];
						
						EditorGUILayout.LabelField("Position " + (i + 1));
						EditorGUI.indentLevel++;	
						
						element.x = EditorGUILayout.IntField("X", element.x);
                        element.y = EditorGUILayout.IntField("Y", element.y);
                        
                        EditorGUI.indentLevel--;	
                    }
                }
                EditorGUI.indentLevel--;	
            }
            
            return foldout;
		}
	}
}