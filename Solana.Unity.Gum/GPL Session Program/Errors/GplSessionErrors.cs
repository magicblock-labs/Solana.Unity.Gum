using System;
using System.Collections.Generic;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Wallet;
using Solana.Unity.Gum;
namespace Solana.Unity.Gum.GplSession
{

    namespace Errors
    {
        public enum GplSessionErrorKind : uint
        {
            ValidityTooLong = 6000U,
            InvalidToken = 6001U,
            NoToken = 6002U
        }
    }
}