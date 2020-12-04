using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        public VideoWindow()
        {
            InitializeComponent();

			var currentAssembly = Assembly.GetEntryAssembly();
			var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
			// Default installation path of VideoLAN.LibVLC.Windows
			var libDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
			//"--fullscreen" , "--width=10"
			//"--video-on-top" ,"--width=400", "--height=400", "--no-embedded-video" ,"--no-autoscale"
			this.VlcControl.SourceProvider.CreatePlayer(libDirectory/* pass your player parameters here */, new String[] { "--width=200" });
			
			Uri t = new Uri(currentDirectory + "\\files\\video.mp4");

			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "VideoPlayer.files.video.mp4";

			Stream stream = assembly.GetManifestResourceStream(resourceName);
            this.VlcControl.SourceProvider.MediaPlayer.SetMedia(stream);
            //this.VlcControl.SourceProvider.MediaPlayer.SetMedia(t);

            //VlcMediaPlayer vlcMediaPlayer = this.VlcControl.SourceProvider.MediaPlayer;
            //media = vlcMediaPlayer.GetMedia();
            //media.ParsedChanged += new EventHandler<VlcMediaParsedChangedEventArgs>(Media_ParsedChanged);
            this.VlcControl.SourceProvider.MediaPlayer.Playing += new System.EventHandler<VlcMediaPlayerPlayingEventArgs>(SetProgresMax);
            this.VlcControl.SourceProvider.MediaPlayer.PositionChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs>(this.vlcControl1_PositionChanged);
            //media.ParseAsync();

            this.VlcControl.SourceProvider.MediaPlayer.Play();          


            //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            ////using (StreamReader reader = new StreamReader(stream))
            //{
            //	//string result = reader.ReadToEnd();

            //}

            //this.VlcControl.SourceProvider.MediaPlayer.Play(t);

            //vlcPlayer.SourceProvider.MediaPlayer.VlcLibDirectory =
            ////replace this path with an appropriate one
            //new DirectoryInfo(@"c:\Program Files (x86)\VideoLAN\VLC\");
            //vlcPlayer.MediaPlayer.EndInit();
            //vlcPlayer.MediaPlayer.Play(new Uri("http://download.blender.org/peach/" +
            //	"bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi"));


            //mePlayer.Source = new Uri("files/video.mp4", UriKind.Relative);

            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += timer_Tick;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            //timer.Start();
        }

        /// <summary>
        /// function that handle the progress bar\label current actual time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vlcControl1_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            var vlc = (VlcMediaPlayer)sender;

            Dispatcher.Invoke(new Action(() =>
            {
                pgb.Value = (int)vlc.Time / 1000;
                lblCurrentTime.Content = GetFormatedTime(TimeSpan.FromMilliseconds(vlc.Time));
            }));
        }

        //private void Media_ParsedChanged(object sender, VlcMediaParsedChangedEventArgs e)
        //{
            //var vlc = (VlcMedia)sender;

            //string duration = media.Duration.ToString(@"mm\:ss");
            //string dur = $"{(media.Duration.Hours > 0 ? media.Duration.Hours.ToString() + ":" : String.Empty)}{duration}";
            
            //Dispatcher.Invoke(new Action(() =>
            //{
            //    pgb.Maximum = media.Duration.TotalSeconds;
            //}));            
        //}

        private void SetProgresMax(object sender, VlcMediaPlayerPlayingEventArgs e)
        {
            var vlc = (VlcMediaPlayer)sender;

            if (!progresBarMaxWasSet)
            {
                this.VlcControl.SourceProvider.MediaPlayer.Pause();
                Dispatcher.Invoke(new Action(() =>
                {
                    pgb.Maximum = (int)vlc.Length / 1000;
                    pgb.Value = (int)vlc.Time / 1000;
                    lblCurrentTime.Content = GetFormatedTime(TimeSpan.FromMilliseconds(vlc.Time));
                    lblMaxTime.Content = GetFormatedTime(TimeSpan.FromMilliseconds(vlc.Length));
                    progresBarMaxWasSet = true;
                }));
            }
        }


        //private VlcMedia media;
        private bool mediaPlayerIsPlaying = false;
        private bool progresBarMaxWasSet = false;
        private bool userIsDraggingSliderWhilePlaying = false;
        private bool isFullScreen = false;
        private bool isInControlPanelArea = false;
        private bool timerForHideControlPanelWasStarted = false;

        private DispatcherTimer timer = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(5)            
        };

        private WindowState lastWindowState;


        private void timer_Tick(object sender, EventArgs e)
        {
            if (isFullScreen && !isInControlPanelArea)
            {
                controlPanel.Visibility = Visibility.Hidden;
                this.Cursor = Cursors.None; //hide the mouse cursor
            }
        }

        //private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        //private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //          OpenFileDialog openFileDialog = new OpenFileDialog();
        //          openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
        //          if (openFileDialog.ShowDialog() == true)
        //              mePlayer.Source = new Uri(openFileDialog.FileName);
        //      }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.VlcControl.SourceProvider.MediaPlayer != null;
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Process player = null;
            //string tempFile = "~clip000.mp4";
            //try
            //{
            //    File.WriteAllBytes(tempFile, Properties.Resources.video);
            //    mePlayer.Source = new Uri(tempFile, UriKind.Relative);
            //    //player = Process.Start(tempFile);
            //    //player.WaitForExit();
            //    mePlayer.Play();
            //    mediaPlayerIsPlaying = true;
            //}
            //finally
            //{
            //    //File.Delete(tempFile);
            //}
            
            mediaPlayerIsPlaying = !mediaPlayerIsPlaying;

            if (mediaPlayerIsPlaying)
            {
                this.VlcControl.SourceProvider.MediaPlayer.Play();                
                imgPlayPause.Source = imgPause.Source;
            }
            else
            {
                this.VlcControl.SourceProvider.MediaPlayer.Pause();
                imgPlayPause.Source = imgPlay.Source;
            }
        }

        private void leaveControlArea(object sender, MouseEventArgs e)
        {
            //controlPanel.Visibility = Visibility.Hidden;
            isInControlPanelArea = false;
        }
        private void enterControlArea(object sender, MouseEventArgs e)
        {
            //controlPanel.Visibility = Visibility.Visible;
            isInControlPanelArea = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.VlcControl.SourceProvider.MediaPlayer.Pause();            
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                this.VlcControl.SourceProvider.MediaPlayer.Stop();
            }
            catch (Exception ex)
            {
                var t = "";
            }
            
            //mePlayer.Stop();
            mediaPlayerIsPlaying = false;
        }        


        /// <summary>
        /// this function is used to enable the user to move current time forward\bacword
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pgb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            VlcControl.SourceProvider.MediaPlayer.Time = (long)pgb.Value * 1000;

            if (userIsDraggingSliderWhilePlaying)
            {
                lblCurrentTime.Content = GetFormatedTime(TimeSpan.FromMilliseconds(VlcControl.SourceProvider.MediaPlayer.Time));
                this.VlcControl.SourceProvider.MediaPlayer.Play();
                userIsDraggingSliderWhilePlaying = false;
            }
        }

        private void pgb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (mediaPlayerIsPlaying)
            {
                this.VlcControl.SourceProvider.MediaPlayer.Pause();
                userIsDraggingSliderWhilePlaying = true;
            }
        }

        private string GetFormatedTime(TimeSpan ts)
        {
            string format = string.Empty;
            
            if (ts.Hours > 0)
                format = @"hh\:mm\:ss";
            else
                format = @"mm\:ss";

            return ts.ToString(format);
        
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isFullScreen)
            {
                controlPanel.Visibility = Visibility.Visible;
                this.Cursor = Cursors.Arrow; //dispaly the mouse cursor

                if (timerForHideControlPanelWasStarted)
                    timer.Stop();

                timer.Start();
                timerForHideControlPanelWasStarted = true;
            }                
        }


        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            HandleFullScreen(e);
        }


        private void HandleFullScreen(EventArgs e)
        {
            Image img = null;
            string title = string.Empty;
            isFullScreen = !isFullScreen;

            if (isFullScreen)
            {
                img = imgExitFullScreen;
                title = "Exit full-screen mode";

                lastWindowState = this.WindowState;
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;

                base.OnStateChanged(e);
            }
            else
            {
                img = imgFullScreen;
                title = "View full screen";
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = lastWindowState;
            }

            btnImgFullScreen.Source = img.Source;
            btnFullScreen.ToolTip = title;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11 || (e.Key == Key.Escape && isFullScreen))
            {
                HandleFullScreen(e);
            }
           
        }

        //private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //	mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        //}
    }
}
