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
            public static Solana.Unity.Rpc.Models.TransactionInstruction CreateSession(CreateSessionAccounts accounts, bool? topUp, long? validUntil, PublicKey programId)
            {
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

            public static Solana.Unity.Rpc.Models.TransactionInstruction RevokeSession(RevokeSessionAccounts accounts, PublicKey programId)
            {
                List<Solana.Unity.Rpc.Models.AccountMeta> keys = new()
                {Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.SessionToken, false), Solana.Unity.Rpc.Models.AccountMeta.Writable(accounts.Authority, false), Solana.Unity.Rpc.Models.AccountMeta.ReadOnly(accounts.SystemProgram, false)};
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