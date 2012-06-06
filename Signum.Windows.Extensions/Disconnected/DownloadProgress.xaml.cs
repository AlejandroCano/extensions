﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Signum.Entities.Disconnected;
using Signum.Entities;
using Signum.Utilities;
using Signum.Entities.Authorization;
using System.Windows.Threading;
using System.IO;
using System.IO.Compression;

namespace Signum.Windows.Disconnected
{
    /// <summary>
    /// Interaction logic for DownloadProgress.xaml
    /// </summary>
    public partial class DownloadProgress : Window
    {
        public DownloadProgress()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(DownloadDatabase_Loaded);

            this.machine = DisconnectedClient.CurrentDisconnectedMachine;
        }

        DisconnectedExportDN estimation;

        Lite<DisconnectedExportDN> currentLite;

        Lite<DisconnectedMachineDN> machine;

        IDisconnectedTransferServer transferServer = DisconnectedClient.GetTransferServer();

        DispatcherTimer timer = new DispatcherTimer();

        void DownloadDatabase_Loaded(object sender, RoutedEventArgs e)
        {
            estimation = Server.Return((IDisconnectedServer ds) => ds.GetDownloadEstimation(machine));

            pbGenerating.Minimum = 0;
            pbGenerating.Maximum = 1;

            currentLite = transferServer.BeginExportDatabase(UserDN.Current.ToLite(), machine);

            timer.Tick += new EventHandler(timer_Tick);

            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            var current = currentLite.RetrieveAndForget();

            ctrlStats.DataContext = null;
            ctrlStats.DataContext = current;

            if (current.State != DisconnectedExportState.InProgress)
            {
                timer.Stop();

                if (current.State == DisconnectedExportState.Error)
                {
                    expander.IsExpanded = true;
                    pbGenerating.IsIndeterminate = false;

                    if (MessageBox.Show(Window.GetWindow(this), "There have been an error. View Details?", "Error exporting database", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        Navigator.View(current.Exception);
                }
                else
                {
                    pbGenerating.Value = 1;

                    StartDownloading();
                }
            }
            else
            {
                if (estimation == null)
                    pbGenerating.IsIndeterminate = true;
                else
                    pbGenerating.Value = current.Ratio(estimation);
            }
        }

        private void StartDownloading()
        {
            var file = transferServer.EndExportDatabase(new DownloadDatabaseRequests
            {
                User = UserDN.Current.ToLite(),
                DownloadStatistics = currentLite
            });

            pbDownloading.Minimum = 0;
            pbDownloading.Maximum = file.Length;

            using (var ps = new ProgressStream(file.Stream))
            {
                ps.ProgressChanged += (s, args) => pbDownloading.Value = ps.Position;

                using (FileStream fs = File.OpenWrite(DisconnectedClient.DownloadBackupFile))
                    ps.CopyTo(fs);
            }

            MessageBox.Show(Window.GetWindow(this), "You have successfully downloaded a local database. \r\nThe application will turn off now.\r\nNext time you start it up, choose LocalDB.", "Download complete", MessageBoxButton.OK);

            Environment.Exit(0);
        }
    }
}
