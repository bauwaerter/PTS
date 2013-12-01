using System;
using System.IO;
using System.Web.Util;
using Microsoft.Security.Application;

namespace ClassLibrary1
{
    /// <summary>
    /// Summary description for AntiXss
    /// Add the below line to your web.config inside system.web
    /// <httpRuntime targetFramework="4.5" encoderType="Core.AntiXssEncoder, Core" />
    /// </summary>
    public class AntiXssEncoder : HttpEncoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AntiXssEncoder"/> class.
        /// </summary>
        public AntiXssEncoder() { }

        /// <summary>
        /// Encodes a string into an HTML-encoded string.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <param name="output">The text writer to write the encoded value to.</param>
        protected override void HtmlEncode(string value, TextWriter output)
        {
            //output.Write(AntiXss.HtmlEncode(value));
            output.Write(Encoder.HtmlEncode(value));
        }

        /// <summary>
        /// Encodes an incoming value into a string that can be inserted into an HTML attribute that is delimited by using single or double quotation marks.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <param name="output">The text writer to write the encoded value to.</param>
        protected override void HtmlAttributeEncode(string value, TextWriter output)
        {
            //output.Write(AntiXss.HtmlAttributeEncode(value));
            output.Write(Encoder.HtmlAttributeEncode(value));
        }

        /// <summary>
        /// Decodes a value from an HTML-encoded string.
        /// </summary>
        /// <param name="value">The string to decode.</param>
        /// <param name="output">The text writer to write the decoded value to.</param>
        protected override void HtmlDecode(string value, TextWriter output)
        {
            base.HtmlDecode(value, output);
        }

        /// <summary>
        /// Encodes an array of characters that are not allowed in a URL into a hexadecimal character-entity equivalent.
        /// </summary>
        /// <param name="bytes">An array of bytes to encode.</param>
        /// <param name="offset">The position in the <paramref name="bytes" /> array at which to begin encoding.</param>
        /// <param name="count">The number of items in the <paramref name="bytes" /> array to encode.</param>
        /// <returns>
        /// An array of encoded characters.
        /// </returns>
        protected override byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            //Can't call AntiXss library because the AntiXss library works with Unicode strings.
            //This override works at a lower level with just a stream of bytes, independent of 
            //the original encoding.

            //
            //Internal ASP.NET implementation reproduced below.
            //
            int cSpaces = 0;
            int cUnsafe = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];

                if (ch == ' ')
                    cSpaces++;
                else if (!IsUrlSafeChar(ch))
                    cUnsafe++;
            }

            // nothing to expand?
            if (cSpaces == 0 && cUnsafe == 0)
                return bytes;

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + cUnsafe * 2];
            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];
                char ch = (char)b;

                if (IsUrlSafeChar(ch))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
            }

            return expandedBytes;


        }

        /// <summary>
        /// Encodes a subsection of a URL.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <returns>
        /// A URL-encoded string.
        /// </returns>
        protected override string UrlPathEncode(string value)
        {
            //AntiXss.UrlEncode is too "pessimistic" for how ASP.NET uses UrlPathEncode

            //ASP.NET's UrlPathEncode splits the query-string off, and then Url encodes
            //the Url path portion, encoding any parts that are non-ASCII, or that
            //are <= 0x20 or >=0x7F.

            //Additionally, it is expected that:
            //                       UrPathEncode(string) == UrlPathEncode(UrlPathEncode(string))
            //which is not the case for UrlEncode.

            //The Url needs to be separated into individual path segments, each of which
            //can then be Url encoded.
            string[] parts = value.Split("?".ToCharArray());
            string originalPath = parts[0];

            string originalQueryString = null;
            if (parts.Length == 2)
                originalQueryString = "?" + parts[1];

            string[] pathSegments = originalPath.Split("/".ToCharArray());

            for (int i = 0; i < pathSegments.Length; i++)
            {
                pathSegments[i] = AntiXss.UrlEncode(pathSegments[i]);  //this step is currently too aggressive
            }

            return String.Join("/", pathSegments) + originalQueryString;
        }

        /// <summary>
        /// Determines whether [is URL safe char] [the specified ch].
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <returns>
        ///   <c>true</c> if [is URL safe char] [the specified ch]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUrlSafeChar(char ch)
        {
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
                return true;

            switch (ch)
            {

                //These are the characters ASP.NET considers safe by default
                //case '-':
                //case '_':
                //case '.':
                //case '!':
                //case '*':
                //case '\'':
                //case '(':
                //case ')':
                //    return true;

                //Modified list based on what AntiXss library allows from the ASCII character set
                case '-':
                case '_':
                case '.':
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ints to hex.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        private char IntToHex(int n)
        {
            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'a');
        }

    } // class
} // namespace