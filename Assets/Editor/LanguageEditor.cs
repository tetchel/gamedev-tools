using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;  
using UnityEngine;

public class LanguageEditor : EditorWindow {

    public const string 
            PREFS_SELECTED_LANG = "localizer_selected_language",
            PREFS_ALL_LANGS = "localizer_languages";

    private int _selectedLanguageIndex = 0;
    private List<string> _languages = new List<string>();

    private const string NEW_LANGUAGE_INIT = "Enter a new language";
    private string _newLanguageTextField = NEW_LANGUAGE_INIT;

    private LanguageEditor() { }

    [MenuItem("Edit/Localizer/Language")]
    public static void ShowWindow() {
        GetWindow(typeof(LanguageEditor), false, title: "Localizer");
    }

    void OnEnable() {
        // Load languages from EditorPrefs - reverse of what we do in OnDestroy()
        string selectedLanguage = EditorPrefs.GetString(PREFS_SELECTED_LANG);

        _languages = loadAllLanguages();
        _selectedLanguageIndex = _languages.FindIndex(l => l == selectedLanguage);
    }

    void OnDestroy() {
        saveLanguages();
    }

    void saveLanguages() {
        // Save the languages separated by spaces
        string allLanguages = String.Join(" ", _languages.ToArray());

        EditorPrefs.SetString(PREFS_ALL_LANGS, allLanguages);
        EditorPrefs.SetString(PREFS_SELECTED_LANG, _languages[_selectedLanguageIndex]);
    }

    public static List<string> loadAllLanguages() {
        string allLanguages = EditorPrefs.GetString(PREFS_ALL_LANGS);
        return allLanguages.Split(' ').ToList();
    }

    void OnGUI() {
        GUILayout.BeginVertical(GUILayout.Height(position.height));
        EditorGUILayout.Space();

        // Top row - New Language text box and Add button to do the adding
        GUILayout.BeginHorizontal();
        _newLanguageTextField = GUILayout.TextField(_newLanguageTextField, 32, GUILayout.Width(250));

        if (GUILayout.Button("Add", GUILayout.Width(100))) {
            tryAddLanguage(_newLanguageTextField);
            // Clearing the language input box would be good
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // Draw the languages list and toggle box for each
        for (int i = 0; i < _languages.Count; i++) {
            GUILayout.BeginHorizontal();
            // Only one can be selected at a time
            bool selected = EditorGUILayout.ToggleLeft(_languages[i], _selectedLanguageIndex == i);
            if (selected) {
                _selectedLanguageIndex = i;
            }

            // a Remove button for each existing language
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

        GUILayout.FlexibleSpace();

        GUILayoutOption width = GUILayout.Width(100);
        GUILayoutOption height = GUILayout.Height(25);

        // Bottom buttons
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Edit Strings", width, height)) {
            StringEditor.ShowWindow();
        }
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Apply", width, height)) {
            saveLanguages();
        }
        if(GUILayout.Button("OK", width, height)) {
            saveLanguages();
            Close();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void tryAddLanguage(string language) {
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
            }
        }
    }

    bool isLetterOrNumber(char c) {
        return (c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122);
    }
}
