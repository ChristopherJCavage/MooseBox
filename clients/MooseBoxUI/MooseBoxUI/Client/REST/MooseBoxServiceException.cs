//////////////////////////////////////////////////////////////////////////
// Copyright (C) 2015  Christopher James Cavage (cjcavage@gmail.com)    //
//                                                                      //
// This program is free software; you can redistribute it and/or        //
// modify it under the terms of the GNU General Public license          //
// as published by the Free Software Foundation; either version 2       //
// of the License, or (at your option) any later version.               //
//                                                                      //
// This program is distributed in the hope that it will be useful,      //
// but WITHOUT ANY WARRANTY; without even the implied warranty of       //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        //
// GNU General Public License for more details.                         //
//                                                                      //
// You should have received a copy of the GNU General Public License    //
// along with this program; if not, see <http://www.gnu.org/licenses/>. //
//////////////////////////////////////////////////////////////////////////
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;

namespace MooseBoxUI.Client.REST
{
    /// <summary>
    /// Defines an abstraction for a general-purpose REST API exception.
    /// </summary>
    internal sealed class MooseBoxServiceException : Exception
    {
        #region Constructor(s)
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="restResponse">RESTSharp generic REST Response object.</param>
        internal MooseBoxServiceException(IRestResponse restResponse) :
            base(MakeExceptionMessage(restResponse))
        {
            Debug.Assert(restResponse != null);

            //Set Members.
            m_restResponse = restResponse;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Error message associated with the failed REST call.
        /// </summary>
        internal string ErrorMessage
        {
            get { return m_restResponse.ErrorMessage; }
        }

        /// <summary>
        /// HTTP Status Code associated with the failed REST call.
        /// </summary>
        internal HttpStatusCode HttpStatusCode
        {
            get { return m_restResponse.StatusCode; }
        }
        #endregion

        #region Worker Methods
        /// <summary>
        /// Worker method to generate a stringified exception for the parent C# class.
        /// </summary>
        /// <param name="restResponse">RESTSharp generic REST Response object.</param>
        /// <returns>Exceptiond details string.</returns>
        private static string MakeExceptionMessage(IRestResponse restResponse)
        {
            //Parameter Validations.
            if (restResponse == null)
                throw new ArgumentNullException("IRestResponse restResponse");

            //Build Friendly Message.
            return string.Format("HTTP Status Code: {0}, Error: {1}, Rest Str: {2}",
                                 restResponse.StatusCode.ToString(),
                                 restResponse.StatusDescription,
                                 restResponse.ToString());
        }
        #endregion

        private readonly IRestResponse m_restResponse;
    }
}
