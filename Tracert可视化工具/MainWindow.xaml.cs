using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tracert可视化工具.QueryIpAddr;
using Tracert可视化工具.Tracert;

namespace Tracert可视化工具
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //两个线程,一个是启动Tracert的,一个是查询IP地址的
        Thread m_thTracert = null, m_thIpQuery = null;
        //两个线程对应的类
        CTracert m_cTracert = new CTracert();
        public CQueryIpAddr m_cQueryAps = new CQueryIpAddr();
        //线程状态
        bool m_bProcessState = true;
        public bool ProcessState
        {
            get
            {
                bool bState;
                lock (this) { bState = m_bProcessState; }
                return bState;
            }
            set { lock (this) { m_bProcessState = value; } }
        }

        public MainWindow()
        {
            InitializeComponent();

            m_thTracert = new Thread(new ParameterizedThreadStart(m_cTracert.ThisThread));
            m_thTracert.Start(this);

            m_thIpQuery = new Thread(new ParameterizedThreadStart(m_cQueryAps.ThisThread));
            m_thIpQuery.Start(this);
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            if(checkBox.IsChecked == true)
            {
                textBox_Tracert.Clear();
                textBox_IpAddr.Clear();
            }

            m_cTracert.SetTracertAddr(textBox_Ips.Text);
        }

        public string GetIps()
        {
            string str = null;
            lock (this) { str = textBox_Ips.Text; }
            return str;
        }

        private void button_Start_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ProcessState = false;
            Thread.Sleep(200);
        }
    }
}
