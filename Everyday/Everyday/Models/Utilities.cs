using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Everyday.Models
{
    public class Utilities
    {
        public static DataSet Ejecutar(string cmd)
        {
            SqlConnection Con = new SqlConnection("Data Source=.; Initial Catalog=EverydayDB; Integrated Security=True");
            Con.Open();

            DataSet ds = new DataSet();
            SqlDataAdapter dp= new SqlDataAdapter(cmd, Con);

            dp.Fill(ds);
            Con.Close();

            return ds;
        }

        //public class MiModelo
        //{
        //    [Range(18, int.MaxValue, ErrorMessage = "Debes ser mayor de 18 años.")]
        //    public int Edad { get; set; }
        //}

    }
}