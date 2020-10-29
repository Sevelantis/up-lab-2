using System;
using System.Windows;
using Microsoft.Win32;
using NAudio.Wave;
using System.Windows.Interop;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Generic;

namespace up_lab_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string filePath = "E:\\SZKOLA\\aaaSEMESTR - 5\\UP\\Lab2\\up-lab-2\\sounds\\sound.wav";
        private string savePath = null;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFileReader;
        private WMPLib.WindowsMediaPlayer wmplayer;
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        private List<WaveInCapabilities> inputDevices = new List<WaveInCapabilities>();


        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            audioFileReader = new AudioFileReader(filePath);
            soundPathLabel.Content = "Sound path: " + filePath;
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFileReader);

            //update input devices combo box
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                inputDevices.Add(WaveIn.GetCapabilities(n));
            }

            InputDeviceCmbBox.ItemsSource = inputDevices;

            waveSource = new WaveIn();
            if (WaveIn.DeviceCount > 0)
            {
                waveSource.DeviceNumber = 0;
                InputDeviceCmbBox.SelectedIndex = 0;
            }

            StopRecordingButton.IsEnabled = false;
        }
        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Audio Files (.wav)|*.wav";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.FileName;
                soundPathLabel.Content = "Sound path: " + filePath;
                audioFileReader = new AudioFileReader(filePath);
                outputDevice = null;
            }
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            if(filePath != null)
            {
                PlaySound();
            }
        }

        private void PlaySound()
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFileReader);
            }
            outputDevice.Play();
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
                audioFileReader = new AudioFileReader(filePath);
            }
        }

        private void OnPauseButtonClick(object sender, RoutedEventArgs e)
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
            }
        }

        private void OnWavHeaderButtonClick(object sender, RoutedEventArgs e)
        {
            if (!filePath.Contains(".wav")) return;

            string AverageBytesPerSecond = "Average Bytes Per Second: " + audioFileReader.WaveFormat.AverageBytesPerSecond.ToString();
            string SampleRate = "Sample rate: " + audioFileReader.WaveFormat.SampleRate.ToString();
            string Channels = "Channels: " + audioFileReader.WaveFormat.Channels.ToString();
            string ExtraSize = "Extra Size: " + audioFileReader.WaveFormat.ExtraSize.ToString();
            string BitsPerSample = "Bits Per Sample: " + audioFileReader.WaveFormat.BitsPerSample.ToString();

            WavHeaderLabel.Content = AverageBytesPerSecond + "\n" + SampleRate + "\n" + Channels + "\n" + ExtraSize + "\n" + BitsPerSample;
        }

        private void OnWmpPlayClick(object sender, RoutedEventArgs e)
        {
            if (wmplayer == null)
            {
                wmplayer = new WMPLib.WindowsMediaPlayer();
                wmplayer.URL = filePath;
            }
            wmplayer.controls.play();
        }

        private void OnWmpPauseClick(object sender, RoutedEventArgs e)
        {
            wmplayer.controls.pause();
        }

        private void OnWmpStopClick(object sender, RoutedEventArgs e)
        {
            wmplayer.controls.stop();
            wmplayer = null;
        }

        //----------------------------MICROPHONE -------------------------------
        private void OnStartRecordButtonClick(object sender, RoutedEventArgs e)
        {
            if (!StartRecordingButton.IsEnabled) return;

            if (savePath == null) ChooseSaveDir();

            updateAudioInputDevice();

            waveSource.WaveFormat = new WaveFormat(44100, waveSource.WaveFormat.Channels);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(savePath + "/recorded.wav", waveSource.WaveFormat);

            //record
            System.Windows.MessageBox.Show("PRESS OK TO RECORD");
            StartRecordingButton.IsEnabled = false;
            StopRecordingButton.IsEnabled = true;
            waveSource.StartRecording();
        }

        private void OnStopRecordButtonClick(object sender, RoutedEventArgs e)
        {
            if (!StopRecordingButton.IsEnabled) return;

            StartRecordingButton.IsEnabled = true;
            StopRecordingButton.IsEnabled = false;

            waveSource.StopRecording();
        }
        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
        }

        private void InputDeviceCmbBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            updateAudioInputDevice();
        }

        private void updateAudioInputDevice()
        {
            if (WaveIn.DeviceCount > 0)
            {
                if (waveSource == null) waveSource = new WaveIn();
                waveSource.DeviceNumber = InputDeviceCmbBox.SelectedIndex;
            }
        }

        private void OnShowInExplorerClick(object sender, RoutedEventArgs e)
        {
            ChooseSaveDir();
        }

        private void ChooseSaveDir()
        {
            //choose save dir
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                savePath = dialog.SelectedPath;
                SavePathLabel.Content = "save path = "+savePath;
            }
        }
        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            waveFile = null;
            outputDevice = null;
            audioFileReader = null;
            wmplayer = null;
            filePath = null;
            soundPathLabel.Content = "Sound path: ";

            //init output
            //audioFileReader = new AudioFileReader(filePath);
            //soundPathLabel.Content = "Sound path: " + filePath;
            //outputDevice = new WaveOutEvent();
            //outputDevice.Init(audioFileReader);
        }
    }

}
