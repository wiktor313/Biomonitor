using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Biomonitor
{
    public partial class Biomonitor : Form
    {
        private int[] emgBuffer = new int[100];
        private int emgBufferIndex = 0;
        private int activationCount = 0;
        private const int activationThreshold = 500; //do zmiany po analizie sygnału
        Timer portRefreshTimer;
        SerialPort serialPort;
        int time = 0;

        public Biomonitor()
        {
            InitializeComponent();
            SetupChart();
            InitializePortRefreshTimer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PortComboBox.Items.AddRange(SerialPort.GetPortNames());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        // Inicjalizacja timera do odświeżania portów
        private void InitializePortRefreshTimer()
        {
            portRefreshTimer = new Timer();
            portRefreshTimer.Interval = 1000;
            portRefreshTimer.Tick += PortRefreshTimer_Tick;

            portRefreshTimer.Start();
        }

        private void PortRefreshTimer_Tick(object sender, EventArgs e)
        {
            var availablePorts = SerialPort.GetPortNames();
            if (!availablePorts.SequenceEqual(PortComboBox.Items.Cast<string>()))
            {
                PortComboBox.Items.Clear();
                PortComboBox.Items.AddRange(availablePorts);
            }
        }

        // Obsługa portu szeregowego
        private void InitSerial()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
            if (PortComboBox.Text == "")
            {
                MessageBox.Show("Wybierz port!");
                return;
            }
            serialPort = new SerialPort(PortComboBox.Text, 9600);
            serialPort.DataReceived += serialPort1_DataReceived;
            try
            {
                serialPort.Open();
            }
            catch
            {
                MessageBox.Show("Nie można otworzyć portu!");
                return;
            }
            
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();
                if (int.TryParse(data, out int emgValue))
                {
                    this.BeginInvoke(new Action(() => UpdateChart(emgValue)));
                }
            }
            catch (Exception) { }
        }

        // Obsługa wykresu
        private void SetupChart()
        {
            chart1.Series.Clear();
            var series = new Series("EMG");
            series.ChartType = SeriesChartType.Line;
            chart1.Series.Add(series);
            chart1.ChartAreas[0].AxisX.Title = "Czas";
            chart1.ChartAreas[0].AxisY.Title = "Wartość EMG";
            chart1.ChartAreas[0].AxisY.Minimum = 50;
            chart1.ChartAreas[0].AxisY.Maximum = 800;
        }

        private void UpdateChart(int value)
        {
            var series = chart1.Series["EMG"];
            series.Points.AddXY(time++, value);

            // Bufor do obliczeń
            emgBuffer[emgBufferIndex % emgBuffer.Length] = value;
            emgBufferIndex++;

            // Aktywacja mięśnia
            if (value > activationThreshold)
                activationCount++;

            // Statystyki
            double mean = emgBuffer.Take(Math.Min(emgBufferIndex, emgBuffer.Length)).Average();
            int max = emgBuffer.Take(Math.Min(emgBufferIndex, emgBuffer.Length)).Max();
            int min = emgBuffer.Take(Math.Min(emgBufferIndex, emgBuffer.Length)).Min();

            labelAvg.Text = $"Średnia: {mean:F2}";
            labelMax.Text = $"Maksimum: {max}";
            labelMin.Text = $"Minimum: {min}";
            labelActivations.Text = $"Aktywacje: {activationCount}";

            // Wykres
            if (series.Points.Count > 200)
            {
                series.Points.RemoveAt(0);
                chart1.ChartAreas[0].AxisX.Minimum = time - 200;
                chart1.ChartAreas[0].AxisX.Maximum = time;
            }
            progressBar1.Value = Math.Min(value, progressBar1.Maximum);
        }

        // Obsługa zdarzeń przycisków
        private void button1_Click_1(object sender, EventArgs e)
        {
            InitSerial();
        }
    }
}
