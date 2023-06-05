using System;

using Solana.Unity.Rpc;
using Solana.Unity.Wallet;
using Solana.Unity.Gum.GplSession;
using Solana.Unity.Gum.GplSession.Program;
using Solana.Unity.Wallet.Bip39;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Gum.GplSession.Accounts;

namespace Solana.Unity.Gum.Examples
{
    public class GplSessionInitiateTest
    {
        // Set up a test wallet

        private static readonly string mnemonic = "embark hockey chaos soda pioneer dynamic acquire surprise urge egg leaf country";

        private static readonly Wallet.Wallet wallet = new Wallet.Wallet(mnemonic);

        private const ulong LAMPORTS_PER_SOL = 1000000000;

        private const string uri = "http://localhost:8899";

        private static IRpcClient rpcClient = Rpc.ClientFactory.GetClient(uri, logger: true);

        private static void RequestAirdrop(PublicKey address, ulong lamports)
        {
            rpcClient.RequestAirdrop(address, lamports);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Running the example");
            Console.WriteLine("Wallet Publickey: " + wallet.Account.PublicKey.ToString());
            RequestAirdrop(wallet.Account.PublicKey, LAMPORTS_PER_SOL * 1);

            var balance = rpcClient.GetBalanceAsync(wallet.Account.PublicKey);
            Console.WriteLine("Balance: " + balance.Result);


            var sessionSigner = new Wallet.Wallet(WordCount.Twelve, WordList.English);

            var targetProgram = new PublicKey("6MhUAJtKdJx3RDCffUsJsQm8xy9YhhywjEmMYrxRc5j6");

            RequestAirdrop(sessionSigner.Account.PublicKey, LAMPORTS_PER_SOL * 1);

            CreateSessionAccounts createSessionAccounts = new CreateSessionAccounts();

            // Create PDA for session token
            createSessionAccounts.SessionToken = SessionToken.DeriveSessionTokenAccount(authority: wallet.Account.PublicKey, targetProgram: targetProgram, sessionSigner: sessionSigner.Account.PublicKey);
            createSessionAccounts.SessionSigner = sessionSigner.Account.PublicKey;
            createSessionAccounts.Authority = wallet.Account.PublicKey;
            createSessionAccounts.TargetProgram = targetProgram;
            createSessionAccounts.SystemProgram = SystemProgram.ProgramIdKey;

            Console.WriteLine("Session Token: " + createSessionAccounts.SessionToken.ToString());
            Console.WriteLine("Session Signer: " + createSessionAccounts.SessionSigner.ToString());
            Console.WriteLine("Authority: " + createSessionAccounts.Authority.ToString());
            Console.WriteLine("Target Program: " + createSessionAccounts.TargetProgram.ToString());
            Console.WriteLine("System Program: " + createSessionAccounts.SystemProgram.ToString());

            // Set up an empty transaction
            var transaction = new Transaction();
            // Initiate a Session
            var createSessionIx = GplSessionProgram.CreateSession(createSessionAccounts);

            transaction.Add(createSessionIx);

            transaction.FeePayer = wallet.Account.PublicKey;
            transaction.RecentBlockHash = rpcClient.GetRecentBlockHash().Result.Value.Blockhash;

            // Sign the transaction
            transaction.Sign(new[] { wallet.Account, sessionSigner.Account });

            // Send the transaction
            var txSig = rpcClient.SendAndConfirmTransaction(transaction.Serialize());
            // // Check the balance of the session token
            var sessionTokenData = rpcClient.GetAccountInfo(createSessionAccounts.SessionToken);
            var sessionToken = SessionToken.Deserialize(Convert.FromBase64String(sessionTokenData.Result.Value.Data[0]));
            Console.WriteLine("Session Token : " + sessionToken.ValidUntil.ToString());
        }
    }
}