using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yesolm.DevOps.Utils
{

    /// <summary>
    /// Contains Json & Xml serialization helpers.
    /// </summary>
    public static class SerializationUtil
    {
        /// <summary>
        /// Readss <see cref="File"/> from <paramref name="path"/> and deserialize it to <see cref="{T}"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetObject<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public static string ToJsonString(this object obj) => JsonConvert.SerializeObject(obj); 

        /// <summary>
        /// Serilizes object tp a json string and returns each line as a list of string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<string> SerializeToJsonAndGetLines(this object obj)
        {
            using (var reader = new StringReader(JsonConvert.SerializeObject(obj, Formatting.Indented)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
