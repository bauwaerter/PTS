using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Core.Helpers
{
    /// <summary>
    /// XML Helper
    /// </summary>
    public partial class XmlHelpers
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string XmlEncode(string str)
        {
            if (str == null)
                return null;
            str = Regex.Replace(str, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "", RegexOptions.Compiled);
            return XmlEncodeAsIs(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string XmlEncodeAsIs(string str)
        {
            if (str == null)
                return null;
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = new XmlTextWriter(stringWriter))
            {
                xmlTextWriter.WriteString(str);
                return stringWriter.ToString();
            }
        }

    } // class
} // namespace