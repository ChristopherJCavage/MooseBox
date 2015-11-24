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
using System.Threading;
using System.Threading.Tasks;

namespace MooseBoxUI.Utility
{
    /// <summary>
    /// Microsoft Internal code to run Tasks synchronously; posted on Stack Overflow.
    /// </summary>
    /// <see href="http://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c"/>
    internal static class AsyncHelper
    {
        /// <summary>
        /// Runs non-void Task synchronously from a non-async method.
        /// </summary>
        /// <typeparam name="TResult">Result of Task after being run.</typeparam>
        /// <param name="func">Func to invoke with task.</param>
        /// <returns>Resultant data from Task.</returns>
        internal static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._myTaskFactory
              .StartNew<Task<TResult>>(func)
              .Unwrap<TResult>()
              .GetAwaiter()
              .GetResult();
        }

        /// <summary>
        /// Runs a void Task synchronously from a non-async method.
        /// </summary>
        /// <param name="func">Func to invoke with task.</param>
        internal static void RunSync(Func<Task> func)
        {
            AsyncHelper._myTaskFactory
              .StartNew<Task>(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None,
                                                                             TaskCreationOptions.None,
                                                                             TaskContinuationOptions.None,
                                                                             TaskScheduler.Default);
    }
}
