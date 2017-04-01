using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tracert可视化工具;

namespace Tracert可视化工具.Tracert
{
    class CTracert
    {
        //tracert进程
        Process m_cTracertProcess = null;
        //当前跟踪的地址
        string m_strCurAddr = null;
        MainWindow win = null;

        public void ThisThread(object obj)
        {
            win = obj as MainWindow;
            while (win.ProcessState)
            {
                if(m_cTracertProcess != null)
                {
                    //如果进程在运行状态,随时检测是否已经退出
                    if(m_cTracertProcess.HasExited)
                    {
                        m_cTracertProcess.Close();
                        m_cTracertProcess = null;
                    }

                    m_strCurAddr = null;

                    Thread.Sleep(500);
                    continue;
                }

                if((m_cTracertProcess == null && m_strCurAddr == null))
                {
                    Thread.Sleep(500);
                    continue;
                }

                m_cTracertProcess = new Process();
                ProcessStartInfo info = m_cTracertProcess.StartInfo;
                //启动的进程名字
                info.FileName = "tracert";
                //启动参数
                Action action1 = () =>{ info.Arguments = " -d " + win.textBox_Ips.Text; };
                win.textBox_Ips.Dispatcher.Invoke(action1);
                //MessageBox.Show(info.Arguments);
                //不显示窗口
                info.CreateNoWindow = true;
                //重定向输出
                info.RedirectStandardOutput = true;
                //重定向输入
                info.RedirectStandardOutput = true;
                //要重定向 IO 流，Process 对象必须将 UseShellExecute 属性设置为 False。
                info.UseShellExecute = false;

                //重定向到的函数,直接定义到主界面线程上去,就不用跨线程设置输出了
                m_cTracertProcess.OutputDataReceived += TracertOut;
                m_cTracertProcess.Start();
                m_cTracertProcess.BeginOutputReadLine();
            }

            if (m_cTracertProcess != null)
            {
                m_cTracertProcess.Kill();
                m_cTracertProcess.Close();
                m_cTracertProcess = null;
            }
        }

        //主线程设置要跟踪的地址
        public void SetTracertAddr(string str)
        {
            lock(this)
            {
                //图方便就写在这里吧
                if(m_cTracertProcess != null)
                {
                    m_cTracertProcess.Kill();
                    m_cTracertProcess.Close();
                    m_cTracertProcess = null;
                }

                m_strCurAddr = str;
            }
        }

        public void TracertOut(object sender, DataReceivedEventArgs e)
        {
            string strLine = e.Data;
            if (strLine != null && strLine.Length > 0)
            {
                try
                {
                    win.Dispatcher.Invoke(new Action(() =>
                    {
                        win.textBox_Tracert.Text += (strLine + "\n");
                        win.textBox_Tracert.SelectionStart = win.textBox_Tracert.Text.Length;

                        //先确定是不是跳跃点信息,看看第三个字符是不是数字就好了
                        if (strLine[2] >= '0' && strLine[2] <= '9')
                        {
                            //解析文字里的IP地址
                            string strIp = TagInfo.GetStrIp(strLine);
                            if (strIp.Length > 0)
                                win.m_cQueryAps.PushIPToList(strIp);
                            //textBox_IpAddr.Text += (strIp + "\n");
                        }
                    }));
                }
               catch
               { }
            }
        }
    }
}