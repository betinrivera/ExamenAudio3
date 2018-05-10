using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Diagnostics;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Microsoft.Win32;
using System.Windows.Threading;
using NAudio.Dsp;



namespace Grabacion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn waveIn;
        WaveFormat formato;
        WaveFileWriter writer;
        WaveOutEvent output;
        AudioFileReader reader;

        Stopwatch timer = Stopwatch.StartNew();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            formato = waveIn.WaveFormat;



            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;

            writer =
                new WaveFileWriter("sonido2.wav", formato);

            waveIn.StartRecording();
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            writer.Dispose();

        }

        void OnDataAvailable (object sender, WaveInEventArgs e)
        {

            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;


            double acumulador = 0;
            double numMuestras = bytesGrabados / 2;
            int exponente = 1;
            int numeroMuestrasComplejas = 0;
            int bitsMaximos = 0;

            do //1,200
            {
                bitsMaximos = (int) Math.Pow(2, exponente);
                exponente++;
            } while (bitsMaximos < numMuestras);

            //bitsMaximos = 2048
            //exponente = 12

            //numeroMuestrasComplejas = 1024
            //exponente = 10

            exponente -= 2;
            numeroMuestrasComplejas = bitsMaximos / 2;

            Complex[] muestrasComplejas =
                new Complex[numeroMuestrasComplejas];

            for (int i=0; i < bytesGrabados; i+=2)
            {
                // byte i =  0 1 1 0 0 1 1 1
                //byte i+1 = 0 0 0 0 0 0 0 0 0 1 1 0 0 1 1 1
                // or      = 0 1 1 0 0 1 1 1 0 1 1 0 0 1 1 1
                short muestra =
                        (short)Math.Abs((buffer[i + 1] << 8)|buffer[i]);
                //lblMuestra.Text = muestra.ToString();
                //sldVolumen.Value = (double)muestra;

                float muestra32bits = (float)muestra / 32768.0f;
                //sldVolumen.Value = Math.Abs(muestra32bits);

                if (i / 2 < numeroMuestrasComplejas)
                {
                    muestrasComplejas[i / 2].X = muestra32bits;
                }
                //acumulador += muestra;
                //numMuestras++;
            }
            //double promedio = acumulador / numMuestras;
            //sldVolumen.Value = promedio;
            //writer.Write(buffer, 0, bytesGrabados);

            FastFourierTransform.FFT(true, exponente, muestrasComplejas);
            float[] valoresAbsolutos = 
                new float[muestrasComplejas.Length];
            for(int i=0; i <muestrasComplejas.Length; i++)
            {
                valoresAbsolutos[i] = (float)
                    Math.Sqrt((muestrasComplejas[i].X * muestrasComplejas[i].X) +
                    (muestrasComplejas[i].Y * muestrasComplejas[i].Y));

            }

            int indiceMaximo =
                valoresAbsolutos.ToList().IndexOf(
                    valoresAbsolutos.Max());

            float frecuenciaFundamental =
                (float)(indiceMaximo * waveIn.WaveFormat.SampleRate) / (float)valoresAbsolutos.Length;

            lblFrecuencia.Text = frecuenciaFundamental.ToString();





            char letraBuena = '1';

            if (frecuenciaFundamental > 80.0 && frecuenciaFundamental < 120.0)
            {
                timer.Start();
                if (timer.ElapsedMilliseconds == 3000)
                {
                    letraBuena = 'A';
                }
            }
            else if (frecuenciaFundamental > 180.0 && frecuenciaFundamental < 220.0)
                timer.Start();
                if (timer.ElapsedMilliseconds == 3000)
                 {
                letraBuena = 'B';
                    }

            else if (frecuenciaFundamental > 280.0 && frecuenciaFundamental < 320.0)
                timer.Start();
            if (timer.ElapsedMilliseconds == 3000)
            {
                letraBuena = 'C';
            }
            else if (frecuenciaFundamental > 380.0 && frecuenciaFundamental < 420.0)
                timer.Start();
            if (timer.ElapsedMilliseconds == 3000)
            {
                letraBuena = 'D';
            }
            else if (frecuenciaFundamental > 480.0 && frecuenciaFundamental < 520.0)
                timer.Start();
            if (timer.ElapsedMilliseconds == 3000)
            {
                letraBuena = 'E';
            }
            else if (frecuenciaFundamental > 580.0 && frecuenciaFundamental < 620.0)
                timer.Start();
            timer.Stop();
                if (timer.ElapsedMilliseconds == 3000)
                {
                    letraBuena = 'F';
                }
            else if (frecuenciaFundamental > 680.0 && frecuenciaFundamental < 720.0)
            {
                timer.Start();
                if (timer.ElapsedMilliseconds == 3000)
                {
                    letraBuena = 'G';
                }
            }
            else if (frecuenciaFundamental > 780.0 && frecuenciaFundamental < 820.0)
                timer.Start();
            if (timer.ElapsedMilliseconds == 3000)
            {
                letraBuena = 'H';
            }
            else if (frecuenciaFundamental > 880.0 && frecuenciaFundamental < 920.0)
                timer.Start();
            if (timer.ElapsedMilliseconds == 3000)
            {
                letraBuena = 'I';
            }
            else if (frecuenciaFundamental > 980.0 && frecuenciaFundamental < 1020.0)
            {
                timer.Stop();
                if (timer.ElapsedMilliseconds == 3000)
                {
                    letraBuena = 'J';
                }
            }
            else if (frecuenciaFundamental > 1080.0 && frecuenciaFundamental < 1120.0)
            {
                letraBuena = 'K';
            }
            else if (frecuenciaFundamental > 1180.0 && frecuenciaFundamental < 1220.0)
            {
                letraBuena = 'L';
            }
            else if (frecuenciaFundamental > 1280.0 && frecuenciaFundamental < 1320.0)
            {
                letraBuena = 'M';
            }
            else if (frecuenciaFundamental > 1380.0 && frecuenciaFundamental < 1420.0)
            {
                letraBuena = 'N';
            }
            else if (frecuenciaFundamental > 1480.0 && frecuenciaFundamental < 1520.0)
            {
                letraBuena = 'O';
            }
            else if (frecuenciaFundamental > 1580.0 && frecuenciaFundamental < 1620.0)
            {
                letraBuena = 'P';
            }
            else if (frecuenciaFundamental > 1680.0 && frecuenciaFundamental < 1720.0)
            {
                letraBuena = 'Q';
            }
            else if (frecuenciaFundamental > 1780.0 && frecuenciaFundamental < 1820.0)
            {
                letraBuena = 'R';
            }
            else if (frecuenciaFundamental > 1880.0 && frecuenciaFundamental < 1920.0)
            {
                letraBuena = 'S';
            }
            else if (frecuenciaFundamental > 1980.0 && frecuenciaFundamental < 2020.0)
            {
                letraBuena = 'T';
            }
            else if (frecuenciaFundamental > 2080.0 && frecuenciaFundamental < 2120.0)
            {
                letraBuena = 'U';
            }
            else if (frecuenciaFundamental > 2180.0 && frecuenciaFundamental < 2220.0)
            {
                letraBuena = 'V';
            }
            else if (frecuenciaFundamental > 2280.0 && frecuenciaFundamental < 2320.0)
            {
                letraBuena = 'W';
            }
            else if (frecuenciaFundamental > 2380.0 && frecuenciaFundamental < 2420.0)
            {
                letraBuena = 'X';
            }
            else if (frecuenciaFundamental > 2480.0 && frecuenciaFundamental < 2520.0)
            {
                letraBuena = 'Y';
            }
            else if (frecuenciaFundamental > 2580.0 && frecuenciaFundamental < 2620.0)
            {
                letraBuena = 'Z';
            }
            else if (frecuenciaFundamental > 2680.0 && frecuenciaFundamental < 2720.0)
            {
                letraBuena = ' ';
            }



            if (frecuenciaFundamental > 100 && frecuenciaFundamental < 2000)
            {
                lblLetra.Text = letraBuena.ToString();
            }

        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            waveIn.StopRecording();
        }

        

        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            output = new WaveOutEvent();
            reader = new AudioFileReader("sonido2.wav");
            output.Init(reader);
            output.Play();
            timer.Start();

        }
    }
}
