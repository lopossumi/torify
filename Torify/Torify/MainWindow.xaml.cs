using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Notifications.Wpf.Core;
using Timer = System.Timers.Timer;

namespace Torify
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<SalesItem> items;
        public List<SalesItem> Items
        {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        
        private readonly NotificationManager notificationManager;
        private readonly Timer timer;
        private readonly RequestParser parser;

        private string keyword = "";
        public string Keyword { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            notificationManager = new NotificationManager();
            parser = new RequestParser();
            Items = RequestItems();
            
            timer = new Timer() { AutoReset = true, Interval = 1000 * 60, Enabled = true };
            timer.Elapsed += TimerOnElapsed;
            TimerOnElapsed(this, null);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            timer.Elapsed -= TimerOnElapsed;
            timer.Dispose();
            Application.Current.Shutdown();
        }

        private List<SalesItem> RequestItems()
        {
            if (keyword == "") return new List<SalesItem>();

            var request = WebRequest.Create($"https://www.tori.fi/koko_suomi?q={keyword}&st=s");
            var data = request.GetResponse().GetResponseStream();

            using (var sr = new StreamReader(data, Encoding.GetEncoding("iso-8859-1")))
            {
                var html = sr.ReadToEnd();
                return parser.ParseItems(html);
            }
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var newItems = RequestItems();
            if (!newItems.Any()) return;

            var firstNewItem = newItems.First();
            if (firstNewItem.Equals(Items.First())) { return; }

            Items = newItems;
            await notificationManager.ShowAsync(
                new NotificationContent
                {
                    Title = "New item for sale",
                    Message = $"{firstNewItem.Description} - {firstNewItem.Price}€",
                    Type = NotificationType.Success
                },
                expirationTime: TimeSpan.FromMinutes(30),
                onClick: () =>
                {
                    OpenInBrowser(firstNewItem.LinkUri);
                });
        }

        private void OpenInBrowser(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;
            var psi = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c start {uri}",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(psi);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenInBrowser(((SalesItem) ((DataGridRow) sender).DataContext).LinkUri);
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            keyword = Keyword.Trim().Replace(" ", "+");
            Items = RequestItems();
        }

        private void KeywordBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Keyword = ((TextBox) sender).Text;
                OnSearchButtonClick(sender, e);
            }
        }
    }
}
