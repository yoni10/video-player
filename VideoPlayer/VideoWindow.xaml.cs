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
			//timer.Interval = TimeSpan.FromSeconds(1);
			//timer.Tick += timer_Tick;
			//timer.Start();
		}

        private bool mediaPlayerIsPlaying = false;
        //private bool userIsDraggingSlider = false;


        //private void timer_Tick(object sender, EventArgs e)
        //{
        //	if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
        //	{
        //		sliProgress.Minimum = 0;
        //		sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
        //		sliProgress.Value = mePlayer.Position.TotalSeconds;
        //	}
        //}

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
            this.VlcControl.SourceProvider.MediaPlayer.Play();
            mediaPlayerIsPlaying = true;

        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //mePlayer.Pause();
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

        //private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        //{
        //	userIsDraggingSlider = true;
        //}

        //private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        //{
        //	userIsDraggingSlider = false;
        //	mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        //}

        //private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //	lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        //}

        //private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //	mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        //}
    }
}
