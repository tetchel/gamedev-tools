using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEditor;  
using UnityEngine;

public class LanguageWindow : EditorWindow {

    public const string PREFS_SELECTED_LANG = "localizer_selected_language",
            PREFS_ALL_LANGS = "localizer_languages",
            STRINGS_DIR_NAME = "LocalizedStrings",
            STRINGS_FILE_EXT = ".csv";

    private int _selectedLanguageIndex = 0;
    private List<string> _languages = new List<string>();

    private const string NEW_LANGUAGE_INIT = "Enter a new language";
    private string _newLanguageTextField = NEW_LANGUAGE_INIT;

    // Path to the Assets/LocalizerStrings folder, which contains any number of .csv files containing strings
    private static DirectoryInfo stringsDir;

    private void OnEnable() {
        // template is in Assets/Editor/localizer_template.txt - Will this ever change?
        string editorDir = Path.Combine(Application.dataPath, "Editor");
        
        stringsDir = Directory.CreateDirectory(Path.Combine(Application.dataPath, STRINGS_DIR_NAME));

        AssetDatabase.Refresh();

        // Load languages from EditorPrefs - reverse of what we do in OnDestroy()
        string selectedLanguage = EditorPrefs.GetString(PREFS_SELECTED_LANG);

        _languages = loadAllLanguages();
        _selectedLanguageIndex = _languages.FindIndex(l => l == selectedLanguage);
    }

    public static List<string> loadAllLanguages() {
        string allLanguages = EditorPrefs.GetString(PREFS_ALL_LANGS);
        return allLanguages.Split(' ').ToList();
    }

    [MenuItem("Edit/Localizer/Language")]
    public static void ShowWindow() {
        GetWindow(typeof(LanguageWindow), false, title: "Localizer");
    }

    public static string[] getStringFiles() {
        return Directory.GetFiles(stringsDir.FullName, "*" + STRINGS_FILE_EXT);
    }

    private void OnGUI() {
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        _newLanguageTextField = GUILayout.TextField(_newLanguageTextField, 32);

        if (GUILayout.Button("Add", GUILayout.Width(100))) {
            tryAddLanguage(_newLanguageTextField);
            // Clearing the language input box would be good
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        for (int i = 0; i < _languages.Count; i++) {
            GUILayout.BeginHorizontal();
            bool selected = EditorGUILayout.ToggleLeft(_languages[i], _selectedLanguageIndex == i);
            if (selected) {
                _selectedLanguageIndex = i;
            }

            if (GUILayout.Button("Remove", GUILayout.Width(100))) {
                bool remove = EditorUtility.DisplayDialog("Remove Language",
                    "Are you sure you want to remove " + _languages[i] + "?\nThis will not delete any of its strings.", 
                    "Yes", "No");

                if (remove) {
                    _languages.RemoveAt(i);
                    // Make sure a language is still selected
                    if (_selectedLanguageIndex == i && _selectedLanguageIndex != 0) {
                        _selectedLanguageIndex--;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void tryAddLanguage(string language) {
        if (string.IsNullOrEmpty(language) || language.Any(c => !isLetterOrNumber(c))) {
            EditorUtility.DisplayDialog("Invalid language", "You have to enter a name for the new language. " +
                "The language name can only contain letters and numbers.", "OK");
        }
        else {
            if (_languages.Any(s => s.Equals(language, StringComparison.InvariantCultureIgnoreCase))) {
                EditorUtility.DisplayDialog("Language Already Exists",
                    "There is already a language called \"" + language + "\".", "OK");
            }
            else {
                _languages.Add(language);

                string[] stringFiles = getStringFiles();

                // String files must be in Assets/LocalizedStrings/*.csv. All csv files will be picked up
                // Create a strings file if one does not exist already
                if (stringFiles.Count() == 0) {
                    string sheetPath = createSimpleSpreadsheet(_languages);
                    AssetDatabase.Refresh();
                    
                    EditorUtility.DisplayDialog("Created strings spreadsheet", 
                        "Created new strings file " + sheetPath + ". The localizer tool will search all " + 
                        STRINGS_FILE_EXT + " files in that directory for strings.", "OK");
                }
                else {
                    // Add the new language to all existing string files, if they do not contain it already.
                }
            }
        }
    }

    private bool isLetterOrNumber(char c) {
        return (c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122);
    }

    private string createSimpleSpreadsheet(List<string> languages) {
        string commaSepdLanguages = String.Join(",", languages.ToArray());
        string firstLine = "String Name," + commaSepdLanguages + '\n';
        string path = Path.Combine(stringsDir.FullName, "strings" + STRINGS_FILE_EXT);

        File.WriteAllText(path, firstLine);

        return path;
    }

    private void OnDestroy() {
        // TODO add Save/Apply button so you don't have to close the window
        // Save the languages separated by spaces
        string allLanguages = String.Join(" ", _languages.ToArray());

        EditorPrefs.SetString(PREFS_ALL_LANGS, allLanguages);
        EditorPrefs.SetString(PREFS_SELECTED_LANG, _languages[_selectedLanguageIndex]);
    }
}
