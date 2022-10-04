using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ustad.API.Models;
using Ustad.API.Variables;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ustad.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UstadFirmController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly tVariables v = new tVariables();

        public UstadFirmController(IConfiguration configration)
        {
            _configuration = configration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
               Select 
               UserId
             , UserFullName
             , UserFirstName
             , UserLastName
             , UserEMail 
             , UserKey
             , convert(varchar(10), RecordDate, 120) as RecordDate 
             , UserGUID
               From UstadUsers 
            ";
                        
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString(v.dbCrm);
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(UstadUser user)
        {
            string query = @"
               Insert into UstadUsers 
               values (@UserFirstName, @UserLastName)
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString(v.dbCrm);
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserFirstName", user.UserFirstName);
                    myCommand.Parameters.AddWithValue("@UserLastName", user.UserLastName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPut]
        public JsonResult Put(UstadUser user)
        {
            string query = @"
               Update UstadUsers set
               UserFirstName = @UserFirstName
             , UserLastName = @UserLastName
               Where UserGUID = @UserGUID ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString(v.dbCrm);
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserFirstName", user.UserFirstName);
                    myCommand.Parameters.AddWithValue("@UserLastName", user.UserLastName);
                    myCommand.Parameters.AddWithValue("@UserGUID", user.UserGUID);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Update successfully");
        }

        [HttpDelete]
        public JsonResult Delete(UstadUser user)
        {
            string query = @"
               Delete UstadUsers 
               Where UserGUID = @UserGUID ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString(v.dbCrm);
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserGUID", user.UserGUID);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Delete successfully");
        }


    }
}
