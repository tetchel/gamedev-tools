using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalizerStringEditor : EditorWindow {

    private Dictionary<string, Dictionary<string, string>> strings;

    private Vector2 size = new Vector2(1280, 720);

    public LocalizerStringEditor() {
        strings = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> test_string1 = new Dictionary<string, string>();
        test_string1.Add("en", "hi");
        test_string1.Add("fr", "bonjour");
        test_string1.Add("es", "hola");

        Dictionary<string, string> test_string2 = new Dictionary<string, string>();
        test_string2.Add("en", "hi2");
        test_string2.Add("fr", "bonjour2");
        test_string2.Add("de", "german2");

        strings.Add("string1", test_string1);
        strings.Add("string2", test_string2);
    }

    [MenuItem("Edit/Localizer/Strings")]
    public static void ShowWindow() {
        GetWindow(typeof(LocalizerStringEditor), false, title: "Edit Strings");
    }

    private void onEnable() {
        minSize = size;
    }

    private void OnDestroy() {
        // Remember size in case user resized
        size = position.size;
    }

    private void OnGUI() {
        EditorGUILayout.Space();
        stringsTable();
    }

    private void stringsTable() {
        List<string> languages = LanguageWindow.loadAllLanguages();

        float firstColWidth = position.width / (languages.Count + 1) / 2;
        float subseqColWidth = (position.width - firstColWidth) / (languages.Count);
        
        // Top row - Titles
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("String Name", GUILayout.Width(firstColWidth));
        foreach (string lang in languages) {
            EditorGUILayout.LabelField(lang, GUILayout.Width(subseqColWidth));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        // Loop over strings, and put the string name + the corresponding translated strings into the table
        foreach (KeyValuePair<string, Dictionary<string, string>> entry in strings) {
            string stringName = entry.Key;
            Dictionary<string, string> stringValues = entry.Value;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedTextField(stringName, GUILayout.Width(firstColWidth));

            foreach(string lang in languages) {
                // Get this language's version of the current string
                string translated;
                bool got = stringValues.TryGetValue(lang, out translated);
                if(!got) {
                    translated = "String N/A";
                }
                EditorGUILayout.DelayedTextField(translated, GUILayout.Width(subseqColWidth));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
