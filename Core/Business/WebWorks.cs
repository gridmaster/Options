// -----------------------------------------------------------------------
// <copyright file="WebWorks.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Core.Business
{
    public static class WebWorks
    {
        public static string GetResponse(string sUri)
        {
            var httpWRequest = (HttpWebRequest) WebRequest.Create(sUri);

            try
            {
                httpWRequest.KeepAlive = true;
                httpWRequest.ProtocolVersion = HttpVersion.Version10;

                var httpWebResponse = (HttpWebResponse) httpWRequest.GetResponse();
                var responseStream = httpWebResponse.GetResponseStream();

                if (responseStream != null)
                {
                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return "WebWorks.GetResponse ERROR: " + ex.Message;
            }

            return "";
        }

        public static string[] GetColumns(string row)
        {
            string newrow = row.Replace("<td", "~");
            string[] cols = newrow.Split('~');
            List<string> newCols = new List<string>();

            foreach (var col in cols)
            {
                string thisCol = StripHRef(col);
                if (thisCol.Equals(col))
                {
                    thisCol = StripSpan(col);
                    if (thisCol.Equals(col))
                    {
                        thisCol = CleanTag(col);
                    }
                }
                newCols.Add(thisCol);
            }

            return newCols.ToArray();
        }

        private static string CleanTag(string col)
        {
            string newCol = col;
            if (newCol.IndexOf(">", System.StringComparison.Ordinal) > -1)
            {
                string result = newCol.Substring(newCol.IndexOf(">", System.StringComparison.Ordinal) + 1);
                newCol = result.Substring(0, result.IndexOf("<", System.StringComparison.Ordinal));
            }
            return newCol;
        }

        private static string StripSpan(string col)
        {
            string newCol = col;
            if (newCol.IndexOf("<span", System.StringComparison.Ordinal) > -1)
            {
                string result = newCol.Substring(newCol.IndexOf("<span", System.StringComparison.Ordinal));
                result = result.Substring(result.IndexOf(">", System.StringComparison.Ordinal) + 1);
                newCol = result.Substring(0, result.IndexOf("<", System.StringComparison.Ordinal));
            }

            return newCol;
        }

        private static string StripHRef(string col)
        {
            string newCol = col;

            if (newCol.IndexOf("<a", System.StringComparison.Ordinal) > -1)
            {
                string result = newCol.Substring(newCol.IndexOf("<a", System.StringComparison.Ordinal));
                result = result.Substring(result.IndexOf(">", System.StringComparison.Ordinal) + 1);
                newCol = result.Substring(0, result.IndexOf("<", System.StringComparison.Ordinal));
            }

            return newCol;
        }

        public static string GetTable(string result)
        {
            string tag = "panel yui3-tabview-panel";
            int index = result.IndexOf(tag, System.StringComparison.Ordinal);
            string strTable = string.Empty;

            if (index > -1)
            {
                strTable = result.Substring(index);

                strTable = strTable.Substring(strTable.IndexOf("<table>", System.StringComparison.Ordinal));
                index = strTable.IndexOf("<\\/table>", System.StringComparison.Ordinal) + "<\\/table>".Length;
                strTable = strTable.Substring(0, index);
            }

            return strTable;
        }

        public static string[] ExtractRowsFromTable(string strTable)
        {
            strTable = strTable.Replace("<tr>", "~");
            return strTable.Split('~');
        }

        public static string[] ExtractRowsFromWebPage(string page)
        {
            string xstrTable = GetTable(page);
            return ExtractRowsFromTable(xstrTable);
        }
    }
}
