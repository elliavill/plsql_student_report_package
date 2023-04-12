using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Package.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            OnPostGetCustomersWithOrders(null);
        }

        public void OnPostGetCustomersWithOrders(string selectedValue)
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_avillyani;user id=CS306_Villyani;password=pcc138772")) 
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetCustomersWithOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> customerList = new List<SelectListItem>();
                    while (reader.Read())
                    {
                        string customerInfo = reader.GetInt32(0).ToString() + " " +
                                              reader.GetString(1) + " " +
                                              reader.GetString(2);
                        SelectListItem customer = new SelectListItem();
                        customer.Text = customerInfo;
                        customer.Value = reader["CUS_CODE"].ToString();
                        if (customer.Value == selectedValue)
                        {
                            customer.Selected = true;
                        }
                        customerList.Add(customer);
                    }
                    ViewData["showCustomerList"] = customerList;
                }
                catch (MySqlException ex)
                {
                    ViewData["showCustomerList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

      
        public void OnPostGetOrdersForCustomer(string selectedOrder)
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_avillyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetOrdersForCustomer";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string selectedCustomer = HttpContext.Request.Form["customerList"];
                    cmd.Parameters.Add("customerCode", MySqlDbType.VarChar);
                    cmd.Parameters["customerCode"].Value = selectedCustomer;

                    // Execute the stored procedure
                    MySqlDataAdapter oda = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showOrderForCustomer"] = ds.Tables[0];
                    OnPostGetCustomersWithOrders(selectedCustomer);
                }
                catch (OracleException ex)
                {
                    ViewData["showOrderForCustomer"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public void OnPostGetOrderDetails()
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_avillyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetOrderDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string selectedInvoice = HttpContext.Request.Form["orderList"];
                    cmd.Parameters.Add("invoiceNumber", MySqlDbType.VarChar);
                    cmd.Parameters["invoiceNumber"].Value = HttpContext.Request.Form["btnOrder"].ToString();
                    // Execute the stored procedure
                    MySqlDataAdapter oda = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showOrderDetails"] = ds.Tables[0];
                    OnPostGetOrdersForCustomer(selectedInvoice);
                }
                catch (OracleException ex)
                {
                    ViewData["showOrderDetails"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        // Show capacity in the dropdown list
        public void OnPostUpdateQuantity(string lineNumberChange)
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_avillyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_UpdateOrderLine";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    decimal selectedCapacity = decimal.Parse(HttpContext.Request.Form["quantityList"]);
                    int selectedCustomer = Convert.ToInt32(HttpContext.Request.Form["customerList"]);
                    int selectedInvoice = Convert.ToInt32(HttpContext.Request.Form["btnOrder"]);

                    cmd.Parameters.Add("invoiceNumber", MySqlDbType.Int32);
                    cmd.Parameters["invoiceNumber"].Value = HttpContext.Request.Form["btnQuantity"].ToString();

                    cmd.Parameters.Add("lineNumber", MySqlDbType.Int32);
                    string lineNumber = HttpContext.Request.Form["lineNumber"];
                    string[] lineNumbers = lineNumber.Split(',');
                    for (int i = 0; i < lineNumbers.Length; i++)
                    {
                        string lineNum = lineNumbers[i];
                        if(lineNum == lineNumberChange)
                           cmd.Parameters["lineNumber"].Value = int.Parse(lineNum); 
                    }

                    cmd.Parameters.Add("newQuantity", MySqlDbType.Decimal);
                    cmd.Parameters["newQuantity"].Value = decimal.Parse(HttpContext.Request.Form["quantityList"].ToString());
                    cmd.ExecuteNonQuery();
                    OnPostGetOrderDetails();
                }
                catch (MySqlException ex)
                {
                    ViewData["showCapacityList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

    }
}