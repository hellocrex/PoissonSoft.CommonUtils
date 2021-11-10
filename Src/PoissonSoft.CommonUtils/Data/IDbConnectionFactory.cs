using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PoissonSoft.CommonUtils.Data
{
    /// <summary>
    /// Interface for simple connection factory
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Get the connection with openning
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Get the connection with openning async
        /// </summary>
        /// <returns></returns>
        Task<IDbConnection> GetConnectionAsync();
    }
}
