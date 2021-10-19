using WSprospectos.Models;
using WSprospectos.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace WSprospectos.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok(/*$" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}"*/);
        }
        [HttpPost]
        [Route("Autenticar")]
        public HttpResponseMessage Autenticar([FromUri] string Usuario , string Pass)
        {


            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = Logeo.Autenticar(Usuario,Pass);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        //[HttpPost]
        //[Route("authenticate")]
        //public HttpResponseMessage Authenticate([FromBody] Logeo Credential)
        //{
        //    HttpResponseMessage Response = new HttpResponseMessage();
        //    bool Logged = Logeo.Prueba(Credential.User, Credential.Password);

        //    if (Logged)
        //    {
        //        var token = TokenGenerator.GenerateTokenJwt(Credential.User);
        //        var userData = ActiveDirectoryMethod.GetUserData(Credential.User, token);
        //        Tools.SetResponseContent(ref Response, ActiveDirectoryMethod.GetUserData(Credential.User, token));
        //        return Response;
        //    }
        //    else
        //    {
        //        Response.StatusCode = HttpStatusCode.Unauthorized;
        //        return Response;
        //    }
    }
    
}
