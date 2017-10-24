using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[InitializeOnLoad]
public class LocalizerEditor : Editor {

    public const string EXTERNALIZED_INDICATOR = "@";

    static LocalizerEditor() {
        Debug.Log("localizerEditor loaded");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void beforeSceneLoaded() {
        Debug.Log("This is running before the scene is loaded");

        string language = EditorPrefs.GetString(LanguageWindow.PREFS_SELECTED_LANG);
        Debug.Log("The current language is " + language);

        string[] stringFiles = LanguageWindow.getStringFiles();
        Dictionary<string, string> languageStrings = new Dictionary<string, string>();


        // For each text containing an externalized string, replace the externalized string placeholder
        // with the actual string value for the current language.
        FindObjectsOfType<Text>()
            .Where(text => text.text.StartsWith(EXTERNALIZED_INDICATOR)).ToList()
            .ForEach(text => {
                string stringName = text.text.Substring(EXTERNALIZED_INDICATOR.Length);
            });
    }

}
