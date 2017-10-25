using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

[DataContract]
public class SerializableStringDictionary {

    [DataMember]
    private Dictionary<string, Dictionary<string, string>> strings;

    private static readonly DataContractSerializer serializer = 
        new DataContractSerializer(typeof(SerializableStringDictionary));

    public SerializableStringDictionary() {
        strings = new Dictionary<string, Dictionary<string, string>>();
    }

    public Dictionary<string, Dictionary<string, string>> dict() {
        return strings;
    }

    public static void write<T>(T toWrite, string outputXmlFile) {
        if (toWrite == null) {
            Debug.LogError("Can't write a null object");
            return;
        }

        var serializer = new DataContractSerializer(toWrite.GetType());

        using (var strWriter = new StringWriter()) {
            using (var writer = new XmlTextWriter(strWriter)) {
                writer.Formatting = Formatting.Indented;
                serializer.WriteObject(writer, toWrite);
                //writer.Flush();
                File.WriteAllText(outputXmlFile, strWriter.ToString());
            }
        }
    }

    public static T read<T>(string inputXmlFile) {
        string xml = File.ReadAllText(inputXmlFile);
        using (var strReader = new StringReader(xml)) {
            using (XmlTextReader xmlReader = new XmlTextReader(strReader)) {
                xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                return (T)serializer.ReadObject(xmlReader);
            }
        }
    }
}