using ApplicationIO;
using DoDoCung.Properties;
using DoDoCung.Service;
using DoDoCung.Utilities;
using PLC_Communication;
using PLC_Communication.NET.Mitsubishi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;

namespace DoDoCung
{
    public partial class frmDDC : Form
    {
        MCProtocol_3E4E cmd_PLC = new MCProtocol_3E4E(CPUType.MELSEC_Q_L, FrameTypeE.Frame_3E, "192.168.1.30", 5010, System.Net.Sockets.ProtocolType.Tcp) { cmdCode = CmdCode.ASCIICode, WaitDataTimeout=500 };
        string pathFileSetting = AppDomain.CurrentDomain.BaseDirectory + @"FILE\SettingFile.ini";
        string pathFileSpec = AppDomain.CurrentDomain.BaseDirectory + @"FILE\SpecFile.ini";
        short DATA_MODEL = 0;
        private double value_D1 = 0, value_D2 = 0, value_D3 = 0, value_D4 = 0, value_D5 = 0, value_D6 = 0;
        int kQsanpham1;
        int kQsanpham2;
        int kQ_L1;
        int kQ_L2;
        int kQ_L3;

        int kQ_R1;
        int kQ_R2;
        int kQ_R3;
        SerialPort[] ports;
        Label[] statusLabels;
        bool isRestarting = false; // cờ kiểm soát việc khởi động lại
        bool statusReadD1 = false;
        bool statusReadD2 = false;
        bool statusReadD3 = false;
        bool statusReadD4 = false;
        bool statusReadD5 = false;
        bool statusReadD6 = false;
        int SellectMax_Min = 0; // 0 = bỏ chọn, 1 = có chọn
        int totalMeasure = 0; // biến đếm tổng số lần đo
        int OKMeasure = 0;
        int NGMeasure = 0;
        int NGMaxMinMeasure = 0;



        DoCungMeter Meter01, Meter02, Meter03, Meter04, Meter05, Meter06;
        private readonly Dictionary<int, string> modelMapping = new Dictionary<int, string>()
        {
            { 1, "1182" },
            { 2, "3849" },
            { 3, "7768" },
            { 4, "AFFB" },
            { 5, "9181" },
            { 6, "BS" },
            // thêm nếu có nữa
        };

        BindingList<string>ListMahang = new BindingList<string> {};
        double Spec_Max_D1 = 0, Spec_Min_D1 = 0, Spec_Max_D2 = 0, Spec_Min_D2 = 0, Spec_Max_D3 = 0, Spec_Min_D3 = 0, Spec_MaxSubMin=0;
        int Spec_Sodiemdo = 0;
        bool Select_MaxSubMin=false, Select_DoMau=false;


        public frmDDC()
        {
            InitializeComponent();
            this.KeyPreview = true;
            timer1.Enabled = true;
        }




        private void frmDDC_Load(object sender, EventArgs e)
        {
            CheckConnectPLC();
            statusLabels = new Label[] { lbDD1status, lbDD2status, lbDD3status, lbDD4status, lbDD5status, lbDD6status };
            ports = new SerialPort[6];
            InitSerialPort();
            ConnectAllDevices();
            cmd_PLC.WaitDataTimeout = 300;
            cmd_PLC.SocketReceiveTimeout = 100;
            cmd_PLC.SocketSendTimeout = 100;

            CheckForIllegalCrossThreadCalls = false;
            List<string> PortName1 = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };
            cbComL1.DataSource = PortName1;
            List<int> Baudrate1 = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            cbBaudrateL1.DataSource = Baudrate1;
            List<string> PortName2 = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };
            cbComL2.DataSource = PortName2;
            List<int> Baudrate2 = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            cbBaudrateL2.DataSource = Baudrate2;
            List<string> PortName3 = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };
            cbComL3.DataSource = PortName3;
            List<int> Baudrate3 = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            cbBaudrateL3.DataSource = Baudrate3;
            List<string> PortName4 = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };
            cbComR1.DataSource = PortName4;
            List<int> Baudrate4 = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            cbBaudrateR1.DataSource = Baudrate4;
            List<string> PortName5 = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };
            cbComR2.DataSource = PortName5;
            List<int> Baudrate5 = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            cbBaudrateR2.DataSource = Baudrate5;
            List<string> PortName6 = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };
            cbComR3.DataSource = PortName6;
            List<int> Baudrate6 = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
            cbBaudrateR3.DataSource = Baudrate6;

            cbMahangSet.DataSource = ListMahang;
            cbMahangrun.DataSource = ListMahang;

            List<int> CbSodiemdo = new List<int> { 1, 3 };
            cbSodiemdo.DataSource = CbSodiemdo;

            Meter01 = new DoCungMeter(ports[00]);
            Meter02 = new DoCungMeter(ports[01]);
            Meter03 = new DoCungMeter(ports[02]);
            Meter04 = new DoCungMeter(ports[03]);
            Meter05 = new DoCungMeter(ports[04]);
            Meter06 = new DoCungMeter(ports[05]);


        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            
            timer1.Enabled = false;
            short[] _data_int_PLC = new short[20];
            string[] _data_string_plc = cmd_PLC.BatchReads(DataFormat.Word, DeviceCode.D, 4500, 10);
            if (_data_string_plc != null && _data_string_plc.Length == 10)
            {
                _data_int_PLC = cmd_PLC.HexaToArrayInt16(_data_string_plc);
            }


            

            string[] dataHex = cmd_PLC.BatchReads(DataFormat.Word, DeviceCode.D, 4510, 16);
            if (dataHex != null && dataHex.Length > 0)
            {
                string lotName = ConvertPLCWordsToString(dataHex);
                cbLot.Text = lotName; // Hiển thị lên Label
            }
            string[] sttModel = cmd_PLC.BatchReads(DataFormat.Word, DeviceCode.D, 4540, 1);
            if (sttModel != null && sttModel.Length == 1)
            {
                short[] _sttModel = cmd_PLC.HexaToArrayInt16(sttModel);
                DATA_MODEL = _sttModel[0];
                if (modelMapping.TryGetValue(DATA_MODEL, out string modelName))
                {
                    cbMahangrun.Text = modelName; // hiển thị mã hàng tương ứng
                }
                else
                {
                    cbMahangrun.Text = ""; // hoặc ""
                }
            }
            await Doc_XyLy_DoCung(_data_int_PLC[0], _data_int_PLC[2]);

            timer1.Enabled = true;
        }

        //if (_data_int_PLC[0] == 1)
        //    {
        //        GaugesAsync1 = TriggerGaugesAsync1();
        //    }
        //    if (_data_int_PLC[2] == 1)
        //    {
        //        GaugesAsync2 = TriggerGaugesAsync2();
        //    }
        private async Task Doc_XyLy_DoCung(short PLC_0, short PLC_2)
        {
            Task<int> GaugesAsync1 = null;
            Task<int> GaugesAsync2 = null;
            if (PLC_0 == 1)
            {
                GaugesAsync1 = TriggerGaugesAsync1();
            }
            if (PLC_2 == 1)
            {
                GaugesAsync2 = TriggerGaugesAsync2();
            }
            // gom các task không null lại
            var taskList = new List<Task<int>>();
            int[] results = new int[] { };
            if (GaugesAsync1 != null) taskList.Add(GaugesAsync1);
            if (GaugesAsync2 != null) taskList.Add(GaugesAsync2);

            if (taskList.Count > 0)
            {
                results = await Task.WhenAll(taskList);
                // results[0] = result của gauge 1 (nếu có)
                // results[1] = result của gauge 2 (nếu có)
            }

            //Phản hồi Cho PLC kết quả
            if (results[0] == 3) cmd_PLC.BatchWrites(DataFormat.Word, DeviceCode.D, 4500, new ushort[] { 2, 3 });
            else if (results[0] == 1) cmd_PLC.BatchWrites(DataFormat.Word, DeviceCode.D, 4500, new ushort[] { 2, 1 });
            else if (results[0] == 2) cmd_PLC.BatchWrites(DataFormat.Word, DeviceCode.D, 4500, new ushort[] { 2, 2 });

            if (results[1] == 3) cmd_PLC.BatchWrites(DataFormat.Word, DeviceCode.D, 4502, new ushort[] { 2, 3 });
            else if (results[1] == 1) cmd_PLC.BatchWrites(DataFormat.Word, DeviceCode.D, 4502, new ushort[] { 2, 1 });
            else if (results[1] == 2) cmd_PLC.BatchWrites(DataFormat.Word, DeviceCode.D, 4502, new ushort[] { 2, 2 });
        }


        private string ConvertPLCWordsToString(string[] dataHex)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string hexWord in dataHex)
            {
                if (hexWord.Length >= 4)
                {
                    // Tách 2 byte
                    string lowByte = hexWord.Substring(0, 2);
                    string highByte = hexWord.Substring(2, 2);

                    // Chuyển từng byte thành ký tự ASCII
                    char c1 = (char)Convert.ToInt32(highByte, 16);
                    char c2 = (char)Convert.ToInt32(lowByte, 16);

                    sb.Append(c1);
                    sb.Append(c2);
                }
            }

            // Loại bỏ ký tự rỗng hoặc NULL ở cuối (nếu có)
            return sb.ToString().Trim('\0', ' ');
        }
        private async void btTest_Click(object sender, EventArgs e)
        {
            Doc_XyLy_DoCung(1, 1);

        }
        private async Task<int> TriggerGaugesAsync1()  // Hàm chung đo 3 bộ đo — có thể gọi từ PLC hoặc nút Test tay
        {
        
            Task<double?> task_Meter01 = new Task<double?>(() => Meter01._Read_Data(1000));
            Task<double?> task_Meter02 = new Task<double?>(() => Meter02._Read_Data(1000));
            Task<double?> task_Meter03 = new Task<double?>(() => Meter03._Read_Data(1000));

            statusReadD1 = statusReadD2 = statusReadD3 = false;  // Reset trạng thái đọc
            kQ_L1 = kQ_L2 = kQ_L3 = 0;
            lbKq_L1.Text = lbKq_L2.Text = lbKq_L3.Text = "";
            tbKetquaL1.Text = tbKetquaL2.Text = tbKetquaL3.Text = "";
            kQsanpham1 = 0;
            lbKetquaL.Text = "Measure";
            lbKetquaL.ForeColor = Color.Blue;
            totalMeasure++;  // 👉 Tăng số lần đo
            tbTotal.Text = totalMeasure.ToString();
            task_Meter01.Start(); // Gửi lệnh trigger đồng thời
            task_Meter02.Start();
            task_Meter03.Start();
            double?[] uuu= await Task.WhenAll(task_Meter01, task_Meter01, task_Meter01); // Chờ cả 3 cùng hoàn thành (hoặc timeout trong từng ReadGauge)
            if (task_Meter01.Result==null || task_Meter02.Result == null || task_Meter03.Result == null || !ports[0].IsOpen || !ports[1].IsOpen || !ports[2].IsOpen)  // Quá 20 lần mà không nhận được → MISS
            {
                lbKetquaL.Text = "MISS";
                lbKetquaL.ForeColor = Color.Yellow;
                kQsanpham1 = 3;

            }
            else
            {
                double max_D1 = double.Parse(tbMaxD1_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                double min_D1 = double.Parse(tbMinD1_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (value_D1 >= min_D1 && value_D1 < max_D1)
                {
                    kQ_L1 = 1;
                }
                else
                {
                    kQ_L1 = 2;
                }

                double max_D3 = double.Parse(tbMaxD3_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                double min_D3 = double.Parse(tbMinD3_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (value_D3 >= min_D3 && value_D3 < max_D3)
                {
                    kQ_L3 = 1;
                }
                else
                {
                    kQ_L3 = 2;
                }


                double max_D2 = double.Parse(tbMaxD2_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                double min_D2 = double.Parse(tbMinD2_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (value_D2 >= min_D2 && value_D2 < max_D2)
                {
                    kQ_L2 = 1;
                }
                else
                {
                    kQ_L2 = 2;
                }


                if (kQ_L1 == 1 & kQ_L2 == 1 & kQ_L3 == 1)
                {
                    kQsanpham1 = 1;
                }
                else
                {
                    kQsanpham1 = 2;
                }
            }

            UIUpdate_KquaL1L2L3();
            if (kQsanpham1 == 1)
            {
                OKMeasure++;  // 👉 Tăng số lần đo
                tbOk.Text = OKMeasure.ToString();
                lbKetquaL.Text = "OK";
                lbKetquaL.ForeColor = Color.Green;
                
            }
            if (kQsanpham1 == 2)
            {
                NGMeasure++;  // 👉 Tăng số lần đo
                tbNG.Text = NGMeasure.ToString();
                lbKetquaL.Text = "NG";
                lbKetquaL.ForeColor = Color.Red;
                
            }
            
            return kQsanpham1;
        }
        private async Task<int> TriggerGaugesAsync2()  // Hàm chung đo 3 bộ đo — có thể gọi từ PLC hoặc nút Test tay
        {
            Task<double?> task_Meter04 = new Task<double?>(() => Meter04._Read_Data(1000));
            Task<double?> task_Meter05 = new Task<double?>(() => Meter05._Read_Data(1000));
            Task<double?> task_Meter06 = new Task<double?>(() => Meter06._Read_Data(1000));



            statusReadD4 = statusReadD5 = statusReadD6 = false;  // Reset trạng thái đọc
            kQ_R1 = kQ_R2 = kQ_R3 = 0;
            lbKq_R1.Text = lbKq_R2.Text = lbKq_R3.Text = "";
            tbKetquaR1.Text = tbKetquaR2.Text = tbKetquaR3.Text = "";
            kQsanpham2 = 0;
            lbKetquaR.Text = "Measure";
            lbKetquaR.ForeColor = Color.Blue;
            totalMeasure++;  // 👉 Tăng số lần đo
            tbTotal.Text = totalMeasure.ToString();
            task_Meter04.Start(); // Gửi lệnh trigger đồng thời
            task_Meter05.Start();
            task_Meter06.Start();
            double?[] uuu = await Task.WhenAll(task_Meter04, task_Meter05, task_Meter06); // Chờ cả 3 cùng hoàn thành (hoặc timeout trong từng ReadGauge)
            if (task_Meter04.Result == null || task_Meter05.Result == null || task_Meter06.Result == null ||!ports[3].IsOpen || !ports[4].IsOpen || !ports[5].IsOpen)  // Quá 20 lần mà không nhận được → MISS
            {
                lbKetquaR.Text = "MISS";
                lbKetquaR.ForeColor = Color.Yellow;
                kQsanpham2 = 3;
                
            }
            else
            {

                double max_D3 = double.Parse(tbMaxD3_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                double min_D3 = double.Parse(tbMinD3_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (value_D6 > min_D3 && value_D6 < max_D3)
                {
                    kQ_R3 = 1;
                }
                else
                {
                    kQ_R3 = 2;
                }

                double max_D2 = double.Parse(tbMaxD2_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                double min_D2 = double.Parse(tbMinD2_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (value_D5 > min_D2 && value_D5 < max_D2)
                {
                    kQ_R2 = 1;
                }
                else
                {
                    kQ_R2 = 2;
                }

                double max_D1 = double.Parse(tbMaxD1_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                double min_D1 = double.Parse(tbMinD1_Main.Text, System.Globalization.CultureInfo.InvariantCulture);
                if (value_D4 > min_D1 && value_D4 < max_D1)
                {
                    kQ_R1 = 1;
                }
                else
                {
                    kQ_R1 = 2;
                }


                if (kQ_R1 == 1 & kQ_R2 == 1 & kQ_R3 == 1)
                {
                    kQsanpham2 = 1;
                }
                else
                {
                    kQsanpham2 = 2;
                }
            }

            UIUpdate_KquaR1R2R3();

            if (kQsanpham2 == 1)
            {
                OKMeasure++;  // 👉 Tăng số lần đo
                tbOk.Text = OKMeasure.ToString();
                lbKetquaR.Text = "OK";
                lbKetquaR.ForeColor = Color.Green;
                
            }
            if (kQsanpham2 == 2)
            {
                NGMeasure++;  // 👉 Tăng số lần đo
                tbNG.Text = NGMeasure.ToString();
                lbKetquaR.Text = "NG";
                lbKetquaR.ForeColor = Color.Red;
                
            }
            return kQsanpham2;
        }
        private void UIUpdate_KquaL1L2L3()
        {
            tbKetquaL1.Text=value_D1.ToString("0.000");
            tbKetquaL2.Text = value_D2.ToString("0.000");
            tbKetquaL3.Text = value_D3.ToString("0.000");

            int?[] kqValues = { kQ_L1, kQ_L2, kQ_L3 };
            Label[] labels = { lbKq_L1, lbKq_L2, lbKq_L3 };
            for (int i = 0; i < 3; i++)
            {
                if (kqValues[i] == 1)
                {
                    labels[i].ForeColor = Color.Blue;
                    labels[i].Text = "OK";
                }
                if (kqValues[i] == 2)
                {
                    labels[i].ForeColor = Color.Red;
                    labels[i].Text = "NG";
                }
            }
        }
        private void UIUpdate_KquaR1R2R3()
        {
            tbKetquaR1.Text = value_D4.ToString("0.000");
            tbKetquaR2.Text = value_D5.ToString("0.000");
            tbKetquaR3.Text = value_D6.ToString("0.000");

            int?[] kqValues = { kQ_R1, kQ_R2, kQ_R3 };
            Label[] labels = { lbKq_R1, lbKq_R2, lbKq_R3 };
            for (int i = 0; i < 3; i++)
            {
                if (kqValues[i] == 1)
                {
                    labels[i].ForeColor = Color.Blue;
                    labels[i].Text = "OK";
                }
                if (kqValues[i] == 2)
                {
                    labels[i].ForeColor = Color.Red;
                    labels[i].Text = "NG";
                }
            }
        }
        
       
        private void ConnectAllDevices() //kiểm tra kết nối bộ đo
        {
            string Content = "";
            for (int i = 0; i < ports.Length; i++)
            {
                try
                {
                    if (!ports[i].IsOpen)
                    {
                        ports[i].Open();
                        statusLabels[i].Text = "Connected";
                        statusLabels[i].ForeColor = Color.Blue;
                    }
                }
                catch (Exception ex)
                {
                    Content+=$"Lỗi Kết Nối Độ Cứng {i+1} : " + ex.Message+"\r\n";
                    statusLabels[i].Text = "Disonnected";
                    statusLabels[i].ForeColor = Color.Red;
                }
            }
            if(Content!="") MessageBox.Show(Content, "Lỗi Kết Nỗi");
        }
        private void InitSerialPort() // mở port để kết nối bộ đo
        {
            string _TEMP_COM_NAME_L1 = INIFile.READ_iniFile(pathFileSetting, "DD L1", "COM_NAME_L1");   // Load DD 1
            string _TBaudrate_L1 = INIFile.READ_iniFile(pathFileSetting, "DD L1", "Baudrate_L1");
            int _TEMP_Baudrate_L1 = int.Parse(_TBaudrate_L1);
            string _TEMP_COM_NAME_L2 = INIFile.READ_iniFile(pathFileSetting, "DD L2", "COM_NAME_L2");   // Load DD 2
            string _TBaudrate_L2 = INIFile.READ_iniFile(pathFileSetting, "DD L2", "Baudrate_L2");
            int _TEMP_Baudrate_L2 = int.Parse(_TBaudrate_L2);
            string _TEMP_COM_NAME_L3 = INIFile.READ_iniFile(pathFileSetting, "DD L3", "COM_NAME_L3");   // Load DD 3
            string _TBaudrate_L3 = INIFile.READ_iniFile(pathFileSetting, "DD L3", "Baudrate_L3");
            int _TEMP_Baudrate_L3 = int.Parse(_TBaudrate_L1);
            string _TEMP_COM_NAME_R1 = INIFile.READ_iniFile(pathFileSetting, "DD R1", "COM_NAME_R1");   // Load DD 4
            string _TBaudrate_R1 = INIFile.READ_iniFile(pathFileSetting, "DD R1", "Baudrate_R1");
            int _TEMP_Baudrate_R1 = int.Parse(_TBaudrate_R1);
            string _TEMP_COM_NAME_R2 = INIFile.READ_iniFile(pathFileSetting, "DD R2", "COM_NAME_R2");   // Load DD 4
            string _TBaudrate_R2 = INIFile.READ_iniFile(pathFileSetting, "DD R2", "Baudrate_R2");
            int _TEMP_Baudrate_R2 = int.Parse(_TBaudrate_R2);
            string _TEMP_COM_NAME_R3 = INIFile.READ_iniFile(pathFileSetting, "DD R3", "COM_NAME_R3");   // Load DD 4
            string _TBaudrate_R3 = INIFile.READ_iniFile(pathFileSetting, "DD R3", "Baudrate_R3");
            int _TEMP_Baudrate_R3 = int.Parse(_TBaudrate_R3);

            string[] comNames = { _TEMP_COM_NAME_L1, _TEMP_COM_NAME_L2, _TEMP_COM_NAME_L3, _TEMP_COM_NAME_R1, _TEMP_COM_NAME_R2, _TEMP_COM_NAME_R3 };
            int[] baudRates = { _TEMP_Baudrate_L1, _TEMP_Baudrate_L2, _TEMP_Baudrate_L3, _TEMP_Baudrate_R1, _TEMP_Baudrate_R2, _TEMP_Baudrate_R3 };

            for (int i = 0; i < ports.Length; i++) // Gán các giá trị và đăng ký sự kiện
            {
                ports[i] = new SerialPort
                {
                    PortName = comNames[i],
                    BaudRate = baudRates[i],
                    DataBits = 8,
                    Parity = Parity.None,
                    Handshake = Handshake.None,
                    NewLine = "\r\n",
                    Encoding = System.Text.Encoding.ASCII
                };
            }
        }
        private void tabSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabSelect.SelectedTab.TabIndex == 2)
            {
                string _IP_plc = INIFile.READ_iniFile(pathFileSetting, "PLC", "IP_plc");        // Load Setting PLC
                tbIPplc.Text = _IP_plc;
                string _Port_plc = INIFile.READ_iniFile(pathFileSetting, "PLC", "Port_plc");
                tbPortplc.Text = _Port_plc;
                string _COM_NAME_L1 = INIFile.READ_iniFile(pathFileSetting, "DD L1", "COM_NAME_L1");   // Load DD 1
                cbComL1.Text = _COM_NAME_L1;
                string _Baudrate_L1 = INIFile.READ_iniFile(pathFileSetting, "DD L1", "Baudrate_L1");
                cbBaudrateL1.Text = _Baudrate_L1;
                string _COM_NAME_L2 = INIFile.READ_iniFile(pathFileSetting, "DD L2", "COM_NAME_L2");   // Load DD 2
                cbComL2.Text = _COM_NAME_L2;
                string _Baudrate_L2 = INIFile.READ_iniFile(pathFileSetting, "DD L2", "Baudrate_L2");
                cbBaudrateL2.Text = _Baudrate_L2;
                string _COM_NAME_L3 = INIFile.READ_iniFile(pathFileSetting, "DD L3", "COM_NAME_L3");   // Load DD 3
                cbComL3.Text = _COM_NAME_L3;
                string _Baudrate_L3 = INIFile.READ_iniFile(pathFileSetting, "DD L3", "Baudrate_L3");
                cbBaudrateL3.Text = _Baudrate_L3;
                string _COM_NAME_R1 = INIFile.READ_iniFile(pathFileSetting, "DD R1", "COM_NAME_R1");   // Load DD 4
                cbComR1.Text = _COM_NAME_R1;
                string _Baudrate_R1 = INIFile.READ_iniFile(pathFileSetting, "DD R1", "Baudrate_R1");
                cbBaudrateR1.Text = _Baudrate_R1;
                string _COM_NAME_R2 = INIFile.READ_iniFile(pathFileSetting, "DD R2", "COM_NAME_R2");   // Load DD 5
                cbComR2.Text = _COM_NAME_R2;
                string _Baudrate_R2 = INIFile.READ_iniFile(pathFileSetting, "DD R2", "Baudrate_R2");
                cbBaudrateR2.Text = _Baudrate_R2;
                string _COM_NAME_R3 = INIFile.READ_iniFile(pathFileSetting, "DD R3", "COM_NAME_R3");   // Load DD 6
                cbComR3.Text = _COM_NAME_R3;
                string _Baudrate_R3 = INIFile.READ_iniFile(pathFileSetting, "DD R3", "Baudrate_R3");
                cbBaudrateR3.Text = _Baudrate_R3;
            }
        }

        private void btCheckconnect_Click(object sender, EventArgs e)
        {
            CheckConnectPLC();
        }
        private void btSaveST_Click(object sender, EventArgs e)
        {
            INIFile.WRITE_iniFile(pathFileSetting, "PLC", "IP_plc", tbIPplc.Text);        // Save IP PLC
            INIFile.WRITE_iniFile(pathFileSetting, "PLC", "Port_plc", tbPortplc.Text);
            INIFile.WRITE_iniFile(pathFileSetting, "DD L1", "COM_NAME_L1", cbComL1.Text);   // Save COM DD 1
            INIFile.WRITE_iniFile(pathFileSetting, "DD L1", "Baudrate_L1", cbBaudrateL1.Text);
            INIFile.WRITE_iniFile(pathFileSetting, "DD L2", "COM_NAME_L2", cbComL2.Text);   //  Save COM DD 2
            INIFile.WRITE_iniFile(pathFileSetting, "DD L2", "Baudrate_L2", cbBaudrateL2.Text);
            INIFile.WRITE_iniFile(pathFileSetting, "DD L3", "COM_NAME_L3", cbComL3.Text);   // Save COM DD 3
            INIFile.WRITE_iniFile(pathFileSetting, "DD L3", "Baudrate_L3", cbBaudrateL3.Text);
            INIFile.WRITE_iniFile(pathFileSetting, "DD R1", "COM_NAME_R1", cbComR1.Text);   // Save COM DD 4
            INIFile.WRITE_iniFile(pathFileSetting, "DD R1", "Baudrate_R1", cbBaudrateR1.Text);
            INIFile.WRITE_iniFile(pathFileSetting, "DD R2", "COM_NAME_R2", cbComR2.Text);   // Save COM DD 5
            INIFile.WRITE_iniFile(pathFileSetting, "DD R2", "Baudrate_R2", cbBaudrateR2.Text);
            INIFile.WRITE_iniFile(pathFileSetting, "DD R3", "COM_NAME_R3", cbComR3.Text);   // Save COM DD 6
            INIFile.WRITE_iniFile(pathFileSetting, "DD R3", "Baudrate_R3", cbBaudrateR3.Text);
            //DialogResult result = MessageBox.Show("Khởi động lại phần mềm sau khi cài đặt", "ĐÃ LƯU");
            AskForRestart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadListMaHang();
        }



        //================================Xu ly mã hang và spec==================================================

        LocalDatabaseProcess _LocalDatabaseMaHang = new LocalDatabaseProcess();
        DataTable tableMaHang;
        private void ReadListMaHang()
        {
           tableMaHang = _LocalDatabaseMaHang.getData_TableMaHang();
           int countRow = tableMaHang.Rows.Count;
            ListMahang.Clear();
            for (int i=0;i< countRow; i++)
            {
                ListMahang.Add(tableMaHang.Rows[i][1].ToString());
            }
        }

        private void LoadSpecMaHang(int index)
        {

            Spec_Max_D1 = double.Parse(tableMaHang.Rows[index][2].ToString());
            Spec_Min_D1 = double.Parse(tableMaHang.Rows[index][3].ToString());
            Spec_Max_D2 = double.Parse(tableMaHang.Rows[index][4].ToString());
            Spec_Min_D2 = double.Parse(tableMaHang.Rows[index][5].ToString());
            Spec_Max_D3 = double.Parse(tableMaHang.Rows[index][6].ToString());
            Spec_Min_D3 = double.Parse(tableMaHang.Rows[index][7].ToString());
            Spec_Sodiemdo = int.Parse(tableMaHang.Rows[index][8].ToString());
            Spec_MaxSubMin = double.Parse(tableMaHang.Rows[index][9].ToString()); 
            Select_MaxSubMin = bool.Parse(tableMaHang.Rows[index][10].ToString()); 
            Select_DoMau = bool.Parse(tableMaHang.Rows[index][11].ToString());
            tbSpecMaxD1.Text = Spec_Max_D1.ToString();
            tbSpecMinD1.Text = Spec_Min_D1.ToString();
            tbSpecMaxD2.Text = Spec_Max_D2.ToString();
            tbSpecMinD2.Text = Spec_Min_D2.ToString();
            tbSpecMaxD3.Text = Spec_Max_D3.ToString();
            tbSpecMinD3.Text = Spec_Min_D3.ToString();
            tbMax_Min.Text = Spec_MaxSubMin.ToString();
            cbSodiemdo.Text = Spec_Sodiemdo.ToString();
            cbSellectMax_Min.Checked = Select_MaxSubMin;
            
        }



        private void btSavespec_Click(object sender, EventArgs e)
        {
            string maHang = cbMahangSet.Text.Trim();  // Lấy mã hàng người dùng chọn hoặc nhập
            if (string.IsNullOrEmpty(maHang))  // Kiểm tra nếu mã hàng rỗng
            {
                MessageBox.Show("Vui lòng chọn mã hàng trước khi lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Kiểm tra mã hàng có nằm trong danh sách combo không
            bool tonTai = false;
            foreach (var item in cbMahangSet.Items)
            {
                if (item.ToString().Equals(maHang, StringComparison.OrdinalIgnoreCase))
                {
                    tonTai = true;
                    break;
                }
            }
            if (!tonTai)
            {
                MessageBox.Show("Mã hàng không tồn tại trong danh sách! Vui lòng chọn lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Nếu đến đây là hợp lệ → tiến hành lưu dữ liệu
            try
            {
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Max_D1", tbSpecMaxD1.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Min_D1", tbSpecMinD1.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Max_D2", tbSpecMaxD2.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Min_D2", tbSpecMinD2.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Max_D3", tbSpecMaxD3.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Min_D3", tbSpecMinD3.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Sodiemdo", cbSodiemdo.Text);
                INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Spec_Max_Min", tbMax_Min.Text);
                if (SellectMax_Min == 1)
                {
                    INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Select_Check", "1");
                }
                else if (SellectMax_Min == 0) 
                {
                    INIFile.WRITE_iniFile(pathFileSpec, cbMahangSet.Text, "Select_Check", "0");
                }
                AskForRestart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void cbSellectMax_Min_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cbSellectMax_Min.Checked)
            {
                SellectMax_Min = 1;
            }
            else
            {
                SellectMax_Min = 0;
            }
        }
        private void cbMahangSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox bb = (ComboBox)sender;

            LoadSpecMaHang(bb.SelectedIndex);

            

        }

       

        private void cbMahangrun_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _SpecMaxD1 = INIFile.READ_iniFile(pathFileSpec, cbMahangrun.Text, "Spec_Max_D1");   // Load DD 6
            tbMaxD1_Main.Text = _SpecMaxD1;
            string _SpecMinD1 = INIFile.READ_iniFile(pathFileSpec, cbMahangrun.Text, "Spec_Min_D1");   // Load DD 6
            tbMinD1_Main.Text = _SpecMinD1;
            string _SpecMaxD2 = INIFile.READ_iniFile(pathFileSpec, cbMahangrun.Text, "Spec_Max_D2");   // Load DD 6
            tbMaxD2_Main.Text = _SpecMaxD2;
            string _SpecMinD2 = INIFile.READ_iniFile(pathFileSpec, cbMahangrun.Text, "Spec_Min_D2");   // Load DD 6
            tbMinD2_Main.Text = _SpecMinD2;
            string _SpecMaxD3 = INIFile.READ_iniFile(pathFileSpec, cbMahangrun.Text, "Spec_Max_D3");   // Load DD 6
            tbMaxD3_Main.Text = _SpecMaxD3;
            string _SpecMinD3 = INIFile.READ_iniFile(pathFileSpec, cbMahangrun.Text, "Spec_Min_D3");   // Load DD 6
            tbMinD3_Main.Text = _SpecMinD3;
        }
        private void CheckConnectPLC()
        {
            try
            {
                // Gửi lệnh đọc thử 1 word D100 để kiểm tra kết nối
                string[] data = cmd_PLC.BatchReads(DataFormat.Word, DeviceCode.D, 100, 1);

                if (data != null && data.Length > 0)
                {
                    lbPLCstatus.Text = "PLC Connected";
                    lbPLCstatus.ForeColor = Color.Green;
                }
                else
                {
                    lbPLCstatus.Text = "PLC No Response";
                    lbPLCstatus.ForeColor = Color.OrangeRed;
                    timer1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lbPLCstatus.Text = "PLC Disconnected";
                lbPLCstatus.ForeColor = Color.Red;
                timer1.Enabled = false;
                Console.WriteLine("PLC Error: " + ex.Message);
            }
        }
        private void AskForRestart()
        {
            DialogResult results = MessageBox.Show("Bạn hãy khởi động lại phần mềm để áp dụng thay đổi!", "THÔNG BÁO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (results == DialogResult.Yes)
            {
                isRestarting = true; // đánh dấu đang khởi động lại
                Application.Restart(); // Khởi động lại ứng dụng
                Environment.Exit(0); // Đóng ứng dụng hiện tại
            }
            else
            {
                // Không làm gì, chỉ đóng MessageBox
                MessageBox.Show("Thay đổi sẽ được áp dụng sau khi bạn khởi động lại chương trình.", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmDDC_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isRestarting)
            {
                DialogResult bb = MessageBox.Show("Bạn Chắc Chắn Muốn Đóng Ứng Dụng ?", "THÔNG BÁO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (bb == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                // Giống như click nút
                btTest.PerformClick();
                // Nếu không muốn "beep" mặc định
                e.SuppressKeyPress = true;
            }
        }
    }
}
