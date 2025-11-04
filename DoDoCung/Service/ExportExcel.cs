using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DoDoCung.Service
{
    public class ExportExcel
    {


        private static void CheckAndCreateFile(DataTable dataLens_table, string filePath)
        {

            using (ExcelPackage p = new ExcelPackage())
            {
                // đặt tên người tạo file
                p.Workbook.Properties.Author = "CNCVINA";

                // đặt tiêu đề cho file
                p.Workbook.Properties.Title = "Tổng hợp Thông số";

                //Tạo một sheet để làm việc trên đó
                p.Workbook.Worksheets.Add("Data");
                // lấy sheet vừa add ra để thao tác
                ExcelWorksheet ws = p.Workbook.Worksheets[1];

                // đặt tên cho sheet
                ws.Name = "Data";
                // fontsize mặc định cho cả sheet
                ws.Cells.Style.Font.Size = 11;
                // font family mặc định cho cả sheet
                ws.Cells.Style.Font.Name = "Calibri";
                //SET độ rộng cac cột
                ws.Column(1).Width = 7;
                ws.Column(2).Width = 15;
                ws.Column(3).Width = 25;
                ws.Column(4).Width = 10;
                ws.Column(5).Width = 10;
                ws.Column(6).Width = 10;
                ws.Column(7).Width = 10;

                
                ws.Cells[1, 1, 1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, 1, 1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells[1, 1, 1, 7].Style.Font.Bold = true;// in đậm
                ws.Cells[1, 1, 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;// căn giữa
                var border1 = ws.Cells[1, 1, 1, 7].Style.Border;
                border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;

                // Tạo header cho File Excel
                setHeader(dataLens_table, ws, 1, 1);

                /*============================Lưu file lại========================*/

                p.SaveAs(new FileInfo( filePath));

            }

        }
        public static void Write1LineData(DataTable dataLens_table, string filePath, string CounterProduction, string MaHangCurrent,string ResultTruc2, string value_D4, string value_D5, string value_D6)
        {

            if (File.Exists(filePath))
            {
                Console.WriteLine("✅ File đã tồn tại");
            }
            else
            {
                Console.WriteLine("❌ File chưa tồn tại");
                CheckAndCreateFile(dataLens_table, filePath);
            }

            using (ExcelPackage p = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                // Tìm dòng cuối cùng có dữ liệu
                int lastRow = ws.Dimension?.End.Row ?? 0;
                // Dòng mới cần ghi
                int newRow = lastRow + 1;

                //Chuyển dữ liệu từ DataTable vào mảng đối tượng
                string[,] arr = new string[,] { { CounterProduction, MaHangCurrent, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ResultTruc2, value_D4, value_D5, value_D6 } };
               
                int obj_rows = arr.Length; // số hàng
                //Thiết lập vùng điền dữ liệu
                int rowStart = newRow;
                int columnStart = 1;
                int rowEnd = newRow;
                int columnEnd = obj_rows;

                //thiết lập giá trị vào vùng range đã chọn trong Excel
                ExcelRange range01 = ws.SelectedRange[rowStart, columnStart, rowEnd, columnEnd];
                range01.Value = arr;

                /*============================Lưu file lại========================*/
                p.Save();

            }

        }

        public void exportData(DataTable dataLens_table)
        {
            string filePath = "";
            // tạo SaveFileDialog để lưu file excel
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Export File Excel Data";
            // chỉ lọc ra các file có định dạng Excel
            dialog.Filter = "Excel | *.xlsx | Excel 2003 | *.xls";

            // Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath = dialog.FileName;
                // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Đường dẫn báo cáo không hợp lệ", "Thông Báo");
                    return;
                }
            }
            else { return; }


            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "CNCVINA";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "Tổng hợp Thông số";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("Data");
                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];

                    // đặt tên cho sheet
                    ws.Name = "Data";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";
                    //SET độ rộng cac cột
                    ws.Column(1).Width = 8;
                    ws.Column(2).Width = 15;
                    ws.Column(3).Width = 10;
                    ws.Column(4).Width = 10;
                    ws.Column(5).Width = 10;
                    ws.Column(6).Width = 10;
                    ws.Column(7).Width = 10;

                    //PRESCRIPTION
                    ws.Cells[3, 8].Value = "PRESCRIPTION";
                    ws.Cells[3, 8, 3, 12].Merge = true; // có thể viết theo chỉ số string ws.Cells["C8:C12"].Merge = true;
                    ws.Cells[3, 8, 3, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[3, 8, 3, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[3, 8, 3, 12].Style.Font.Bold = true;// in đậm
                    ws.Cells[3, 8, 3, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;// căn giữa
                    var border1 = ws.Cells[3, 8, 3, 11].Style.Border;
                    border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;

                    // Tạo header cho File Excel
                    setHeader(dataLens_table, ws, 1, 1);
                    //Chuyển dữ liệu từ DataTable vào mảng đối tượng
                    object[,] arr = new object[dataLens_table.Rows.Count, dataLens_table.Columns.Count];
                    for (int r = 0; r < dataLens_table.Rows.Count; r++)
                    {
                        DataRow dr = dataLens_table.Rows[r];
                        for (int c = 0; c < dataLens_table.Columns.Count; c++)
                        {
                            arr[r, c] = dr[c];
                        }
                    }
                    int obj_rows = arr.GetLength(0); // số hàng (chiều 0)
                    int obj_cols = arr.GetLength(1); // số cột (chiều 1)
                                                     //Thiết lập vùng điền dữ liệu
                    int rowStart = 2;
                    int columnStart = 1;
                    int rowEnd = rowStart + obj_rows - 1;
                    int columnEnd = obj_cols;


                    //thiết lập giá trị vào vùng range đã chọn trong Excel
                    ExcelRange range01 = ws.SelectedRange[rowStart, columnStart, rowEnd, columnEnd];
                    range01.Value = arr;

                    /*============================Lưu file lại========================*/
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);

                }

                MessageBox.Show("Xuất excel thành công!", "Thông Báo");
            }
            catch (Exception EE)
            {
                MessageBox.Show("Có lỗi khi lưu file!\nException: " + EE.Message, "Thông báo");
            }
        }

        private static void setHeader(DataTable dt, ExcelWorksheet ws, int colIndex_Start, int rowIndex_Start)
        {
            // Tạo danh sách các column header
            string[] arrColumnHeader = new String[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                arrColumnHeader[i] = dt.Columns[i].ToString();
            }
            // lấy ra số lượng cột cần dùng dựa vào số lượng header
            var countColHeader = arrColumnHeader.Count();
            //set stype cho các cell của header
            var border0 = ws.Cells[rowIndex_Start, colIndex_Start, rowIndex_Start, countColHeader].Style.Border;
            border0.Bottom.Style = border0.Top.Style = border0.Left.Style = border0.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[rowIndex_Start, colIndex_Start, rowIndex_Start, countColHeader].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[rowIndex_Start, colIndex_Start, rowIndex_Start, countColHeader].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);//Backgroud
            ws.Cells[rowIndex_Start, colIndex_Start, rowIndex_Start, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;// căn giữa
            ws.Cells[rowIndex_Start, colIndex_Start, rowIndex_Start, countColHeader].Style.Font.Bold = true;// in đậm
            //tạo các header từ column header đã tạo từ bên trên
            foreach (var item in arrColumnHeader)
            {
                var cell = ws.Cells[rowIndex_Start, colIndex_Start];
                //gán giá trị
                cell.Value = item;
                colIndex_Start++;
            }



        }


    }
}
