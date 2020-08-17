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
                web.Navigate($"http://stockapp.finance.qq.com/mstats/?_={DateTime.Now.Ticks}");
            };
            timer.Start();
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
