using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoDoCung.Service
{
    internal class LocalDatabaseProcess
    {
        private static string _connectString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppDomain.CurrentDomain.BaseDirectory}File\\DatabaseMaHang.mdf;Integrated Security=True";
        private SqlConnection Conn = new SqlConnection();

        //public LocalDatabaseProcess(string path)
        //{
        //    _connectString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={AppDomain.CurrentDomain.BaseDirectory}File\\DatabaseMaHang.mdf;Integrated Security=True";
        //    Conn = new SqlConnection(_connectString);
        //}
        public LocalDatabaseProcess()
        {
            Conn = new SqlConnection(_connectString);
        }
        private void OpenConnect()
        {
            if (Conn.State != ConnectionState.Open) Conn.Open();
        }

        private void DisConnect()
        {
            if (Conn.State != ConnectionState.Closed) Conn.Close();
        }

        public void AddMaHang(string NameMaHang,double Spec_Max_D1, double Spec_Min_D1, double Spec_Max_D2, double Spec_Min_D2, double Spec_Max_D3, double Spec_Min_D3, int Spec_Sodiemdo, double Spec_MaxSubMin,bool Select_MaxSubMin,bool Select_DoMau)
        {
            try
            {
                OpenConnect();
                using (SqlCommand sqlCommand = new SqlCommand("Insert into TableMaHang (NameMaHang,Spec_Max_D1,Spec_Min_D1,Spec_Max_D2,Spec_Min_D2,Spec_Max_D3,Spec_Min_D3,Spec_Sodiemdo,Spec_MaxSubMin,Select_MaxSubMin,Select_DoMau)  Values(@NameMaHang,@Spec_Max_D1,@Spec_Min_D1,@Spec_Max_D2,@Spec_Min_D2,@Spec_Max_D3,@Spec_Min_D3,@Spec_Sodiemdo,@Spec_MaxSubMin,@Select_MaxSubMin,@Select_DoMau)", this.Conn))
                {
                    sqlCommand.Parameters.AddWithValue("@NameMaHang", NameMaHang);
                    sqlCommand.Parameters.AddWithValue("@Spec_Max_D1", Spec_Max_D1);
                    sqlCommand.Parameters.AddWithValue("@Spec_Min_D1", Spec_Min_D1);
                    sqlCommand.Parameters.AddWithValue("@Spec_Max_D2", Spec_Max_D2);
                    sqlCommand.Parameters.AddWithValue("@Spec_Min_D2", Spec_Min_D2);
                    sqlCommand.Parameters.AddWithValue("@Spec_Max_D3", Spec_Max_D3);
                    sqlCommand.Parameters.AddWithValue("@Spec_Min_D3", Spec_Min_D3);
                    sqlCommand.Parameters.AddWithValue("@Spec_Sodiemdo", Spec_Sodiemdo);
                    sqlCommand.Parameters.AddWithValue("@Spec_MaxSubMin", Spec_MaxSubMin);
                    sqlCommand.Parameters.AddWithValue("@Select_MaxSubMin", Select_MaxSubMin);
                    sqlCommand.Parameters.AddWithValue("@Select_DoMau", Select_DoMau);
                    sqlCommand.ExecuteNonQuery();
                }
                DisConnect();
            }
            catch (Exception ex)
            {

            }
        }


        public void UpdateMaHang(string NameMaHang, double Spec_Max_D1, double Spec_Min_D1, double Spec_Max_D2, double Spec_Min_D2, double Spec_Max_D3, double Spec_Min_D3, int Spec_Sodiemdo, double Spec_MaxSubMin, bool Select_MaxSubMin, bool Select_DoMau)
        {
            try
            {
                OpenConnect();
                using (SqlCommand sqlCommand = new SqlCommand("UPDATE TableMaHang SET Spec_Max_D1 = @Spec_Max_D1, Spec_Min_D1 = @Spec_Min_D1, Spec_Max_D2 = @Spec_Max_D2, Spec_Min_D2 = @Spec_Min_D2, Spec_Max_D3 = @Spec_Max_D3, Spec_Min_D3 = @Spec_Min_D3, Spec_Sodiemdo = @Spec_Sodiemdo,  Spec_MaxSubMin = @Spec_MaxSubMin, Select_MaxSubMin = @Select_MaxSubMin, Select_DoMau = @Select_DoMau WHERE NameMaHang = @NameMaHang", this.Conn))
                {
                    sqlCommand.Parameters.AddWithValue("@NameMaHang", NameMaHang);
                    sqlCommand.Parameters.AddWithValue("@Spec_Max_D1", Spec_Max_D1);
                    sqlCommand.Parameters.AddWithValue("@Spec_Min_D1", Spec_Min_D1);
                    sqlCommand.Parameters.AddWithValue("@Spec_Max_D2", Spec_Max_D2);
                    sqlCommand.Parameters.AddWithValue("@Spec_Min_D2", Spec_Min_D2);
                    sqlCommand.Parameters.AddWithValue("@Spec_Max_D3", Spec_Max_D3);
                    sqlCommand.Parameters.AddWithValue("@Spec_Min_D3", Spec_Min_D3);
                    sqlCommand.Parameters.AddWithValue("@Spec_Sodiemdo", Spec_Sodiemdo);
                    sqlCommand.Parameters.AddWithValue("@Spec_MaxSubMin", Spec_MaxSubMin);
                    sqlCommand.Parameters.AddWithValue("@Select_MaxSubMin", Select_MaxSubMin);
                    sqlCommand.Parameters.AddWithValue("@Select_DoMau", Select_DoMau);
                    sqlCommand.ExecuteNonQuery();
                }
                DisConnect();
            }
            catch (Exception ex)
            {

            }
        }

        public DataTable getData_TableMaHang()
        {
            DataTable _table = new DataTable();
            OpenConnect();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM TableMaHang", Conn))
            {
                adapter.Fill(_table);
            }
            DisConnect();
            return _table;
        }

        //public DataTable getDataWhitDate_Table1(string _date)
        //{

        //    DataTable _table = new DataTable();
        //    if (_date == null) return _table;
        //    OpenConnect();
        //    DateTime dateTime = DateTime.Parse(_date);
        //    string dateStr = dateTime.ToString("yyyy-MM-dd");
        //    using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Testdata WHERE CAST(TestTime AS DATE) = '{dateStr}'", Conn))
        //    {
        //        adapter.Fill(_table);
        //    }
        //    DisConnect();
        //    return _table;
        //}






    }
}
