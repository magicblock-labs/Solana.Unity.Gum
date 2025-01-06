using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solana.Unity.Programs.Abstract;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Core.Sockets;
using Solana.Unity.Rpc.Types;
using Solana.Unity.Wallet;
using Solana.Unity.SessionKeys.GplSession.Accounts;
using Solana.Unity.SessionKeys.GplSession.Errors;
using Solana.Unity.SessionKeys.GplSession.Program;

namespace Solana.Unity.SessionKeys.GplSession
{
    /// <summary>
    /// Client for interacting with the GPL Session program on the Solana blockchain.
    /// </summary>
    public partial class GplSessionClient : TransactionalBaseClient<GplSessionErrorKind>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GplSessionClient"/> class.
        /// </summary>
        /// <param name="rpcClient">The RPC client to use for sending requests.</param>
        /// <param name="streamingRpcClient">The streaming RPC client to use for subscribing to account changes.</param>
        /// <param name="programId">The program ID.</param>
        public GplSessionClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
        }

        /// <summary>
        /// Retrieves all session token accounts associated with a program.
        /// </summary>
        /// <param name="programAddress">The address of the program.</param>
        /// <param name="commitment">The commitment level to use for the request.</param>
        /// <returns>A result containing a list of session token accounts.</returns>
        public async Task<Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<SessionToken>>> GetSessionTokensAsync(string programAddress, Commitment commitment = Commitment.Finalized)
        {
            var list = new List<Solana.Unity.Rpc.Models.MemCmp> { new Solana.Unity.Rpc.Models.MemCmp { Bytes = SessionToken.ACCOUNT_DISCRIMINATOR_B58, Offset = 0 } };
            var res = await RpcClient.GetProgramAccountsAsync(programAddress, commitment, memCmpList: list);
            if (!res.WasSuccessful || !(res.Result?.Count > 0))
                return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<SessionToken>>(res);
            List<SessionToken> resultingAccounts = new List<SessionToken>(res.Result.Count);
            resultingAccounts.AddRange(res.Result.Select(result => SessionToken.Deserialize(Convert.FromBase64String(result.Account.Data[0]))));
            return new Solana.Unity.Programs.Models.ProgramAccountsResultWrapper<List<SessionToken>>(res, resultingAccounts);
        }

        /// <summary>
        /// Retrieves a session token account.
        /// </summary>
        /// <param name="accountAddress">The address of the session token account.</param>
        /// <param name="commitment">The commitment level to use for the request.</param>
        /// <returns>A result containing the session token account.</returns>
        public async Task<Solana.Unity.Programs.Models.AccountResultWrapper<SessionToken>> GetSessionTokenAsync(string accountAddress, Commitment commitment = Commitment.Finalized)
        {
            var res = await RpcClient.GetAccountInfoAsync(accountAddress, commitment);
            if (!res.WasSuccessful)
                return new Solana.Unity.Programs.Models.AccountResultWrapper<SessionToken>(res);
            var resultingAccount = SessionToken.Deserialize(Convert.FromBase64String(res.Result.Value.Data[0]));
            return new Solana.Unity.Programs.Models.AccountResultWrapper<SessionToken>(res, resultingAccount);
        }

        /// <summary>
        /// Subscribes to changes in a session token account.
        /// </summary>
        /// <param name="accountAddress">The address of the session token account.</param>
        /// <param name="callback">The callback function to be called when the account changes.</param>
        /// <param name="commitment">The commitment level to use for the subscription.</param>
        /// <returns>The subscription state.</returns>
        public async Task<SubscriptionState> SubscribeSessionTokenAsync(string accountAddress, Action<SubscriptionState, Solana.Unity.Rpc.Messages.ResponseValue<Solana.Unity.Rpc.Models.AccountInfo>, SessionToken> callback, Commitment commitment = Commitment.Finalized)
        {
            SubscriptionState res = await StreamingRpcClient.SubscribeAccountInfoAsync(accountAddress, (s, e) =>
            {
                SessionToken parsingResult = null;
                if (e.Value?.Data?.Count > 0)
                    parsingResult = SessionToken.Deserialize(Convert.FromBase64String(e.Value.Data[0]));
                callback(s, e, parsingResult);
            }, commitment);
            return res;
        }

        /// <summary>
        /// Sends a transaction to create a new session.
        /// </summary>
        /// <param name="accounts">The accounts involved in the transaction.</param>
        /// <param name="topUp">Whether to top up the session account.</param>
        /// <param name="validUntil">The timestamp until which the session is valid.</param>
        /// <param name="topUpLamports">The lamports to topup.</param>
        /// <param name="feePayer">The public key of the fee payer.</param>
        /// <param name="signingCallback">The signing callback function.</param>
        /// <param name="programId">The program ID.</param>
        /// <returns>A request result containing the transaction signature.</returns>
        public async Task<RequestResult<string>> SendCreateSessionAsync(CreateSessionAccounts accounts, bool? topUp, long? validUntil, ulong? topUpLamports, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.GplSessionProgram.CreateSession(accounts, topUp, validUntil, topUpLamports, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        /// <summary>
        /// Sends a transaction to revoke a session.
        /// </summary>
        /// <param name="accounts">The accounts involved in the transaction.</param>
        /// <param name="feePayer">The public key of the fee payer.</param>
        /// <param name="signingCallback">The signing callback function.</param>
        /// <param name="programId">The program ID.</param>
        /// <returns>A request result containing the transaction signature.</returns>
        public async Task<RequestResult<string>> SendRevokeSessionAsync(RevokeSessionAccounts accounts, PublicKey feePayer, Func<byte[], PublicKey, byte[]> signingCallback, PublicKey programId)
        {
            Solana.Unity.Rpc.Models.TransactionInstruction instr = Program.GplSessionProgram.RevokeSession(accounts, programId);
            return await SignAndSendTransaction(instr, feePayer, signingCallback);
        }

        /// <summary>
        /// Builds a dictionary of program errors and their corresponding error codes.
        /// </summary>
        /// <returns>A dictionary of program errors and their corresponding error codes.</returns>
        protected override Dictionary<uint, ProgramError<GplSessionErrorKind>> BuildErrorsDictionary()
        {
            return new Dictionary<uint, ProgramError<GplSessionErrorKind>> { { 6000U, new ProgramError<GplSessionErrorKind>(GplSessionErrorKind.ValidityTooLong, "Requested validity is too long") }, { 6001U, new ProgramError<GplSessionErrorKind>(GplSessionErrorKind.InvalidToken, "Invalid session token") }, { 6002U, new ProgramError<GplSessionErrorKind>(GplSessionErrorKind.NoToken, "No session token provided") }, };
        }
    }
}