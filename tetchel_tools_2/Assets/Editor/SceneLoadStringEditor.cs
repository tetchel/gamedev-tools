using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Build;
using System.Linq;

/*
[InitializeOnLoad]
public class SceneLoadStringEditor : Editor, IPreprocessBuild, IPostprocessBuild {

    public const string LOCALIZABLE_STRING_INDICATOR = "@";

    // Maps Text elements that had their strings replaced to the original 
    // placeholder-string content.
    private static Dictionary<Text, string> replacedStrings = new Dictionary<Text, string>();

    int IOrderedCallback.callbackOrder {
        get {
            return 0;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void beforeSceneLoaded() {
        replaceAllStrings();
    }

    public void OnPreprocessBuild(BuildTarget target, string path) {
        replaceAllStrings();
    }

    // Undo the changes we made to the scene in replaceAllStrings, so that
    // the scene editor will have the placeholder strings again.
    public void OnPostprocessBuild(BuildTarget target, string path) {
        // does not work, Text objects don't exist any more at this time.
//        foreach(KeyValuePair<Text, string> changedText in replacedStrings) {
//            changedText.Key.text = changedText.Value;
//        }
    }

    // Loop over all Text objects and replace any string starting with LOCALIZABLE_STRING_INDICATOR
    // with the localized string from the current language 
    private static void replaceAllStrings() {
        replacedStrings.Clear();

        //Debug.Log("This is running before the scene is loaded");

        string language = EditorPrefs.GetString(LanguageEditor.PREFS_SELECTED_LANG);
        //Debug.Log("The current language is " + language);

        if(string.IsNullOrEmpty(language)) {
            Debug.LogError("No language is selected for the string externalizing tool! Loading strings will not work.");
            return;
        }

        Dictionary<string, Dictionary<string, string>> dict = StringEditor.instance().dict();

        if(dict == null) {
            // nothing will work
            Debug.LogError("Dictionary is null!!!");
            return;
        }

        Text[] allTexts = FindObjectsOfType<Text>();

        // For each text containing an externalized string, replace the externalized string placeholder
        // with the actual string value for the current language.
        allTexts.Where(text => text.text.StartsWith(LOCALIZABLE_STRING_INDICATOR)).ToList()
            .ForEach(text => {
                replacedStrings.Add(text, text.text);

                string stringName = text.text.Substring(LOCALIZABLE_STRING_INDICATOR.Length);

                // load the dictionary which maps languages to their translated versions
                Dictionary<string, string> thisStringDict;
                if(!dict.TryGetValue(stringName, out thisStringDict)) {
                    Debug.LogError("No localizable string named: " + stringName);
                    return;
                }

                string thisLanguageString;
                if(!thisStringDict.TryGetValue(language, out thisLanguageString)) {
                    Debug.LogError("No entry for string " + stringName + " in language " + language);
                    return;
                }

                //Debug.Log(stringName + " in " + language + " is " + thisLanguageString);
                text.text = thisLanguageString;
            });

        // You can also escape the indicator with a \, in this case we just remove the '\'
        // and the string will not be externalized.
        allTexts.Where(text => text.text.StartsWith('\\' + LOCALIZABLE_STRING_INDICATOR)).ToList()
            .ForEach(text => {
                replacedStrings.Add(text, text.text);
                text.text = text.text.Substring(1, text.text.Length-1);
            });
    }
}
*/