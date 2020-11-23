using System;
using System.Collections;
using System.Text;

namespace Zlo4NET.Core.Helpers
{
    internal interface IPHPDeserializer
    {
        object Deserialize(string content);
    }

    // https://gist.github.com/xiangwan/1225981/1f6d12679fe510ff241468561e0f80fa757f8db4
    /// <summary>
    /// PHP to C# deserializer
    /// </summary>
    internal class PHPDeserializer : IPHPDeserializer
    {
        //types:
        // N = null
        // s = string
        // i = int
        // d = double
        // a = array (hashtable)

        public bool XMLSafe = true; //This member tells the serializer wether or not to strip carriage returns from strings when serializing and adding them back in when deserializing
        //http://www.w3.org/TR/REC-xml/#sec-line-ends

        public Encoding StringEncoding = new System.Text.UTF8Encoding();

        private System.Globalization.NumberFormatInfo nfi;

        public PHPDeserializer()
        {
            this.nfi = new System.Globalization.NumberFormatInfo();
            this.nfi.NumberGroupSeparator = "";
            this.nfi.NumberDecimalSeparator = ".";
        }

        /// <summary>
        /// Deserialize php - serialized object to object
        /// </summary>
        /// <param name="content">The content for deserialization</param>
        /// <returns>Deserialized object</returns>
        public object Deserialize(string content)
        {
            return string.IsNullOrEmpty(content)
                ? new object()
                : _deserializeObjectFromString(content);
        }

        private object _deserializeObjectFromString(string content, int handlePosition = 0)
        {
            int start, end, length;
            string stLen;

            switch (content[handlePosition]) // check character value
            {
                case 'N':
                    handlePosition += 2;
                    return null;
                case 'b':
                    char chBool;
                    chBool = content[handlePosition + 2];
                    handlePosition += 4;
                    return chBool == '1';
                case 'i':
                    string stInt;
                    start = content.IndexOf(":", handlePosition) + 1;
                    end = content.IndexOf(";", start);
                    stInt = content.Substring(start, end - start);
                    handlePosition += 3 + stInt.Length;
                    return Int32.Parse(stInt, this.nfi);
                case 'd':
                    string stDouble;
                    start = content.IndexOf(":", handlePosition) + 1;
                    end = content.IndexOf(";", start);
                    stDouble = content.Substring(start, end - start);
                    handlePosition += 3 + stDouble.Length;
                    return Double.Parse(stDouble, this.nfi);
                case 's':
                    start = content.IndexOf(":", handlePosition) + 1;
                    end = content.IndexOf(":", start);
                    stLen = content.Substring(start, end - start);
                    int bytelen = Int32.Parse(stLen);
                    length = bytelen;
                    //This is the byte length, not the character length - so we migth  
                    //need to shorten it before usage. This also implies bounds checking
                    if ((end + 2 + length) >= content.Length) length = content.Length - 2 - end;
                    string stRet = content.Substring(end + 2, length);
                    while (this.StringEncoding.GetByteCount(stRet) > bytelen)
                    {
                        length--;
                        stRet = content.Substring(end + 2, length);
                    }
                    handlePosition += 6 + stLen.Length + length;
                    if (this.XMLSafe)
                    {
                        stRet = stRet.Replace("\n", "\r\n");
                    }
                    return stRet;
                case 'a':
                    //if keys are ints 0 through N, returns an ArrayList, else returns Hashtable
                    start = content.IndexOf(":", handlePosition) + 1;
                    end = content.IndexOf(":", start);
                    stLen = content.Substring(start, end - start);
                    length = Int32.Parse(stLen);
                    Hashtable htRet = new Hashtable(length);
                    ArrayList alRet = new ArrayList(length);
                    handlePosition += 4 + stLen.Length; //a:Len:{
                    for (int i = 0; i < length; i++)
                    {
                        //read key
                        object key = _deserializeObjectFromString(content, handlePosition);
                        //read value
                        object val = _deserializeObjectFromString(content, handlePosition);

                        if (alRet != null)
                        {
                            if (key is int && (int)key == alRet.Count)
                                alRet.Add(val);
                            else
                                alRet = null;
                        }
                        htRet[key] = val;
                    }
                    handlePosition++; //skip the }
                    if (handlePosition < content.Length && content[handlePosition] == ';')//skipping our old extra array semi-colon bug (er... php's weirdness)
                        handlePosition++;
                    if (alRet != null)
                        return alRet;
                    else
                        return htRet;
                default:
                    return "";
            }
        }
    }
}