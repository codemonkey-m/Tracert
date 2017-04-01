using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tracert可视化工具.QueryIpAddr
{
    public class CQueryIpAddr
    {

        LinkedList<string> m_vtIps = new LinkedList<string>();
        public void PushIPToList(string strIp)
        {
            lock (this) { m_vtIps.AddLast(strIp); }
        }

        public void ThisThread(object obj)
        {
            MainWindow win = obj as MainWindow;
            while (win.ProcessState)
            {
                if (m_vtIps.Count <= 0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                string strIp = TagInfo.GetStrIp(m_vtIps.First());
                m_vtIps.RemoveFirst();

                if (strIp.Length <= 0)
                    continue;

                //请求http
                string httpaddr = "http://ip.taobao.com/service/getIpInfo.php?ip=" + strIp;
                string httpdata = TagInfo.Get(httpaddr);

                //从中解析出地址信息
                string strAddr = TagInfo.GetJsonData(httpdata, "country") + TagInfo.GetJsonData(httpdata, "region") + TagInfo.GetJsonData(httpdata, "city") + TagInfo.GetJsonData(httpdata, "county") + TagInfo.GetJsonData(httpdata, "isp");
                Action action1 = () => { win.textBox_IpAddr.Text += (strIp + " ==> " + strAddr + "\n"); };
                win.textBox_IpAddr.Dispatcher.BeginInvoke(action1);
            }
        }
    }
}
