using System;
using System.Activities;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.Win32;

namespace up_lab_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string filePath = "sound.wav";
        private Device dSound;
        private SecondaryBuffer sound;
        private BufferDescription buffDescr = new BufferDescription();

        private IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            dSound = new Device(); //c# directsound new device program stopped working 
            dSound.SetCooperativeLevel(handle, CooperativeLevel.Priority);

            //buffDescr.ControlPan = true;
            //buffDescr.ControlVolume = true;
            //buffDescr.ControlFrequency = true;
            //buffDescr.ControlEffects = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Audio Files (.wav)|*.wav";

            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
            }
        }
    }
}
