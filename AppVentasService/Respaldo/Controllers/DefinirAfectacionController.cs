using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppBodegaService.Method;
using System.Text;
using Newtonsoft.Json;
using AppBodegaService.Models.Ajutes;

namespace AppBodegaService.Controllers
{
    public class DefinirAfectacionController : ApiController
    {
        [HttpGet]
        [Route("api/DefinirAfectacion/GetAjustesXdefinir")]
        public HttpResponseMessage GetAjustesXdefinir([FromUri] int CodigoBodega = 0, int CodigoUsuario = 0)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            string Mensaje = DefinirAfectacionMethod.GetAjustesXdefinir(CodigoBodega, CodigoUsuario);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/DefinirAfectacion/GetDetalleAjusteXdefinir")]
        public HttpResponseMessage GetDetalleAjusteXdefinir([FromUri] int CodigoAjuste, string JobId)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            string Mensaje = DefinirAfectacionMethod.GetDetalleAjusteXdefinir(CodigoAjuste, JobId);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        //[HttpGet]
        //[Route("api/DefinirAfectacion/pruebaAvanzarActividad")]
        //public HttpResponseMessage pruebaAvanzarActividad([FromUri] string JobId, string afectaAgricutlor, string ObservacionesAjuste)
        //{
        //    HttpResponseMessage Response = new HttpResponseMessage();

        //    bool afectaAgricutlorB = Convert.ToBoolean(afectaAgricutlor);

        //    var resultado = DefinirAfectacionMethod.AvanzarActividad(JobId, afectaAgricutlorB, ObservacionesAjuste);

        //    var Mensaje = resultado == 1 ? "ok" : "fallo";

        //    Tools.SetResponseContent(ref Response, Mensaje);

        //    return Response;
        //}

        [Route("api/DefinirAfectacion/GuardaAfectacionAjuste")]
        [HttpPost]
        public HttpResponseMessage GuardaAfectacionAjuste([FromBody] AjusteXdefinirClass AjusteXdefinirClass)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                DefinirAfectacionMethod.GuardaAfectacionAjuste(AjusteXdefinirClass);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar guardar el tipo de ajuste.");
                Log.SaveLog(Error);
                Log.SaveLog(ex.ToString());
                Tools.SetResponseContent(ref Response, Error);
                return Response;
            }

        }

    }
}