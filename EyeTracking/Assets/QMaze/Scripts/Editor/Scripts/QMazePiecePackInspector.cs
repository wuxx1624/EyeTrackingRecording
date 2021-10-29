using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace qtools.qmaze
{
	[CustomEditor(typeof(QMazePiecePack))]
	public class QMazePiecePackInspector : Editor 	
	{
		private QMazePiecePack mazePiecePack;
		private SerializedProperty customPieces;

		private Texture2D pieceIconNone;
		private Texture2D pieceIconLine;
		private Texture2D pieceIconDeadlock;		
		private Texture2D pieceIconTriple;						
		private Texture2D pieceIconCorner;
		private Texture2D pieceIconCrossing;
		private Texture2D pieceIconStart;
		private Texture2D pieceIconFinish;
		private Texture2D pieceIconDoubleCorner;
		private Texture2D pieceIconIntersection;		
		private Texture2D pieceIconDeadlockCorner;		
		private Texture2D pieceIconDeadlockLine;		
		private Texture2D pieceIconDeadlockTriple;		
		private Texture2D pieceIconDeadlockCrossing;		
		private Texture2D pieceIconTripleDeadlock;		
		private Texture2D pieceIconLineDeadlock;		
		private Texture2D pieceIconLineDeadlockLine;		
		private Texture2D pieceIconCornerDeadlockLeft;		
		private Texture2D pieceIconCornerDeadlockRight;		
		private Texture2D pieceIconCornerDeadlockCorner;	

		private Texture2D addButton;
		private Texture2D removeButton;

		private GUIStyle imageButton;
		private GUIStyle dragAndDropLabelStyle;
		private Color greyLight;

		private Vector2 scrollPosition;
		private static bool basePiecesFoldout = true;
		private static bool additionalPiecesFoldout = true;

		private SerializedProperty inited;

		private void OnEnable()
		{
			mazePiecePack = (QMazePiecePack)target;

			inited = serializedObject.FindProperty("inited");
			greyLight = EditorGUIUtility.isProSkin ? new Color(0.25f, 0.25f, 0.25f) :  new Color(0.8f, 0.8f, 0.8f);

			customPieces = serializedObject.FindProperty("dragAndDropPieceGeometryArray");
			QInspectorUtils.SetIcon(mazePiecePack, QInspectorUtils.getAsset<Texture2D>("MazePiecePackIcon t:texture2D"));

			pieceIconNone = QInspectorUtils.getAsset<Texture2D>("PieceIconNone t:texture2D");
			pieceIconLine = QInspectorUtils.getAsset<Texture2D>("PieceIconLine t:texture2D");

			pieceIconDeadlock = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadLock t:texture2D");
			pieceIconTriple = QInspectorUtils.getAsset<Texture2D>("PieceIconTriple t:texture2D");

			pieceIconCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconCorner t:texture2D");
			pieceIconCrossing = QInspectorUtils.getAsset<Texture2D>("PieceIconCrossing t:texture2D");

			pieceIconStart = QInspectorUtils.getAsset<Texture2D>("PieceIconStart t:texture2D");
			pieceIconFinish = QInspectorUtils.getAsset<Texture2D>("PieceIconFinish t:texture2D");

			pieceIconDoubleCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconDoubleCorner t:texture2D");
			pieceIconIntersection = QInspectorUtils.getAsset<Texture2D>("PieceIconIntersection t:texture2D");

			pieceIconDeadlockCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockCorner t:texture2D");
			pieceIconDeadlockLine = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockLine t:texture2D");

			pieceIconDeadlockTriple = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockTriple t:texture2D");
			pieceIconDeadlockCrossing = QInspectorUtils.getAsset<Texture2D>("PieceIconDeadlockCrossing t:texture2D");

			pieceIconTripleDeadlock = QInspectorUtils.getAsset<Texture2D>("PieceIconTripleDeadlock t:texture2D");
			pieceIconLineDeadlock = QInspectorUtils.getAsset<Texture2D>("PieceIconLineDeadlock t:texture2D");

			pieceIconLineDeadlockLine = QInspectorUtils.getAsset<Texture2D>("PieceIconLineDeadlockLine t:texture2D");
			pieceIconCornerDeadlockLeft = QInspectorUtils.getAsset<Texture2D>("PieceIconCornerDeadlockLeft t:texture2D");

			pieceIconCornerDeadlockRight = QInspectorUtils.getAsset<Texture2D>("PieceIconCornerDeadlockRight t:texture2D");
			pieceIconCornerDeadlockCorner = QInspectorUtils.getAsset<Texture2D>("PieceIconCornerDeadlockCorner t:texture2D");

			addButton = QInspectorUtils.getAsset<Texture2D>("AddButton t:texture2D");
			removeButton = QInspectorUtils.getAsset<Texture2D>("RemoveButton t:texture2D");
		}

		private void initStyle()
		{
			imageButton = new GUIStyle(GUI.skin.button); 
			imageButton.normal.background = null;		

			dragAndDropLabelStyle = new GUIStyle(GUI.skin.label);
			dragAndDropLabelStyle.alignment = TextAnchor.MiddleCenter;
		}

		public override void OnInspectorGUI()
		{ 
			if (imageButton == null) initStyle();

			serializedObject.Update();
			if (!inited.boolValue)
			{
				MethodInfo awakeMethod = mazePiecePack.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
				awakeMethod.Invoke(mazePiecePack, null);
			}

			GUI.changed = false;
			Undo.RecordObject(target, "MazePiecePackChnaged");

			GUILayout.Space(5);

			drawDragAndDrop();

			basePiecesFoldout = EditorGUILayout.Foldout(basePiecesFoldout, "Base Pieces");
			if (basePiecesFoldout)
			{			
				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.None, "None", pieceIconNone, true);
					drawPiece(QMazePieceType.Line, "Line", pieceIconLine);            
	            GUILayout.EndHorizontal();
	            
	            GUILayout.Space(5);

				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Deadlock, "Deadlock", pieceIconDeadlock);
					drawPiece(QMazePieceType.Triple, "Triple", pieceIconTriple, true);			
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Corner, "Corner", pieceIconCorner);
					drawPiece(QMazePieceType.Crossing, "Crossing", pieceIconCrossing, true);          			
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.Start, "Start", pieceIconStart);
					drawPiece(QMazePieceType.Finish, "Finish", pieceIconFinish);
				GUILayout.EndHorizontal();

				GUILayout.Space(5);
			}

			additionalPiecesFoldout = EditorGUILayout.Foldout(additionalPiecesFoldout, "Additional Pieces");
			if (additionalPiecesFoldout)
			{
				GUILayout.BeginHorizontal();
					drawPiece(QMazePieceType.DoubleCorner, "Double Corner", pieceIconDoubleCorner, true);
					drawPiece(QMazePieceType.Intersection, "Intersection", pieceIconIntersection, true);   
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.DeadlockCorner, "Deadlock Corner", pieceIconDeadlockCorner, true);
				drawPiece(QMazePieceType.DeadlockLine, "Deadlock Line", pieceIconDeadlockLine, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.DeadlockTriple, "Deadlock Triple", pieceIconDeadlockTriple, true);
				drawPiece(QMazePieceType.DeadlockCrossing, "Deadlock Crossing", pieceIconDeadlockCrossing, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.TripleDeadlock, "Triple Deadlock", pieceIconTripleDeadlock, true);
				drawPiece(QMazePieceType.LineDeadlock, "Line Deadlock", pieceIconLineDeadlock, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.LineDeadlockLine, "Line Deadlock Line", pieceIconLineDeadlockLine, true);
				drawPiece(QMazePieceType.CornerDeadlockLeft, "Corner Deadlock Left", pieceIconCornerDeadlockLeft, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				GUILayout.BeginHorizontal();
				drawPiece(QMazePieceType.CornerDeadlockRight, "Corner Deadlock Right", pieceIconCornerDeadlockRight, true);
				drawPiece(QMazePieceType.CornerDeadlockCorner, "Corner Deadlock Corner", pieceIconCornerDeadlockCorner, true);   
				GUILayout.EndHorizontal();
				
				GUILayout.Space(5);
			}

			checkDragAndDrop();

			if (GUI.changed) EditorUtility.SetDirty(mazePiecePack);
		}

		private void drawPiece(QMazePieceType pieceType, string pieceName, Texture2D pieceIcon, bool specOptions = false)
		{
			QMazePiece piece = mazePiecePack.getPiece(pieceType);
			
			GUILayout.Box(pieceIcon);
			GUILayout.BeginVertical();
			{
				drawPieceGeometryList(pieceName, piece);
	
				bool found = false;
				bool errorFound = false;
				for (int i = 0; i < piece.geometryList.Count; i++)
				{
					if (piece.geometryList[i] != null) 
					{
						found = true;
					}
					else
					{
						errorFound = true;
					}
				}

				if (piece.isRequire() && !found) EditorGUILayout.HelpBox("Piece geometry is required", MessageType.Warning);		
				else if (errorFound) EditorGUILayout.HelpBox("One of the elements is null", MessageType.Warning);		

				if (specOptions)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(20);
						GUILayout.Label("Use");
						piece.use = EditorGUILayout.Toggle(piece.use, GUILayout.Width(15));
					}
					GUILayout.EndHorizontal();
					
					if (piece.use)
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(20);
							GUILayout.Label("Frequency", GUILayout.MinWidth(40));
							piece.frequency = Mathf.Clamp01(EditorGUILayout.FloatField(piece.frequency, GUILayout.Width(32)));	                            
						}
						GUILayout.EndHorizontal();
					}
				} 
			}
			GUILayout.EndVertical();
		}
		
		private void drawPieceGeometryList(string pieceName, QMazePiece piece)
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(addButton, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(20)))				
					piece.geometryList.Add(null);

				Rect rect = GUILayoutUtility.GetRect(50, 150, 10, 20);
				rect.x += 3;
				rect.y += 2;
				EditorGUI.LabelField(rect, pieceName);
			}
			GUILayout.EndHorizontal();
			if (piece.geometryList.Count > 0)
			{
				for(int i = 0; i < piece.geometryList.Count; i++)
				{
					GUILayout.BeginHorizontal();
					{
						bool remove = GUILayout.Button(removeButton,  GUIStyle.none, GUILayout.Width(16), GUILayout.Height(20));
						piece.geometryList[i] = (GameObject)EditorGUILayout.ObjectField(piece.geometryList[i], typeof(GameObject), true, GUILayout.MinWidth(80));
						if (remove)
						{
							piece.geometryList.RemoveAt(i);
							i--;
						}
					}
					GUILayout.EndHorizontal();
				}
			}
			else
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(20);
					GameObject gameObject = (GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject), true, GUILayout.MinWidth(80));
					if (gameObject != null)	piece.geometryList.Add(gameObject);				
				}
				GUILayout.EndHorizontal();
			}
		}

		private void drawDragAndDrop()
		{
			EditorGUI.indentLevel++;
			Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(40), GUILayout.ExpandWidth(true));
			EditorGUI.PropertyField(rect, customPieces);	
			EditorGUI.indentLevel--;
			EditorGUI.DrawRect(rect, greyLight);
			EditorGUI.LabelField(rect, "Drag & Drop", dragAndDropLabelStyle);
		}

		private void checkDragAndDrop()
		{
			for (int i = 0; i < customPieces.arraySize; i++)
			{
				GameObject go = (GameObject)customPieces.GetArrayElementAtIndex(i).objectReferenceValue;
				string goName = go.name.ToLower();
				
				string[] cornerSplit = goName.Split(new string[]{"corner"}, System.StringSplitOptions.None);
				if (cornerSplit.Length > 1)
				{
					string[] deadlockSplit0 = cornerSplit[0].Split(new string[] {"deadlock"}, System.StringSplitOptions.None);
					string[] deadlockSplit1 = cornerSplit[1].Split(new string[] {"deadlock"}, System.StringSplitOptions.None);
					if (deadlockSplit0.Length > 1)
					{
						mazePiecePack.getPiece(QMazePieceType.DeadlockCorner).geometryList.Add(go);
					}
					else if (deadlockSplit1.Length > 1)
					{
						if (cornerSplit.Length == 3)
						{
							mazePiecePack.getPiece(QMazePieceType.CornerDeadlockCorner).geometryList.Add(go);
						}
						else if (deadlockSplit1[1].Contains("left"))
						{
							mazePiecePack.getPiece(QMazePieceType.CornerDeadlockLeft).geometryList.Add(go);
						}
						else if (deadlockSplit1[1].Contains("right"))
						{
							mazePiecePack.getPiece(QMazePieceType.CornerDeadlockRight).geometryList.Add(go);
						}
					}
					else
					{
						if (deadlockSplit0[0].Contains("double"))
						{
							mazePiecePack.getPiece(QMazePieceType.DoubleCorner).geometryList.Add(go);
						}
						else
						{
							mazePiecePack.getPiece(QMazePieceType.Corner).geometryList.Add(go);
						}
					}
				}
				else
				{
					string[] deadlockSplit = cornerSplit[0].Split(new string[] {"deadlock"}, System.StringSplitOptions.None);
					if (deadlockSplit.Length > 1)
					{
						if (deadlockSplit[0].Contains("line"))
						{
							if (deadlockSplit[1].Contains("line"))
								mazePiecePack.getPiece(QMazePieceType.LineDeadlockLine).geometryList.Add(go);
							else
								mazePiecePack.getPiece(QMazePieceType.LineDeadlock).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("line"))
						{
							mazePiecePack.getPiece(QMazePieceType.LineDeadlock).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("triple"))
						{
							mazePiecePack.getPiece(QMazePieceType.TripleDeadlock).geometryList.Add(go);
						}
						else if (deadlockSplit[1].Contains("triple"))
						{
							mazePiecePack.getPiece(QMazePieceType.DeadlockTriple).geometryList.Add(go);
						}
						else if (deadlockSplit[1].Contains("crossing"))
						{
							mazePiecePack.getPiece(QMazePieceType.DeadlockCrossing).geometryList.Add(go);
						}
						else if (deadlockSplit[1].Contains("line"))
						{
							mazePiecePack.getPiece(QMazePieceType.DeadlockLine).geometryList.Add(go);
						}
						else
						{
							mazePiecePack.getPiece(QMazePieceType.Deadlock).geometryList.Add(go);
						}
					}
					else
					{
						if (deadlockSplit[0].Contains("crossing"))
						{
							mazePiecePack.getPiece(QMazePieceType.Crossing).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("finish"))
						{
							mazePiecePack.getPiece(QMazePieceType.Finish).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("intersection"))
						{
							mazePiecePack.getPiece(QMazePieceType.Intersection).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("line"))
						{
							mazePiecePack.getPiece(QMazePieceType.Line).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("none"))
						{
							mazePiecePack.getPiece(QMazePieceType.None).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("start"))
						{
							mazePiecePack.getPiece(QMazePieceType.Start).geometryList.Add(go);
						}
						else if (deadlockSplit[0].Contains("triple"))
						{
							mazePiecePack.getPiece(QMazePieceType.Triple).geometryList.Add(go);
						}
					}
				}
			}
			customPieces.ClearArray();
		}
	}
}