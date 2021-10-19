using AppBodegaService.Method;
using AppBodegaService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppBodegaService.Controllers
{
    public class RequestOvertimeController : ApiController
    {
       
        // GET: api/RequestOvertime/5
        public HttpResponseMessage Get([FromUri] string CodigoCarga)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            //Buscar el overtime correspondiente a esa carga
            string RequestOvertime = RequestOvertimeMethod.GetRequestOvertimeByCodigoCarga(CodigoCarga);
            Tools.SetResponseContent(ref Response, RequestOvertime);
            return Response;
           
        }

        // POST: api/RequestOvertime
      
        [Route("api/RequestOvertime/Cancelar")]
        [HttpPost]
        public HttpResponseMessage CancelarRequestOvertime([FromBody]RequestOvertimeClass RequestOvertime)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            Log.SaveLog("Se Cancelará RequestOvertime");
            Log.SaveJsonInLog(RequestOvertime);

            try
            {
                RequestOvertimeMethod.CancelOvertime(RequestOvertime);
                Response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception Ex)
            {
                Log.SaveLog(Ex.ToString());
                Response.StatusCode = (HttpStatusCode)422;
            }

            return Response;
        }

        [Route("api/RequestOvertime/Actualizar")]
        [HttpPost]
        public void ActualizarRequestOvertime([FromBody]RequestOvertimeClass RequestOvertime)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            Log.SaveLog("Se Actualizará RequestOvertime");
            Log.SaveJsonInLog(RequestOvertime);

            try
            {
                RequestOvertimeMethod.UpdateRequestOvertime(RequestOvertime);
                Response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception Ex)
            {
                Log.SaveLog(Ex.ToString());
                Response.StatusCode = (HttpStatusCode)422;

            }
            
        }

        public void Post([FromBody]string value)
        {
        }

        // PUT: api/RequestOvertime/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RequestOvertime/5
        public void Delete(int id)
        {
        }
    }
}
