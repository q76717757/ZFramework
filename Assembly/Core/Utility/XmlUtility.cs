using System.IO;
using System.Text;
using System.Xml;

namespace ZFramework
{
    public static class XmlUtility
    {
        public static string FormatString(this XmlDocument xml)
        {
            StringBuilder sb = new StringBuilder();
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(new StringWriter(sb))
                {
                    Formatting = Formatting.Indented,
                    Indentation = 1,
                    IndentChar = '\t'
                };
                xml.WriteTo(xtw);
            }
            finally
            {
                xtw?.Close();
            }
            return sb.ToString();
        }

    }
}
