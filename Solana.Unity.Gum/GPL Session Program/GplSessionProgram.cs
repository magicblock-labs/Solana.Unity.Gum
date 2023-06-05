using System;
using System.Collections.Generic;
using Solana.Unity.Programs.Utilities;
using Solana.Unity.Wallet;
namespace Solana.Unity.Gum.GplSession
{
    namespace Program {
        public class CreateSessionAccounts
        {
            public PublicKey SessionToken { get; set; }

            public PublicKey SessionSigner { get; set; }

            public PublicKey Authority { get; set; }

            public PublicKey TargetProgram { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public class RevokeSessionAccounts
        {
            public PublicKey SessionToken { get; set; }

            public PublicKey Authority { get; set; }

            public PublicKey SystemProgram { get; set; }
        }

        public static class GplSessionProgram
        {

            /// <summary>
            /// The public key of the GPL Session program.
            /// </summary>
            public static readonly PublicKey ProgramIdKey = new PublicKey("3ao63wcSRNa76bncC2M3KupNtXBFiDyNbgK52VG7dLaE");
            /// <summary>
            /// Creates a new session account.
            /// </summary>
            /// <param name="accounts">The accounts used to create the session.</param>
            /// <param name="topUp">Whether to top up the session account.</param>
            /// <param name="validUntil">The time until the session is valid.</param>
            /// <param name="programId">The program ID of the session program.</param>
            /// <returns>A transaction instruction to create the session account.</returns>
            public static Solana.Unity.Rpc.Models.TransactionInstruction CreateSession(CreateSessionAccounts accounts, bool? topUp = null, long? validUntil = null, PublicKey programId = null!)

            {
                programId ??= ProgramIdKey;

                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.SessionToken, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.SessionSigner, true), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Authority, true), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.TargetProgram, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(16391441928816673266UL, offset);
                offset += 8;
                if (topUp != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteBool(topUp.Value, offset);
                    offset += 1;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                if (validUntil != null)
                {
                    _data.WriteU8(1, offset);
                    offset += 1;
                    _data.WriteS64(validUntil.Value, offset);
                    offset += 8;
                }
                else
                {
                    _data.WriteU8(0, offset);
                    offset += 1;
                }

                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction { Keys = keys, ProgramId = programId.KeyBytes, Data = resultData };
            }

            /// <summary>
            /// Revokes a session account.
            /// </summary>
            /// <param name="accounts">The accounts used to revoke the session.</param>
            /// <param name="programId">The program ID of the session program.</param>
            /// <returns>A transaction instruction to revoke the session account.</returns>
            public static Solana.Unity.Rpc.Models.TransactionInstruction RevokeSession(RevokeSessionAccounts accounts, PublicKey programId = null!)
            {
                programId ??= ProgramIdKey;

                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {
                    Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.SessionToken, false),
                    Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Authority, false),
                    Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)
                };
                byte[] _data = new byte[1200];
                int offset = 0;
                _data.WriteU64(13981146387719806038UL, offset);
                offset += 8;
                byte[] resultData = new byte[offset];
                Array.Copy(_data, resultData, offset);
                return new Solana.Unity.Rpc.Models.TransactionInstruction { Keys = keys, ProgramId = programId.KeyBytes, Data = resultData };
            }
        }

    }

    }