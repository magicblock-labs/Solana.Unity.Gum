using System;
using System.Collections.Generic;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Wallet;
using Solana.Unity.Gum;
namespace Solana.Unity.Gum.GplSession
{

    namespace Errors
    {
        /// <summary>
        /// Enum representing the different kinds of errors that can occur during a GPL session.
        /// </summary>
        public enum GplSessionErrorKind : uint
        {
            /// <summary>
            /// Error indicating that the validity is too long.
            /// </summary>
            ValidityTooLong = 6000U,
            /// <summary>
            /// Error indicating that the token is invalid.
            /// </summary>
            InvalidToken = 6001U,
            /// <summary>
            /// Error indicating that no token was found.
            /// </summary>
            NoToken = 6002U
        }
    }
}