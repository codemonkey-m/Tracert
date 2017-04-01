using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tracert可视化工具
{
    static class TagInfo
    {
        public static string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // 创建一个HTTP请求
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseText = myreader.ReadToEnd();
            myreader.Close();
            return responseText;
        }

        public static string GetJsonData(string strValue, string reg)
        {
            Match m = Regex.Match(strValue, "\"" + reg + "\":\".*?\"");
            string str = "";
            if (m.Value.Length > 0)
                str = m.Value.Substring(4 + reg.Length, m.Value.Length - (4 + reg.Length) - 1);

            return Regex.Unescape(str);
        }
        public static string GetStrIp(string strValue)
        {
            Match m = Regex.Match(strValue, @"\d+.\d+.\d+.\d+");
            return m.Value;
        }
    }
}
