using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StringEditor : EditorWindow {

    private static StringEditor _instance;

    // The first key is the string name, which points to a dict which maps language names to translated strings.dict().
    private SerializableStringDictionary _strings;

    private static string _storageFile;

    private Vector2 _size = new Vector2(1280, 720);

    private Vector2 _scrollPos;

    private StringEditor() { }

    public static StringEditor instance() {
        if(_instance == null) {
            _instance = CreateInstance<StringEditor>();
            _instance.OnEnable();
        }
        return _instance;
    }

    [MenuItem("Edit/Localizer/Strings")]
    public static void ShowWindow() {
        GetWindow(typeof(StringEditor), false, title: "Edit Strings");
    }

    public Dictionary<string, Dictionary<string, string>> dict() {
        return _strings.dict();
    }

    void OnEnable() {
        position = new Rect(position.position, _size);

        // load the dictionary from the xml
        _storageFile = Path.Combine(Application.dataPath, "strings.xml");
        _strings = SerializableStringDictionary.read<SerializableStringDictionary>(_storageFile);
    }

    void OnDestroy() {
        // Remember size in case user resized
        _size = position.size;

        // Save the dictionary to file
        SerializableStringDictionary.write(_strings, _storageFile);
        //Debug.Log("saved strings into " + _storageFile);
    }

    void OnGUI() {
        GUILayout.BeginVertical(GUILayout.Height(position.height));
        EditorGUILayout.Space();
        stringsTable();

        //GUILayout.FlexibleSpace();

        GUILayoutOption width = GUILayout.Width(150);
        GUILayoutOption height = GUILayout.Height(25);

        // Bottom buttons
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Edit Languages", width, height)) {
            LanguageEditor.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("OK", width, height)) {
            Close();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    // Draw a table to display strings.dict(). The columns represent languages, and the rows translated strings.dict().
    private void stringsTable() {
        List<string> languages = LanguageEditor.loadAllLanguages();

        int removeButtonWidth = 80;
        int rowHeight = 20;
        float minimumColWidth = 256;
        // first column is smaller than the others
        float firstColWidth = (position.width - removeButtonWidth) / (languages.Count + 1) / 2;
        if(firstColWidth < minimumColWidth / 1.5f) {
            firstColWidth = minimumColWidth / 1.5f;
        }
        // rest of the columns take up remaining space evenly. -50 stops overflow out the right side when scrolling
        float subseqColWidth = (position.width - firstColWidth - removeButtonWidth - 50) / languages.Count;
        if(subseqColWidth < minimumColWidth) {
            subseqColWidth = minimumColWidth;
        }
        
        // Top row - Titles
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(removeButtonWidth));
        EditorGUILayout.LabelField("String Name", GUILayout.Width(firstColWidth));
        foreach (string lang in languages) {
            EditorGUILayout.LabelField(lang, GUILayout.Width(subseqColWidth));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // In the stringsTable loop that draws the translated strings, this dictionary tracks strings that 
        // want to be renamed. This is so that  we can rename (ie, delete and re-add) those entries after the loop finishes.
        Dictionary<string, string> stringsToRename = new Dictionary<string, string>();
       
        int originalFontsize = GUI.skin.textField.fontSize;

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        // Loop over strings, and put the string name + the corresponding translated strings into the table as rows
        foreach (KeyValuePair<string, Dictionary<string, string>> entry in _strings.dict()) {
            string stringName = entry.Key;
            Dictionary<string, string> stringValues = entry.Value;

            GUI.skin.textField.fontSize = 14;

            EditorGUILayout.BeginHorizontal();
            // a Remove button for each string
            if(GUILayout.Button("Remove", GUILayout.Width(removeButtonWidth), GUILayout.Height(rowHeight))) {
                if(EditorUtility.DisplayDialog("Remove String", "Are you ABSOLUTELY sure you want to remove " 
                    + stringName + "?\nThis WILL DELETE all translations of this string.", 
                    "OK", "Cancel")) {
                    // A stringToRename with a null new name means a string to delete
                    stringsToRename.Add(stringName, null);
                }
            }

            // It would be better to make this a Delayed field since we don't have to store values while user is typing
            // But then if a user closes the window without deselecting the textfield, the changes will not be saved.
            string newStringName = EditorGUILayout.TextField(stringName, GUILayout.Width(firstColWidth), 
                GUILayout.Height(rowHeight));
            if(newStringName != stringName) {
                if(stringsToRename.ContainsKey(stringName)) {
                    stringsToRename.Remove(stringName);
                }
                stringsToRename.Add(stringName, newStringName);
            }

            foreach(string lang in languages) {
                // Get this language's version of the current string
                string translated;
                if(!stringValues.TryGetValue(lang, out translated)) {
                    translated = "String N/A";
                }
                // Should be a delayed text field as discussed above
                string newTranslated = EditorGUILayout.TextField(translated, GUILayout.Width(subseqColWidth), 
                    GUILayout.Height(rowHeight));

                if(newTranslated != translated) {
                    stringValues.Remove(lang);
                    stringValues.Add(lang, newTranslated);
                    //Debug.Log("changed " + lang + " " + stringName + " to " + newTranslated);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        GUI.skin.textField.fontSize = originalFontsize;
        
        // Rename any keys that were changed - we can't edit these above because it would change the dictionary
        // being iterated over
        foreach (KeyValuePair<string, string> entry in stringsToRename) {
            Dictionary<string, string> translations;
            if(!_strings.dict().TryGetValue(entry.Key, out translations)) {
                Debug.LogError("Failed to get stringToRename: " + entry.Key);
            }
            else {
                _strings.dict().Remove(entry.Key);
                if(entry.Value != null) {
                    _strings.dict().Add(entry.Value, translations);
                }
                //Debug.Log("Renamed " + entry.Key + " to " + entry.Value);
            }
        }
        stringsToRename.Clear();

        string defaultStringBaseName = "NewString";
        string defaultStringName = defaultStringBaseName;
        int defaultStringIndex = 1;
        if(GUILayout.Button("Add String", GUILayout.Width(firstColWidth), GUILayout.Height(30))) {
            // Add an index to the new string names to prevent collision
            // eg. UnnamedString, UnnamedString1, UnnamedString2 ...
            while(_strings.dict().ContainsKey(defaultStringName)) {
                defaultStringName = defaultStringBaseName + defaultStringIndex++;
            }
            _strings.dict().Add(defaultStringName, new Dictionary<string, string>());
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndScrollView();
    }
}
