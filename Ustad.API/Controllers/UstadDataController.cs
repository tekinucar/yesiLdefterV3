using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ustad.API.Classes;
using Ustad.API.Models;
using Ustad.API.Variables;
using Ustad.API.ToolBox;
using QuickType;
using Newtonsoft.Json;

namespace Ustad.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UstadDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly tVariables v = new tVariables();
        private tTools t = new tTools();
        private tQueryAbout _queryAbout = new tQueryAbout();
        public MsTablesIp _ms = new MsTablesIp();
        public UstadDataController(IConfiguration configration)
        {
            _configuration = configration;
        }

        [HttpGet]
        [Route("{action}")]
        public JsonResult Get()
        {
            string key = "";
            string value = ""; 
            string tableIPCode = "";

            if (Request.Query.Count == 0) 
                return new JsonResult("Lütfen parametre giriniz... ( UstadData/Get?TableIPCode=xxx/yyy/TableCode.IPCode ) ");

            foreach (var item in Request.Query)
            {
                key = item.Key; 
                value = item.Value;

                if (key.ToUpper() == "TABLEIPCODE")
                    tableIPCode = value;
            }  

            if (tableIPCode == "") new JsonResult("Lütfen TableIPCode yi kontrol ediniz...");
              
            /// tableIPCode = "UST/OMS/HMaliyet.Maliyet_L04";

            _queryAbout.Clear();
            _queryAbout.SqlDataSource = _configuration.GetConnectionString(v.dbProject);
            _queryAbout.QuerySql = t.GetTableIPCodeSql(tableIPCode, _configuration);

            JsonResult result = null;

            if (_queryAbout.QuerySql != "")
                 result = t.RunQueryJson(_queryAbout);
            else result = new JsonResult(tableIPCode + " için SQL cümlesi yok..");

            return result;
        }
        
        [HttpGet]
        public JsonResult GetDefault()
        {
            return new JsonResult("Merhaba. Data talebiniz için hazırım...");
        }

    }


}
