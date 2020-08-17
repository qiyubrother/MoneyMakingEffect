using mshtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Threading;

namespace MoneyMakingEffect
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

            SuppressScriptErrors(web, true);
            web.Navigate("http://stockapp.finance.qq.com/mstats/");

            web.Navigated += (o, ex) => {
                Task.Run(() => {
                    Thread.Sleep(4000);
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        compute();
                    }));
                });
            };

            timer.Interval = new TimeSpan(0, 0, 6);
            timer.Tick += (o, ex) => {
                SuppressScriptErrors(web, true);
                web.Navigate($"http://stockapp.finance.qq.com/mstats/?_={DateTime.Now.Ticks}");
            };
            timer.Start();
        }
        /// <summary>
        /// 在加载页面之前调用此方法设置hide为true就能抑制错误的弹出了。
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="hide"></param>
        static void SuppressScriptErrors(WebBrowser webBrowser, bool hide)
        {
            webBrowser.Navigating += (s, e) =>
            {
                var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (fiComWebBrowser == null)
                    return;

                object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
                if (objComWebBrowser == null)
                    return;

                objComWebBrowser.GetType().InvokeMember("Silent", System.Reflection.BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
            };
        }
        private void compute()
        {
            var doc = web.Document as mshtml.HTMLDocument;
            if (doc == null)
            {
                return;
            }
            var _rise = 0;
            var _fall = 0;
            IHTMLElement shUpElement = doc.getElementById("mktinfo-sh000001-zjs");
            IHTMLElement shDownElement = doc.getElementById("mktinfo-sh000001-djs");
            IHTMLElement szUpElement = doc.getElementById("mktinfo-sz399001-zjs");
            IHTMLElement szDownElement = doc.getElementById("mktinfo-sz399001-djs");

            _rise = Convert.ToInt32(shUpElement.innerHTML) + Convert.ToInt32(szUpElement.innerHTML);
            _fall = Convert.ToInt32(shDownElement.innerHTML) + Convert.ToInt32(szDownElement.innerHTML);

            rise.Content = $"上涨：{_rise} 只";
            fall.Content = $"下跌：{_fall} 只";
            mme.Content = $"赚钱效应：{Math.Round(1.0 * _rise / (_rise + _fall) * 100)}";
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
              new Action(delegate { }));
        }

    }
}
