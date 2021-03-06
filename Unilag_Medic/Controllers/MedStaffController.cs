﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unilag_Medic.Data;

namespace Unilag_Medic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedStaffController : ControllerBase
    {
        // GET: api/MedStaff
        [HttpGet]
        public string GetMedStaff()
        {
            EntityConnection con = new EntityConnection("tbl_MedicalStaff");
            string result = "{'Status': true, 'Data':" + EntityConnection.ToJson(con.Select()) + "}";
            return result;
        }

        // GET: api/MedStaff/5
        [HttpGet("{id}")]
        public string GetMedStaffbyId(int id)
        {
            EntityConnection con = new EntityConnection("tbl_MedicalStaff");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("itbId", id + "");
            string record = "{'status':true,'data':" + EntityConnection.ToJson(con.SelectByColumn(dict)) + "}";
            return record;
        }

        // POST: api/MedStaff
        [HttpPost]
        public string Post([FromBody] Dictionary<string, string> param )
        {
            EntityConnection con = new EntityConnection("tbl_MedicalStaff");
            if (param != null)
            {
                con.Insert(param);
                Response.WriteAsync("Record saves successfully!");
            }
            else
            {
                var resp = Response.WriteAsync("Error in creating record");
                return resp + "";
            }
            return param + "";
        }

        // PUT: api/MedStaff/5
        [HttpPut("{id}")]
        public string Put(int id, Dictionary<string, string> content)
        {
            EntityConnection con = new EntityConnection("tbl_MedicalStaff");
            if (id != 0)
            {
                con.Update(id, content);
                Response.WriteAsync("Record updated successfully!");
            }
            else
            {
                return BadRequest("Error in updating record!") + "";
            }
            return content + "";
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            EntityConnection con = new EntityConnection("tbl_MedicalStaff");
            if (id!= 0)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("itbId", id + "");
                con.Delete(param);
            }
            else
            {
                return NotFound().ToString();
            }
            return "Record deleted successfully";
        }
    }
}
