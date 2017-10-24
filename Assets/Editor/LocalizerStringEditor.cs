using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizerStringEditor : EditorWindow {

    // The first key is the string name, which points to a dict which maps language names to translated strings.dict().
    private SerializableStringDictionary strings;

    // In the stringsTable loop that draws the translated strings, this dictionary tracks strings that 
    // want to be renamed. This is so that  we can rename (ie, delete and re-add) those entries after the loop finishes.
    private Dictionary<string, string> stringsToRename = new Dictionary<string, string>();

    private static string storageFile;

    // default window size, will be overwritten when user resizes.
    private Vector2 size = new Vector2(1280, 720);

    [MenuItem("Edit/Localizer/Strings")]
    public static void ShowWindow() {
        GetWindow(typeof(LocalizerStringEditor), false, title: "Edit Strings");
    }

    void OnEnable() { 
        storageFile = Path.Combine(Application.dataPath, "strings.xml");
        strings = SerializableStringDictionary.read<SerializableStringDictionary>(storageFile);

        minSize = size;
    }

    void OnDestroy() {
        // Remember size in case user resized
        size = position.size;

        // Save the dictionary to file
        SerializableStringDictionary.write(strings, storageFile);
        Debug.Log("saved strings into " + storageFile);
    }

    void OnGUI() {
        EditorGUILayout.Space();
        stringsTable();
    }

    // Draw a table to display strings.dict(). The columns represent languages, and the rows translated strings.dict().
    private void stringsTable() {
        List<string> languages = LanguageWindow.loadAllLanguages();

        // first column is smaller than the others
        float firstColWidth = position.width / (languages.Count + 1) / 2;
        // rest of the columns take up remaining space evenly. -5 stops overflow out the right side
        float subseqColWidth = (position.width - firstColWidth) / (languages.Count) - 5;
        
        // Top row - Titles
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("String Name", GUILayout.Width(firstColWidth));
        foreach (string lang in languages) {
            EditorGUILayout.LabelField(lang, GUILayout.Width(subseqColWidth));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        // Loop over strings, and put the string name + the corresponding translated strings into the table
        foreach (KeyValuePair<string, Dictionary<string, string>> entry in strings.dict()) {
            string stringName = entry.Key;
            Dictionary<string, string> stringValues = entry.Value;

            EditorGUILayout.BeginHorizontal();
            string newStringName = EditorGUILayout.DelayedTextField(stringName, GUILayout.Width(firstColWidth));
            if(newStringName != stringName) {
                if(stringsToRename.ContainsKey(stringName)) {
                    stringsToRename.Remove(stringName);
                }
                stringsToRename.Add(stringName, newStringName);
            }

            foreach(string lang in languages) {
                // Get this language's version of the current string
                string translated;
                bool got = stringValues.TryGetValue(lang, out translated);
                if(!got) {
                    translated = "String N/A";
                }
                string newTranslated = EditorGUILayout.DelayedTextField(translated, GUILayout.Width(subseqColWidth));

                if(newTranslated != translated) {
                    stringValues.Remove(lang);
                    stringValues.Add(lang, newTranslated);
                    Debug.Log("changed " + lang + " " + stringName + " to " + newTranslated);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        // Rename any keys that were changed - we can't edit these above because it would change the dictionary
        // being iterated over
        foreach (KeyValuePair<string, string> entry in stringsToRename) {
            Dictionary<string, string> translations;
            bool got = strings.dict().TryGetValue(entry.Key, out translations);
            if(!got) {
                Debug.LogError("Failed to get stringToRename: " + entry.Key);
            }
            else {
                strings.dict().Remove(entry.Key);
                strings.dict().Add(entry.Value, translations);
                Debug.Log("Renamed " + entry.Key + " to " + entry.Value);
            }
        }
        stringsToRename.Clear();
    }
}
