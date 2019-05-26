using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace SerialComm
{
    public partial class Form1 : Form
    {
        char[] RXbuffer;
        byte[] intToBCD = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
        double speed = 0;
        int step = 0;

        public Form1()
        {
            InitializeComponent();
            getAvailablePorts();
        }
        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    textBox2.Text = "Port Settings Not Selected!";
                }
                else
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Open();
                    if (serialPort1.IsOpen)
                    {
                        progressBar1.Value = 100;
                        //Send & Close Port buttons disabled
                        button1.Enabled = true;
                        button4.Enabled = true;
                        textBox1.Enabled = true;
                        //Open Port button disabled
                        button3.Enabled = false;
                        //textBox2.Enabled = true;
                        timer1.Interval = 1;
                        timer1.Start();
                    }
                }
            }
            catch(UnauthorizedAccessException)
            {
                textBox2.Text = "Unauthorized Access!";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            timer1.Stop();
            progressBar1.Value = 0;
            //Send & Close Port buttons disabled
            button1.Enabled = false;
            button4.Enabled = false;
            //Open Port button enabled
            button3.Enabled = true;
            textBox1.Enabled = false;
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine(textBox1.Text);
            //Clear the output textBox
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.BytesToRead != 0)
            try
            {
                    int temp = serialPort1.ReadByte();
                    if (temp == 60) step = 1;
                    else
                    {
                        switch (step)
                        {
                            case 1:
                                if (Convert.ToBoolean(temp & 0x01)) {
                                    step++;
                                    speed = 0;
                                } 
                                break;
                            case 2:
                                speed += temp;
                                step++;
                                break;
                            case 3:
                                speed += 0.1 * Convert.ToDouble((temp & 0x00F0) >> 4);
                                speed += 0.01 * Convert.ToDouble(temp & 0x000F);
                                step++;
                                break;
                            case 4:
                                if (temp == 62)
                                    textBox3.Text = Convert.ToString(speed);
                                break;
                            default:
                                break;
                        }
                    }
                    textBox2.Text += Convert.ToChar(temp);
            }
            catch (TimeoutException)
            {
                textBox2.Text = "Timeout Exception";
            }
        }
    }
}
