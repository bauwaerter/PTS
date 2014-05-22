using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Core.Helpers
{
    /// <summary>
    /// Serialize and Deserialize the object to the MediaTypeFormatter specified
    /// example usage:var value = new Person() { Name = "Alice", Age = 23 };
    /// var xml = new XmlMediaTypeFormatter();
    /// string str = Serialize(xml, value);
    /// </summary>
    public static class SerializeHelper
    {
        /// <summary>
        /// Serializes the specified formatter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string Serialize<T>(MediaTypeFormatter formatter, T value)
        {
            // Create a dummy HTTP Content.
            Stream stream = new MemoryStream();
            var content = new StreamContent(stream);
            // Serialize the object.
            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
            // Read the serialized string.
            stream.Position = 0;
            return content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Deserializes the specified formatter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static T Deserialize<T>(MediaTypeFormatter formatter, string str) where T : class
        {
            // Write the serialized string to a memory stream.
            Stream stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            // Deserialize to an object of type T
            return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
        }

    } // class
} // namespace