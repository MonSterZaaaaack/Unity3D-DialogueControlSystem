using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using DialogueControl;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class DialogueDataEditor : EditorWindow
{
    private DialogueData dialogueData;
    private ActorDatabase actorDatabase;
    private SerializedObject serializedData;
    private Vector2 sidebarScroll;
    private Vector2 mainScroll;
    private int selectedIndex = -1;
    private string defaultLocalizationFolder = "Assets/Localization/Tables";

    private readonly Color dividerColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    private const int dividerWidth = 2;
    private const float sidebarWidth = 250f;

    private Dictionary<(string, string), string> tempLocalizedEdits = new();
    private Dictionary<(string, string), string> tempOptionEdits = new();
    private string fileName = "";

    private Locale selectedLocale;
    private List<Locale> availableLocales = new();
    private string[] localeNames;
    private int selectedLocaleIndex = 0;
    [MenuItem("Tools/Dialogue Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<DialogueDataEditor>("Dialogue Editor");
        window.minSize = new Vector2(900, 600);
        window.RefreshAvailableLocales();
    }

    private void OnGUI()
    {
        DrawLocaleDropdown();
        EditorGUILayout.BeginHorizontal();

        DrawSidebar();

        Rect dividerRect = new Rect(sidebarWidth, 0, dividerWidth, position.height);
        EditorGUI.DrawRect(dividerRect, dividerColor);
        GUILayout.Space(dividerWidth);

        DrawMainPanel();

        EditorGUILayout.EndHorizontal();
    }

    void DrawSidebar()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(sidebarWidth));
        GUILayout.Label("Dialogue Data", EditorStyles.boldLabel);
        dialogueData = (DialogueData)EditorGUILayout.ObjectField(dialogueData, typeof(DialogueData), false);
        if(dialogueData != null)
        {
            fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(dialogueData));
        }
        GUILayout.Label("Actor Database", EditorStyles.boldLabel);
        actorDatabase = (ActorDatabase)EditorGUILayout.ObjectField(actorDatabase, typeof(ActorDatabase), false);

        if (dialogueData != null)
        {
            if (serializedData == null || serializedData.targetObject != dialogueData)
            {
                serializedData = new SerializedObject(dialogueData);
            }

            sidebarScroll = EditorGUILayout.BeginScrollView(sidebarScroll);
            for (int i = 0; i < dialogueData.dialogueLines.Count; i++)
            {
                if (GUILayout.Button($"Dialogue Line {i + 1}", (selectedIndex == i) ? EditorStyles.toolbarButton : EditorStyles.miniButton))
                {
                    GUI.FocusControl(null);
                    selectedIndex = i;
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Dialogue Line"))
            {
                var newLine = new DialogueLines();
                
                newLine.tableName = fileName;
                newLine.tableKey = GenerateUniqueLineKey();
                dialogueData.dialogueLines.Add(newLine);
                selectedIndex = dialogueData.dialogueLines.Count - 1;
            }
            if (GUILayout.Button("Save All Changes"))
            {
                SaveAllDialogueLines();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawLocaleDropdown()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.FlexibleSpace();

        if (localeNames != null && localeNames.Length > 0)
        {
            EditorGUI.BeginChangeCheck();
            selectedLocaleIndex = EditorGUILayout.Popup(selectedLocaleIndex, localeNames, GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
            {
                selectedLocale = availableLocales[selectedLocaleIndex];
                LocalizationSettings.SelectedLocale = selectedLocale;
                tempLocalizedEdits.Clear();
                tempOptionEdits.Clear();
            }
        }

        EditorGUILayout.EndHorizontal();
    }
    private void RefreshAvailableLocales()
    {
        availableLocales = new List<Locale>(LocalizationEditorSettings.GetLocales());
        localeNames = new string[availableLocales.Count];

        for (int i = 0; i < availableLocales.Count; i++)
        {
            localeNames[i] = availableLocales[i].Identifier.ToString();
        }

        selectedLocale = LocalizationSettings.SelectedLocale ?? availableLocales[0];
        selectedLocaleIndex = availableLocales.IndexOf(selectedLocale);
    }
    void DrawMainPanel()
    {
        EditorGUILayout.BeginVertical();

        if (dialogueData != null && selectedIndex >= 0 && selectedIndex < dialogueData.dialogueLines.Count)
        {
            var line = dialogueData.dialogueLines[selectedIndex];

            // Early check: allow deletion without starting layout
            if (line == null)
            {
                return;
            }

            // Render Delete button before scroll view
            if (GUILayout.Button("Delete This Dialogue Line"))
            {
                dialogueData.dialogueLines.RemoveAt(selectedIndex);
                selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, dialogueData.dialogueLines.Count - 1);
                GUI.FocusControl(null);  // Optional: clear focus to avoid ghost fields
                return;
            }

            mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

            EditorGUILayout.LabelField("Table Name: ",line.tableName);
            line.tableKey = EditorGUILayout.TextField("Table Key", line.tableKey);

            var key = (line.tableName, line.tableKey);
            if (!tempLocalizedEdits.TryGetValue(key, out var localizedText))
            {
                localizedText = GetLocalizedString(line.tableName, line.tableKey);
            }

            string newLocalizedText = EditorGUILayout.TextField("Localized Text", localizedText ?? "");
            tempLocalizedEdits[key] = newLocalizedText;

            DrawActorFieldWithName("Main Actor", line.mainActor);
            DrawActorFieldWithName("Talking To Actor", line.talkingToActor);
            line.hasOption = EditorGUILayout.Toggle("Has Options", line.hasOption);
            if (line.hasOption)
            {
                if (line.options == null)
                    line.options = new List<DialogueOptions>();

                for (int i = 0; i < line.options.Count; i++)
                {
                    var opt = line.options[i];
                    EditorGUILayout.LabelField($"Option {i + 1}", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Option Table Name", opt.tableName);
                    opt.tableKey = EditorGUILayout.TextField("Option Table Key", opt.tableKey);

                    var optKey = (opt.tableName, opt.tableKey);
                    if (!tempOptionEdits.TryGetValue(optKey, out var optText))
                    {
                        optText = GetLocalizedString(opt.tableName, opt.tableKey);
                    }
                    string newOptText = EditorGUILayout.TextField("Option Text", optText ?? "Option Text Not Found");
                    tempOptionEdits[optKey] = newOptText;

                    opt.skipTo = EditorGUILayout.IntField("Skip To", opt.skipTo);

                    if (GUILayout.Button("Remove Option"))
                    {
                        line.options.RemoveAt(i);
                        break;
                    }
                }

                if (line.options.Count < 4)
                {
                    if (GUILayout.Button("Add Option"))
                    {
                        int nextIndex = line.options.Count + 1;
                        var newOption = new DialogueOptions
                        {
                            tableName = line.tableName,
                            tableKey = GenerateUniqueOptionKey(line)
                        };
                        line.options.Add(newOption);
                    }
                }
            }
            line.reAssignPosition = EditorGUILayout.Toggle("Reassign Position", line.reAssignPosition);
            if (line.reAssignPosition)
            {
                //line.leftSidePosition2Actor = DrawActorField("Left Side Actor", line.leftSidePosition2Actor);
                DrawActorFieldWithName("Left Side Actor 2", line.leftSidePosition2Actor);
                //line.rightSidePosition2Actor = DrawActorField("Right Side Actor", line.rightSidePosition2Actor);
                DrawActorFieldWithName("Right Side Actor 2", line.rightSidePosition2Actor);
            }

            line.jumpTo = EditorGUILayout.IntField("Jump To", line.jumpTo);

            EditorGUILayout.Space();


            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    ActorInScene DrawActorField(string label, ActorInScene actor)
    {
        if (actor == null) actor = new ActorInScene();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        actor.actorId = EditorGUILayout.IntField("Actor ID", actor.actorId);
        actor.actorExpression = (ActorExpression)EditorGUILayout.EnumPopup("Expression", actor.actorExpression);
        return actor;
    }

    void DrawActorFieldWithName(string label, ActorInScene actor)
    {
        if (actor == null) actor = new ActorInScene();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        actor.actorId = EditorGUILayout.IntField("Actor ID", actor.actorId);

        string nameText = "ActorData Not Found";
        if (actorDatabase != null)
        {
            var match = actorDatabase.datas.Find(d => d.actorId == actor.actorId);
            if (match != null)
            {
                string localizedName = GetLocalizedString(match.tableName, match.tableEntry);
                nameText = localizedName ?? "(Name Missing in Table)";
            }
        }
        EditorGUILayout.LabelField(nameText, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        actor.actorExpression = (ActorExpression)EditorGUILayout.EnumPopup("Expression", actor.actorExpression);
    }

    string GetLocalizedString(string tableName, string key)
    {
        var tableCollection = LocalizationEditorSettings.GetStringTableCollection(tableName);
        if (tableCollection != null && tableCollection.SharedData.GetEntry(key) != null)
        {
            var locale = selectedLocale?.Identifier.Code ?? "en";
            var table = tableCollection.GetTable(locale) as StringTable;
            if (table != null && table.GetEntry(key) != null)
            {
                return table.GetEntry(key).LocalizedValue;
            }
        }
        return null;
    }

    void UpdateLocalization(string tableName, string key, string value)
    {
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(key)) return;

        var collection = LocalizationEditorSettings.GetStringTableCollection(tableName);
        if (collection == null)
        {
            string path = Path.Combine(defaultLocalizationFolder, tableName + ".asset");
            collection = LocalizationEditorSettings.CreateStringTableCollection(tableName, path);
            AssetDatabase.SaveAssets();  // Ensure the new collection is saved
            AssetDatabase.Refresh();
        }

        if (!collection.SharedData.Contains(key))
        {
            collection.SharedData.AddKey(key);
            EditorUtility.SetDirty(collection.SharedData);  // Mark shared data as dirty
        }

        var locale = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
        var table = collection.GetTable(locale) as StringTable;
        if (table == null)
        {
            table = collection.AddNewTable(locale) as StringTable;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        var entry = table.GetEntry(key);
        if (entry == null)
        {
            table.AddEntry(key, value);
        }
        else
        {
            entry.Value = value;
        }

        EditorUtility.SetDirty(table);                 // Ensure the table is marked dirty
        EditorUtility.SetDirty(collection);            // Also mark the collection dirty
        AssetDatabase.SaveAssets();                    // Save to disk
        AssetDatabase.Refresh();                       // Refresh asset database
    }

    void SaveAllDialogueLines()
    {
        if (dialogueData == null) return;

        foreach (var line in dialogueData.dialogueLines)
        {
            var key = (line.tableName, line.tableKey);
            if (tempLocalizedEdits.TryGetValue(key, out var text))
            {
                UpdateLocalization(key.Item1, key.Item2, text);
            }

            if (line.hasOption && line.options != null)
            {
                foreach (var option in line.options)
                {
                    var optKey = (option.tableName, option.tableKey);
                    if (tempOptionEdits.TryGetValue(optKey, out var optText))
                    {
                        UpdateLocalization(optKey.Item1, optKey.Item2, optText);
                    }
                }
            }
        }

        EditorUtility.SetDirty(dialogueData);
        AssetDatabase.SaveAssets();
    }
    string GenerateUniqueLineKey()
    {
        int maxKey = 100000;
        foreach (var line in dialogueData.dialogueLines)
        {
            if (int.TryParse(line.tableKey, out int existingKey))
            {
                maxKey = Mathf.Max(maxKey, existingKey);
            }
        }
        return (maxKey + 1).ToString();
    }

    string GenerateUniqueOptionKey(DialogueLines line)
    {
        int maxOptionIndex = 0;
        foreach (var opt in line.options)
        {
            string suffix = opt.tableKey.Substring(line.tableKey.Length);
            if (int.TryParse(suffix, out int optIndex))
            {
                maxOptionIndex = Mathf.Max(maxOptionIndex, optIndex);
            }
        }
        return $"{line.tableKey}{(maxOptionIndex + 1):00}";
    }
}