using AppBodegaService.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppBodegaService.Controllers
{
    public class PalletFaltanteController : ApiController
    {
        // GET: api/PalletFaltante
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PalletFaltante/5
        public HttpResponseMessage Get([FromUri]int CodigoCarga,int CodigoEntry)
        {
            
            HttpResponseMessage Response = new HttpResponseMessage();
            Log.SaveLog("Se Buscará si hay algún Pallet Faltante");

            try
            {
                string Resultado = PalletFaltanteMethod.GetPalletFaltante(CodigoEntry,CodigoCarga);
                Tools.SetResponseContent(ref Response, Resultado);
                Response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception Ex)
            {
                Log.SaveLog(Ex.ToString());
                Response.StatusCode = (HttpStatusCode)422;
            }

            return Response;
        }

        // POST: api/PalletFaltante
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/PalletFaltante/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PalletFaltante/5
        public void Delete(int id)
        {
        }
    }
}
