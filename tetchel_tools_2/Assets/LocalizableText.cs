using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizableText : MonoBehaviour {

    private Text text;

    public string localizedStringName = "";

    public const string STRING_FILENAME = "strings.xml";
    public const string LANG_FILENAME = "current-language.txt";
 
	// Use this for initialization
	void Start () {
        if(GetComponents<LocalizableText>().Length > 1) {
            Debug.LogError("Multiple instances of LocalizableText script attached to " + name);
        }
        else if (string.IsNullOrEmpty(localizedStringName)) {
            Debug.LogError("Localizable text is attached to text field \"" + name + 
                "\" but no Localized String Name is set.");
        }
        //else if(string.IsNullOrEmpty(STRING_FILENAME)) {
        //    Debug.LogError("No string filename set for " + name);
        //}
        else {
            text = GetComponent<Text>();

            // Get current language from file
            string languageFile = Path.Combine(Application.streamingAssetsPath, LANG_FILENAME);
            if(!File.Exists(languageFile)) {
                Debug.LogError("No selected language was found. Select a language in Localizer > Language");
            }
            string language = File.ReadAllText(languageFile);
            //Debug.Log("The current language is " + language);

            string stringFilePath = Path.Combine(Application.streamingAssetsPath, STRING_FILENAME);
            Dictionary<string, Dictionary<string, string>> dict = SerializableStringDictionary
                .read<SerializableStringDictionary>(stringFilePath)
                .dict();

            // load the dictionary which maps languages to their translated versions
            Dictionary<string, string> thisStringDict;
            if (!dict.TryGetValue(localizedStringName, out thisStringDict)) {
                Debug.LogError("No localizable string named: \"" + localizedStringName + 
                    "\". Add it in the Localizer Strings editor.");
                return;
            }
            //Debug.Log("textName=" + text.name + " localizedStringName=" + localizedStringName);

            string thisLanguageString;
            if (!thisStringDict.TryGetValue(language, out thisLanguageString)) {
                Debug.LogError("No entry for string " + localizedStringName + " in language " + language);
                return;
            }

            //Debug.Log(localizedStringName + " in " + language + " is " + thisLanguageString);
            text.text = thisLanguageString;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
