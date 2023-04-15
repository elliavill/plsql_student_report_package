﻿using Microsoft.AspNetCore.Mvc;
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
            OnPostGetManagerList();
            OnPostShowManagerDropdown(null);
        }

        public void OnPostGetManagerList()
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_getManagerList";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    // Execute the stored procedure
                    MySqlDataAdapter oda = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    oda.Fill(ds);

                    // Show the table of the list of manager
                    ViewData["showManagerList"] = ds.Tables[0];
                }
                catch (MySqlException ex)
                {
                    ViewData["showManagerList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public void OnPostGetEmployeeList(string selectedOrder)
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_getEmployeeList";
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
                    //OnPostGetManagerList(selectedCustomer);
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

        public void OnPostShowManagerDropdown(string selectedValue)
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_getManagerList";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> managerList = new List<SelectListItem>();
                    while (reader.Read())
                    {
                        string managerInfo = reader.GetInt32(0).ToString() + ' ' + reader.GetString(1);
                        SelectListItem manager = new SelectListItem();
                        manager.Text = managerInfo;
                        manager.Value = reader["employeeNumber"].ToString();
                        if (manager.Value == selectedValue)
                        {
                            manager.Selected = true;
                        }
                        managerList.Add(manager);
                    }
                    ViewData["showManagerDropdown"] = managerList;
                }
                catch (MySqlException ex)
                {
                    ViewData["showManagerDropdown"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}