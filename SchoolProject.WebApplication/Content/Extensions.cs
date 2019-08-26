using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public static class Extensions {
        public static bool ToBoolean(this string value) {
            switch (value.ToLower()) {
            case "true":
                return true;
            case "t":
                return true;
            case "1":
                return true;
            case "0":
                return false;
            case "false":
                return false;
            case "f":
                return false;
            default:
                throw new InvalidCastException("You can't cast that value to a bool!");
            }
        }

        public static string[] LowMemSplit(this string s, string seperator) {
            List<string> list = new List<string>();
            int lastPos = 0;
            int pos = s.IndexOf(seperator);
            while (pos > -1) {
                while (pos == lastPos) {
                    lastPos += seperator.Length;
                    pos = s.IndexOf(seperator, lastPos);
                    if (pos == -1)
                        return list.ToArray();
                }

                string tmp = s.Substring(lastPos, pos - lastPos);
                if (tmp.Trim().Length > 0)
                    list.Add(tmp);
                lastPos = pos + seperator.Length;
                pos = s.IndexOf(seperator, lastPos);
            }

            if (lastPos < s.Length) {
                string tmp = s.Substring(lastPos, s.Length - lastPos);
                if (tmp.Trim().Length > 0)
                    list.Add(tmp);
            }

            return list.ToArray();
        }
    
        public static string[] ConvertCommaDelimetedStringToArray(string contentToConvertToArray, string fileContentDelimeter) {
            if (!string.IsNullOrEmpty(contentToConvertToArray)) {
                string[] result = contentToConvertToArray.Split(new string[] { fileContentDelimeter }, StringSplitOptions.None);
                return result;
            }
            return null;
        }

        public static string[] ConvertDelimetedStringToArray(this string contentToConvertToArray, string fileContentDelimeter) {
            if (!string.IsNullOrEmpty(contentToConvertToArray)) {
                string[] result = contentToConvertToArray.Split(new string[] { fileContentDelimeter }, StringSplitOptions.None);
                return result;
            }
            return null;
        }

        public static string[] ConvertPipedDelimetedStringToArray(string contentToConvertToArray) {
            if (!string.IsNullOrEmpty(contentToConvertToArray)) {
                return contentToConvertToArray.Split(new string[] { "|" }, StringSplitOptions.None);
            }
            return null;
        }

        public static string RemoveLastString(this string row) {
            return row.Substring(0, row.Length - 1);
        }

        public static bool TryParseDate(string datetTime) {
            DateTime outDateTime;
            if (DateTime.TryParse(datetTime, out outDateTime))
                return true;
            return false;
        }
    }
}
