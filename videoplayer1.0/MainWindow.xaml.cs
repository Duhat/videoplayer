using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Threading;
using TagLib;

namespace videoplayer1._0
{
    public partial class MainWindow : Window
    { 
        
        string videoFilePath = "";
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval =TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timeSlider.Value = videoPlayer.Position.TotalMilliseconds;
        }

       

        private void OpenVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files|*.mp4;*.avi;*.mov"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                videoPlayer.Source = new Uri(openFileDialog.FileName);
                videoFilePath = openFileDialog.FileName;

                string fileName = GetFileName(videoFilePath);
                titleTextBlock.Text = fileName;

                string comment = GetFileComment(videoFilePath);
                commentTextBlock.Text = comment ?? "Комментарий отсутствует";

                mediaContol();
                videoPlayer.Play();

            }
        }

        private void PauseVideo_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Pause();
        }

        private void PlayVideo_Click(object sender, RoutedEventArgs e)
        {
            mediaContol();
            videoPlayer.Play();
        }

        #region getInfo

        public static string GetFileComment(string filePath)
        {
            try
            {
                var videoFile = TagLib.File.Create(filePath);
                return videoFile.Tag.Comment;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении комментария: {ex.Message}");
                return null;
            }
        }

        private string GetFileName(string filePath)
        {
            return System.IO.Path.GetFileName(filePath);
        }

        #endregion


        #region slider videoControl
        private void mediaContol()
        {
            videoPlayer.Volume = (double)volumeSlider.Value;
            videoPlayer.SpeedRatio = (double)speedSlider.Value;
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            videoPlayer.Volume = (double)volumeSlider.Value;
        }
        private void speedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            videoPlayer.SpeedRatio = (double)speedSlider.Value;
        }
        private void timeSlider_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            videoPlayer.Position = TimeSpan.FromMilliseconds(timeSlider.Value);
            videoPlayer.Play();
            timer.Start();
        }
        private void timeSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            videoPlayer.Pause();
            timer.Stop();
        }
        #endregion

        #region update info

        private string RenameFile(string newFileName)
        {
            if (string.IsNullOrWhiteSpace(newFileName))
            {
                MessageBox.Show("Введите новое имя файла.");
                return null;
            }

            try
            {
                string directory = System.IO.Path.GetDirectoryName(videoFilePath);
                string newFilePath = System.IO.Path.Combine(directory, newFileName + System.IO.Path.GetExtension(videoFilePath));

                // Проверяем, существует ли файл с новым именем
                if (System.IO.File.Exists(newFilePath))
                {
                    MessageBox.Show("Файл с таким именем уже существует. Пожалуйста, выберите другое имя.");
                    return null;
                }

                // Переименовываем файл
                System.IO.File.Move(videoFilePath, newFilePath);
                return newFilePath; // Возвращаем новый путь к файлу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переименовании файла: {ex.Message}");
                return null;
            }
        }

        private void ReWriteFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(videoFilePath) || !System.IO.File.Exists(videoFilePath))
            {
                MessageBox.Show("Файл не выбран или не существует.");
                return;
            }

            try
            {
                videoPlayer.Source = null;
                var videoFile = TagLib.File.Create(videoFilePath);
                videoFile.Tag.Title = titleTextBox.Text;
                videoFile.Tag.Comment = commentTextBox.Text;
                videoFile.Save();

                // Переименовываем файл
                string newFilePath = RenameFile(titleTextBox.Text);

                if (newFilePath != null)
                {
                    videoFilePath = newFilePath; // Обновляем путь к файлу
                    MessageBox.Show("Метаданные успешно обновлены!");
                    videoPlayer.Source = new Uri(videoFilePath); // Обновляем источник плеера
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи метаданных: {ex.Message}");
            }
        }
        #endregion


        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e) //размер медиалемента
        {
            mediaContol();
            timeSlider.Maximum = videoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;



            var videoWidth = videoPlayer.NaturalVideoWidth;
            var videoHeight = videoPlayer.NaturalVideoHeight;

            // Устанавливаем размеры окна в зависимости от видео
            this.Width = Math.Max(Math.Min(videoWidth + 50, 1920), 320); // Ограничиваем ширину
            this.Height = Math.Max(Math.Min(videoHeight + 100, 1080), 240); // Ограничиваем высоту
        }


        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e) { 
        
            videoPlayer.Stop();
        }

        
    }
}
