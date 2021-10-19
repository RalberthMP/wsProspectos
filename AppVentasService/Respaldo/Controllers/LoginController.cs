using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppBodegaService.Method.ActiveDirectoryCredential;
using AppBodegaService.Models.ActiveDirectoryCredential;
using AppBodegaService.Method;
using TripleH.Erp.Model;
using System.Text;

namespace AppBodegaService.LoginController
{
    
    public class LoginController : ApiController
    {
        // GET: api/Login
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Login/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Login
        public HttpResponseMessage Post([FromBody]ActiveDirectoryCredential Credential)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            bool Logged = ActiveDirectoryCredentialMethod.UserLogin(Credential.User, Credential.Password);

            if (Logged)
            {
                Response.StatusCode = HttpStatusCode.OK;
                Tools.SetResponseContent(ref Response, ActiveDirectoryCredentialMethod.GetUserData(Credential.User));
            }
            else
            {
                Response.StatusCode = HttpStatusCode.Unauthorized;
            }

            return Response;
        }

        // PUT: api/Login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Login/5
        public void Delete(int id)
        {
        }
    }
}
