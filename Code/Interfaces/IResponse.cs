using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    internal interface IResponse<TEntity>
    {
        /// <summary>
        /// Response result
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Response model data
        /// </summary>
        public IEnumerable<TEntity> Data { get; }

        /// <summary>
        /// Rows count of response (refer to Data property model)
        /// </summary>
        public long Rows { get; }

        /// <summary>
        /// Response code
        /// <see cref="https://docs.microsoft.com/es-es/dotnet/api/system.net.httpstatuscode?view=net-5.0"/>
        /// </summary>
        public HttpStatusCode Code { get; }

        /// <summary>
        /// Message response
        /// </summary>
        public string Message { get; }

    }

}