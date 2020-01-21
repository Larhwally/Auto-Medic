using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Unilag_Medic.Data;
using Unilag_Medic.ViewModel;

namespace Unilag_Medic.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Tbl_LoginController : ControllerBase
    {
        //private IUserService userService;
        //private readonly UserManager<UnilagMedLogin> _usermanager;
        //private readonly IConfiguration _config;
        //public Tbl_LoginController(UserManager<UnilagMedLogin> manager, IConfiguration iconfiguration)
        //{
        //    _usermanager = manager;
        //    _config = iconfiguration;
        //}

        //GET: api/Tbl_Login
        [HttpGet]
        public string GetTblLogin()
        {
            EntityConnection con = new EntityConnection("tbl_Login");
            string rec = "{'Status': true, 'Data':" + EntityConnection.ToJson(con.Select()) + "}";
            return rec;
        }

        //POST: api/Tbl_Login
        [HttpPost]
        public string InsertTblLogin([FromBody] Dictionary<string, string> param)
        {
            EntityConnection con = new EntityConnection("tbl_Login");
            //bool response = false;
            if (param != null)
            {
                con.Insert(param);
                Response.WriteAsync("Record created successfully!");
            }
            else
            {
                //con.ResponseMsg(response);
                var resp = Response.WriteAsync("Error in creating record, check details and try again!");
                return resp.ToString();
            }
            return param.ToString();
        }
    }
}