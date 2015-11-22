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
using System;
using System.Diagnostics;

namespace MooseBoxUI.Client.REST
{
    /// <summary>
    /// Defines an abstraction to create a versioned MooseBox REST API interface.
    /// </summary>
    internal sealed class MooseBoxRESTAPIFactory
    {
        #region Constructor(s) / Singleton
        /// <summary>
        /// Constructor.
        /// </summary>
        private MooseBoxRESTAPIFactory()
        {
            //Set Members.
            m_baseUrlStr = string.Empty;
        }

        /// <summary>
        /// Gets the singletone instance for the MooseBox REST API factory.
        /// </summary>
        internal static MooseBoxRESTAPIFactory Instance
        {
            get { return s_instance.Value; }
        }
        #endregion

        #region Creation Methods
        /// <summary>
        /// Creates a MooseBox REST API at the latest version.
        /// </summary>
        /// <param name="baseUrlStr">Base URL address to the MooseBox, which should be "MooseBox."</param>
        /// <returns>Instance of versioned MooseBox REST API.</returns>
        internal IMooseBoxRESTAPI Create()
        {
            return Create(m_baseUrlStr);
        }

        /// <summary>
        /// Creates a MooseBox REST API at the latest version.
        /// </summary>
        /// <param name="baseUrlStr">Base URL address to the MooseBox, which should be "MooseBox."</param>
        /// <returns>Instance of versioned MooseBox REST API.</returns>
        internal IMooseBoxRESTAPI Create(string baseUrlStr)
        {
            return Create(MaxVersion, baseUrlStr);
        }

        /// <summary>
        /// Creates a MooseBox REST API at the latest version.
        /// </summary>
        /// <param name="version">Specific version of the MooseBox REST API to create.</param>
        /// <param name="baseUrlStr">Base URL address to the MooseBox, which should be "MooseBox."</param>
        /// <returns>Instance of versioned MooseBox REST API.</returns>
        internal IMooseBoxRESTAPI Create(Byte version, string baseUrlStr)
        {
            IMooseBoxRESTAPI mooseBoxRESTAPI = null;
            Uri baseUrl = default(Uri);

            //Parameter Validations.
            if (string.IsNullOrEmpty(baseUrlStr) == true)
                throw new ArgumentNullException("string baseUrlStr");

            if (Uri.IsWellFormedUriString(baseUrlStr, UriKind.RelativeOrAbsolute) == false)
                throw new ArgumentException("Invalid Url. Found: " + base.ToString(), "Uri baseUrlStr");

            //Create the base URL.
            baseUrl = new Uri(baseUrlStr);

            //Create API based on version.
            switch(version)
            {
                case 1:
                    mooseBoxRESTAPI = new MooseBoxRESTAPI_v1_0(baseUrl);

                    break;

                default:
                    throw new ArgumentOutOfRangeException("Byte version",
                                                          string.Format("{0} <= Version <= {1}. Found: {2}",
                                                                        MinVersion,
                                                                        MaxVersion,
                                                                        version));
            }

            Debug.Assert(mooseBoxRESTAPI != null);

            return mooseBoxRESTAPI;
        }
        #endregion

        #region Registration Methods
        /// <summary>
        /// Registers a base web address URL for the MooseBox REST API.
        /// </summary>
        /// <param name="baseUrlStr">Base URL address to the MooseBox, which should be "MooseBox."</param>
        void Register(string baseUrlStr)
        {
            //Parameter Validations.
            if (string.IsNullOrEmpty(baseUrlStr) == true)
                throw new ArgumentNullException("string baseUrlStr");

            if (Uri.IsWellFormedUriString(baseUrlStr, UriKind.RelativeOrAbsolute) == false)
                throw new ArgumentException("Invalid Url. Found: " + base.ToString(), "Uri baseUrlStr");

            //Is it already registered?
            if (string.IsNullOrEmpty(m_baseUrlStr) == true)
                throw new InvalidOperationException(string.Format("Base URL already registered. URL: {0}", m_baseUrlStr));

            //Set Member.
            m_baseUrlStr = baseUrlStr;
        }

        /// <summary>
        /// Unregisters a base web address URL for the MooseBox REST API.
        /// </summary>
        void Unregister()
        {
            //Clear base URL.
            m_baseUrlStr = string.Empty;
        }
        #endregion

        private string m_baseUrlStr;

        private const Byte MinVersion = 1;
        private const Byte MaxVersion = 1;

        private static readonly Lazy<MooseBoxRESTAPIFactory> s_instance = new Lazy<MooseBoxRESTAPIFactory>(() => { return new MooseBoxRESTAPIFactory(); }, true);
    }
}
