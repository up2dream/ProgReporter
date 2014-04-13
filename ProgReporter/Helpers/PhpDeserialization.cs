using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace ProgReporter.Helpers
{
    internal class PhpDeserialization
    {
        private const bool XmlSafe = true;
        private readonly Encoding stringEncoding = new UTF8Encoding();
        private NumberFormatInfo nfi;
        private int pos;

        internal object Deserialize(string str)
        {
            nfi = new NumberFormatInfo
            {
                NumberGroupSeparator = "",
                NumberDecimalSeparator = "."
            };

            pos = 0;
            return DeserializeIn(str);
        }

        private object DeserializeIn(string str)
        {
            if (str == null || str.Length <= pos)
                return new Object();

            int start, end, length;
            string stLen;
            switch (str[pos])
            {
                case 'N':
                    pos += 2;
                    return null;
                case 'b':
                    char chBool = str[pos + 2];
                    pos += 4;
                    return chBool == '1';
                case 'i':
                    start = str.IndexOf(":", pos, StringComparison.Ordinal) + 1;
                    end = str.IndexOf(";", start, StringComparison.Ordinal);
                    string stInt = str.Substring(start, end - start);
                    pos += 3 + stInt.Length;
                    object oRet;
                    try
                    {
                        //first try to parse as int
                        oRet = Int32.Parse(stInt, nfi);
                    }
                    catch
                    {
                        //if it failed, maybe it was too large, parse as long
                        oRet = Int64.Parse(stInt, nfi);
                    }
                    return oRet;
                case 'd':
                    start = str.IndexOf(":", pos, StringComparison.Ordinal) + 1;
                    end = str.IndexOf(";", start, StringComparison.Ordinal);
                    string stDouble = str.Substring(start, end - start);
                    pos += 3 + stDouble.Length;
                    return Double.Parse(stDouble, nfi);
                case 's':
                    start = str.IndexOf(":", pos, StringComparison.Ordinal) + 1;
                    end = str.IndexOf(":", start, StringComparison.Ordinal);
                    stLen = str.Substring(start, end - start);
                    int bytelen = Int32.Parse(stLen);
                    length = bytelen;
                    //This is the byte length, not the character length - so we might  
                    //need to shorten it before usage. This also implies bounds checking
                    if ((end + 2 + length) >= str.Length) length = str.Length - 2 - end;
                    string stRet = str.Substring(end + 2, length);
                    while (stringEncoding.GetByteCount(stRet) > bytelen)
                    {
                        length--;
                        stRet = str.Substring(end + 2, length);
                    }
                    pos += 6 + stLen.Length + length;
                    stRet = stRet.Replace("\n", "\r\n");
                    return stRet;
                case 'a':
                    //if keys are ints 0 through N, returns an ArrayList, else returns Hashtable
                    start = str.IndexOf(":", pos, StringComparison.Ordinal) + 1;
                    end = str.IndexOf(":", start, StringComparison.Ordinal);
                    stLen = str.Substring(start, end - start);
                    length = Int32.Parse(stLen);
                    var htRet = new Hashtable(length);
                    var alRet = new ArrayList(length);
                    pos += 4 + stLen.Length; //a:Len:{
                    for (int i = 0; i < length; i++)
                    {
                        //read key
                        object key = DeserializeIn(str);
                        //read value
                        object val = DeserializeIn(str);

                        if (alRet != null)
                        {
                            if (key is int && (int) key == alRet.Count)
                                alRet.Add(val);
                            else
                                alRet = null;
                        }
                        htRet[key] = val;
                    }
                    pos++; //skip the }
                    if (pos < str.Length && str[pos] == ';')
                        //skipping our old extra array semi-colon bug (er... php's weirdness)
                        pos++;
                    if (alRet != null)
                        return alRet;
                    return htRet;
                default:
                    return "";
            } //switch
        } //unserialzie(object)	
    }
}