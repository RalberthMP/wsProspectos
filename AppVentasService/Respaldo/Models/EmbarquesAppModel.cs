using System.Configuration;
namespace EmbarquesAppService.Models
{
    partial class EmbarquesAppModelDataContext
    {
        public EmbarquesAppModelDataContext(string connection) :
                base(connection, mappingSource)
        {
            OnCreated();
        }
    }
}
