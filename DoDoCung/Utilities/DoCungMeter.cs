using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoDoCung.Utilities
{
    internal class DoCungMeter
    {
        SerialPort Port;
        public DoCungMeter(SerialPort _Port)
        {
            Port = _Port;
        }
        public double? _Read_Data(int Wait_time)
        {
            string _ReceDataDC = "";
            double? res = null;
            double value1 = -1;
            int plusIndex = 0;
            Thread ReadDiaThread = new Thread(() =>
            {
                while (true)
                {
                    _ReceDataDC += Port.ReadExisting();
                    if (_ReceDataDC.Length >= 5)
                    {
                        plusIndex = _ReceDataDC.IndexOf('+');
                        if (plusIndex >= 0 && plusIndex < (_ReceDataDC.Length - 2) && _ReceDataDC.Substring(_ReceDataDC.Length - 2, 2) == "\r\n")
                        {
                            string value1_D5 = _ReceDataDC.Substring(plusIndex + 1).Trim(); // Trim() bỏ CR/LF dư thừa
                            value1 = double.Parse(value1_D5, System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                }
            })
            { IsBackground = true };

            //Gửi dữ liệu qua RS232
            try
            {
                if (!Port.IsOpen) Port.Open();
            }
            catch (Exception ee)
            {
                return res;
            }
            //Port.DiscardInBuffer();
            //Port.DiscardOutBuffer();
            Port.WriteLine("D");  // sẽ tự động thêm CRLF vì NewLine = "\r\n"

            ReadDiaThread.Start(); //Start thread cho đọc dữ liệu

            if (!ReadDiaThread.Join(Wait_time)) //chờ đọc dữ liệu xong.
            {
                ReadDiaThread.Abort();
               // throw new InvalidOperationException("Read Data Over Time");
            }
            Console.WriteLine(_ReceDataDC);
            if (value1 > -1) { res = value1; }
            Console.WriteLine("Da XOng)");
            return res;
        }


    }
}
