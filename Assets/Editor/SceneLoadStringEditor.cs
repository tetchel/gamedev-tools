using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[InitializeOnLoad]
public class SceneLoadStringEditor : Editor {

    public const string EXTERNALIZED_INDICATOR = "@";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void beforeSceneLoaded() {
        Debug.Log("This is running before the scene is loaded");

        string language = EditorPrefs.GetString(LanguageEditor.PREFS_SELECTED_LANG);
        //Debug.Log("The current language is " + language);

        Dictionary<string, Dictionary<string, string>> dict = StringEditor.instance().dict();

        if(dict == null) {
            // nothing will work
            Debug.LogError("Dictionary is null!!!");
            return;
        }

        // For each text containing an externalized string, replace the externalized string placeholder
        // with the actual string value for the current language.
        FindObjectsOfType<Text>()
            .Where(text => text.text.StartsWith(EXTERNALIZED_INDICATOR)).ToList()
            .ForEach(text => {
                string stringName = text.text.Substring(EXTERNALIZED_INDICATOR.Length);
                Dictionary<string, string> thisStringDict;
                bool got = dict.TryGetValue(stringName, out thisStringDict);
                if(!got) {
                    Debug.LogError("No localizable string named: " + stringName);
                    return;
                }

                string thisLanguageString;
                got = thisStringDict.TryGetValue(language, out thisLanguageString);
                if(!got) {
                    Debug.LogError("No entry for string " + stringName + " in language " + language);
                    return;
                }

                //Debug.Log(stringName + " in " + language + " is " + thisLanguageString);
                text.text = thisLanguageString;
            });
    }

}
