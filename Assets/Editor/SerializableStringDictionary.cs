using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml;

[DataContract]
public class SerializableStringDictionary {

    [DataMember]
    private Dictionary<string, Dictionary<string, string>> strings;

    public SerializableStringDictionary() {
        strings = new Dictionary<string, Dictionary<string, string>>();
    }

    public Dictionary<string, Dictionary<string, string>> getDictionary() {
        return strings;
    }

    public void write() {
        var serializer = new DataContractSerializer(typeof(SerializableStringDictionary));
        string xmlString;
        using (var sw = new StringWriter()) {
            using (var writer = new XmlTextWriter(sw)) {
                writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                serializer.WriteObject(writer, this);
                writer.Flush();
                xmlString = sw.ToString();
            }
        }
    }
}