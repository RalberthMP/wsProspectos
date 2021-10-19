using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TripleH.Erp.Model;
using AppBodegaService.Models;
using AppBodegasService.Global;

namespace AppBodegaService
{
    public class RequestOvertimeMethod
    {
        static string ConnectionString = Configurations.ConnectionString;
        internal static string GetRequestOvertimeByCodigoCarga(string CodigoCarga)
        {
            SqlCommand Command = new SqlCommand(string.Format(
                                                    @"Select
	                                                RequestOvertime.*,
	                                                (Select RequestOvertimeDetalle.* from RequestOvertimeDetalle
			                                                where RequestOvertimeDetalle.CodigoRequestOvertime = RequestOvertime.Codigo
			                                                For xml path('RequestOvertimeDetalle'),type
		                                                )
	                                                from RequestOvertime
		                                                join RequestOvertimeDetalle 
			                                                on(RequestOvertime.Codigo = RequestOvertimeDetalle.CodigoRequestOvertime)
	                                                Where 
		                                                RequestOvertimeDetalle.CodigoCarga = {0}
                                                     and
	                                                    RequestOvertimeDetalle.FechaHoraLlegada is not null
                                                     and
	                                                    RequestOvertimeDetalle.FechaCancelacion is null
	                                                For xml path('Data'),root('Root')", CodigoCarga));

            return Tools.CommandToJson(Command);            
        }
        internal static void CancelOvertime(RequestOvertimeClass RequestOvertime)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            int? CodigoCarga = RequestOvertime.RequestOvertimeDetalle.CodigoCarga;
            long CodigoRequestOvertimeDetalle = RequestOvertime.RequestOvertimeDetalle.Codigo;
            string MensajeCancelacion = RequestOvertime.RequestOvertimeDetalle.DescripcionMotivoCancelacion;

            RequestOvertimeDetalle OvertimeDetalle = data.RequestOvertimeDetalles
                                                        .SingleOrDefault(x => x.Codigo == CodigoRequestOvertimeDetalle);

            if (OvertimeDetalle == null)
            {
                Tools.ThrowException(string.Format("No se encontró OvertimeDetalle con Codigo {0}", CodigoRequestOvertimeDetalle));
                return;
            }

            OvertimeDetalle.DescripcionMotivoCancelacion = MensajeCancelacion;
            OvertimeDetalle.FechaCancelacion = DateTime.Now;
            data.SubmitChanges();            
        }

        internal static void UpdateRequestOvertime(RequestOvertimeClass RequestOvertimeUpdated)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            long CodigoRequestOvertimeDetalle = RequestOvertimeUpdated.RequestOvertimeDetalle.Codigo;
            long CodigoRequestOvertime = RequestOvertimeUpdated.Codigo;

            RequestOvertime RequestOvertime = data.RequestOvertimes.SingleOrDefault(ro => ro.Codigo == CodigoRequestOvertime);

            if (RequestOvertime == null)
            {
                Tools.ThrowException(string.Format("No se encontró RequestOvertime con Codigo {0}", CodigoRequestOvertime));
            }

            RequestOvertime.DescripcionMotivo = RequestOvertimeUpdated.DescripcionMotivo;

            RequestOvertimeDetalleClass RequestOvertimeDetalleClassUpdated = RequestOvertimeUpdated.RequestOvertimeDetalle;
            RequestOvertimeDetalle OvertimeDetalle = data.RequestOvertimeDetalles
                                                        .SingleOrDefault(x => x.Codigo == CodigoRequestOvertimeDetalle);

            if (OvertimeDetalle == null)
            {
                Tools.ThrowException(string.Format("No se encontró RequestOvertimeDetalle con Codigo {0}", CodigoRequestOvertimeDetalle));
            }

            OvertimeDetalle.FechaHoraLlegada = RequestOvertimeDetalleClassUpdated.FechaHoraLlegada;
            OvertimeDetalle.ImporteOvertime = RequestOvertimeDetalleClassUpdated.ImporteOvertime;
            OvertimeDetalle.HorasOvertime = RequestOvertimeDetalleClassUpdated.HorasOvertime;
            OvertimeDetalle.CodigoAfectacionOvertime = RequestOvertimeDetalleClassUpdated.CodigoAfectacionOvertime;
            OvertimeDetalle.CodigoCausaOvertime = RequestOvertimeDetalleClassUpdated.CodigoCausaOvertime;

            OvertimeDetalle.DescripcionMotivoOverTime = RequestOvertimeDetalleClassUpdated.DescripcionMotivoOverTime;
            OvertimeDetalle.DescripcionMotivoCancelacion = RequestOvertimeDetalleClassUpdated.DescripcionMotivoCancelacion;

            data.SubmitChanges();
        }
    }
}