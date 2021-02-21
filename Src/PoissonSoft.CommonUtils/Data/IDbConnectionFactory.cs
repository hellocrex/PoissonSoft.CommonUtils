using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PoissonSoft.CommonUtils.Data
{
    /// <summary>
    /// Interface for simple connection factory
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Get the connection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
    }
}
