using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[InitializeOnLoad]
public class SceneLoadStringEditor : Editor {

    public const string LOCALIZABLE_STRING_INDICATOR = "@";

    // When a scene loads, loop over all Text objects and replace any string starting with LOCALIZABLE_STRING_INDICATOR
    // with the localized string from the current language 
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void beforeSceneLoaded() {
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
                text.text = text.text.Substring(1, text.text.Length-1);
            });
    }
}
