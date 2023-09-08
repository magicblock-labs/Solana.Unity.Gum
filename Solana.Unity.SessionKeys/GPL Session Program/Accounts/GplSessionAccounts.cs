using System;
using System.Collections.Generic;
using System.Text;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.SessionKeys.GplSession.Program;
using Solana.Unity.Wallet;

namespace Solana.Unity.SessionKeys.GplSession
{
    namespace Accounts
    {
        /// <summary>
        /// Represents a session token account.
        /// </summary>
        public partial class SessionToken
        {
            /// <summary>
            /// The discriminator value for the session token account.
            /// </summary>
            public static ulong ACCOUNT_DISCRIMINATOR => 1081168673100727529UL;
            /// <summary>
            /// The byte representation of the account discriminator.
            /// </summary>
            public static ReadOnlySpan<byte> ACCOUNT_DISCRIMINATOR_BYTES => new byte[]{233, 4, 115, 14, 46, 21, 1, 15};
            /// <summary>
            /// Base58 encoded string representation of the account discriminator.
            /// </summary>
            public static string ACCOUNT_DISCRIMINATOR_B58 => "fyZWTdUu1pS";
            /// <summary>
            /// The public key of the authority for the session token account.
            /// </summary>
            public PublicKey Authority { get; set; }

            /// <summary>
            /// The public key of the target program for the session token account.
            /// </summary>
            public PublicKey TargetProgram { get; set; }

            /// <summary>
            /// The public key of the session signer for the session token account.
            /// </summary>
            public PublicKey SessionSigner { get; set; }

            /// <summary>
            /// The Unix timestamp in seconds when the session token expires.
            /// </summary>
            public long ValidUntil { get; set; }

            /// <summary>
            /// Deserializes a session token account from a byte array.
            /// </summary>
            /// <param name="_data">The byte array to deserialize.</param>
            /// <returns>The deserialized session token account.</returns>
            public static SessionToken Deserialize(ReadOnlySpan<byte> _data)
            {
                int offset = 0;
                ulong accountHashValue = _data.GetU64(offset);
                offset += 8;
                if (accountHashValue != ACCOUNT_DISCRIMINATOR)
                {
                    return null;
                }

                SessionToken result = new SessionToken();
                result.Authority = _data.GetPubKey(offset);
                offset += 32;
                result.TargetProgram = _data.GetPubKey(offset);
                offset += 32;
                result.SessionSigner = _data.GetPubKey(offset);
                offset += 32;
                result.ValidUntil = _data.GetS64(offset);
                offset += 8;
                return result;
            }

            // Function to derive PDA
            /// <summary>
            /// Derives the public key for a session token account.
            /// </summary>
            /// <param name="authority">The authority for the session token account.</param>
            /// <param name="targetProgram">The target program for the session token account.</param>
            /// <param name="sessionSigner">The session signer for the session token account.</param>
            /// <returns>The public key for the session token account.</returns>
            public static PublicKey DeriveSessionTokenAccount(PublicKey authority, PublicKey targetProgram, PublicKey sessionSigner)
            {

                var accounts = new List<byte[]>() {
                    Encoding.UTF8.GetBytes("session_token"),
                    targetProgram,
                    sessionSigner,
                    authority
                };
                bool success = PublicKey.TryFindProgramAddress(accounts, GplSessionProgram.ProgramIdKey, out PublicKey sessionTokenPublicKey, out _);
                return sessionTokenPublicKey;
            }
        }
    }
}