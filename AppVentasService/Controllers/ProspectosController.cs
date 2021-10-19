using System.Net.Http;
using System.Web.Http;
using WSprospectos.Method.Prospectos;
using WSprospectos.Models;
using WSprospectos.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;


namespace WSprospectos.AjusteController
{

    public class ProspectosController : ApiController
    {
        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok(/*$" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}"*/);
        }
        
        [HttpGet]
        [Route("api/Prospecto/Prueba")]
        public HttpResponseMessage Prueba([FromUri] string CodigoTipoAjuste = "")
        {


            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = ProspectosMetodo.Prueba();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }


        [HttpPost]
        [Route("api/Prospecto/Login")]
        public HttpResponseMessage Authenticate([FromBody] LoginRequest Credential)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            bool Logged = ProspectosMetodo.Autenticar(Credential.User, Credential.Password);

            if (Logged)
            {
                var token = TokenGenerator.GenerateTokenJwt(Credential.User);
                var userData = ProspectosMetodo.GetUserData(Credential.User, token);
                Tools.SetResponseContent(ref Response, userData);
                return Response;
            }
            else
            {
                Response.StatusCode = HttpStatusCode.Unauthorized;
                return Response;
            }
        }

        [HttpPost]
        [Route("api/Prospecto/Guardar")]
        public HttpResponseMessage Guardad([FromBody] ProspectosClass Prospecto)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            try
            {
                
                string resultado = ProspectosMetodo.Guardar(Prospecto);
                Response.StatusCode = HttpStatusCode.OK;
                Tools.SetResponseContent(ref Response, JsonConvert.SerializeObject(resultado));
                return Response;
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
                Response.StatusCode = (HttpStatusCode)422;
                Tools.SetResponseContent(ref Response, Error);
            }
            return Response;

        }

        [HttpPost]
        [Route("api/Prospecto/Guardar2")]
        public HttpResponseMessage Guardar([FromBody] ProspectosClass Prospecto)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            string resultado = ProspectosMetodo.Guardar(Prospecto);

      
                //var token = TokenGenerator.GenerateTokenJwt(Credential.User);
                //var userData = ProspectosMetodo.GetUserData(Credential.User, token);
                Tools.SetResponseContent(ref Response, resultado);
                return Response;

        }

        [HttpGet]
        [Route("api/Prospecto/GetProspectos")]
        public HttpResponseMessage GetProspectos([FromUri] string id = "")
        {


            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = ProspectosMetodo.GetProspectos();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }
        [HttpGet]
        [Route("api/Prospecto/GetProspectosTodos")]
        public HttpResponseMessage GetProspectosTodos([FromUri] string id = "")
        {


            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = ProspectosMetodo.GetProspectoTodoss();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }
        [HttpGet]
        [Route("api/Prospecto/CheckToken")]
        public HttpResponseMessage checkToken()
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            try
            {
                string resultado = "OK";
                Response.StatusCode = HttpStatusCode.OK;
                Tools.SetResponseContent(ref Response, resultado);
                return Response;
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
                Response.StatusCode = (HttpStatusCode)422;
                //Log.SaveLog(Error);
                Tools.SetResponseContent(ref Response, Error);
            }
            return Response;
        }
        [HttpPost]
        [Route("api/Prospecto/AutorizaRechaza")]
        public HttpResponseMessage AutorizaRechaza([FromBody] ProspectosClass Prospecto)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            try
            {

                string resultado = ProspectosMetodo.AutorizaRechaza(Prospecto);
                Response.StatusCode = HttpStatusCode.OK;
                Tools.SetResponseContent(ref Response, JsonConvert.SerializeObject(resultado));
                return Response;
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
                Response.StatusCode = (HttpStatusCode)422;
                Tools.SetResponseContent(ref Response, Error);
            }
            return Response;

        }
        [HttpPost, Route("api/Prospectos/upload")]
        public HttpResponseMessage Upload2([FromBody] FileClass[] Fotos)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                var provider = new MultipartMemoryStreamProvider();

                foreach (FileClass file in Fotos)
                {
                    if (file.NombreFoto != null)
                    {

                        var rutaFoto = ConfigurationManager.AppSettings["RutaArchivos"];
                        var filename = file.NombreFoto.Trim('\"');
                        byte[] imageBytes = Convert.FromBase64String(file.Url);
                        using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                        {
                                 File.WriteAllBytes(rutaFoto + "\\" + filename, imageBytes);               
                            ProspectosMetodo.GuardarNombreArchivo(filename,Fotos[0].IdProspecto);
                        }
                    }
                }

  
                Tools.SetResponseContent(ref Response, "Ok");
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar el Entry.");

                return Response;
            }

        }

    }
}
