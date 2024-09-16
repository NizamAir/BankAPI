using BankAPI.DAL;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.Utility;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BankAPI.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private BankingDbContext _dbContext;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountService _accountService;

        public TransactionService(BankingDbContext dbContext, ILogger<TransactionService> logger, IOptions<AppSettings> settings, IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _accountService = accountService;
        }

        public Response CreateNewTransaction(Transaction transaction)
        {
            Response response = new Response();

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = null;

            return response;
        }

        public Response FindTransactionByDate(DateTime date)
        {
            Response response = new();

            var transactions = _dbContext.Transactions.Where(t => t.TransactionDate == date).ToList();

            response.ResponseCode = "00";
            response.ResponseMessage = "";
            response.Data = transactions;

            return response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new();

            Account sourceAccount, destinationAccount;

            Transaction transaction = new();

            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null)
                throw new ApplicationException("Invalid credintials");


            try
            {
                sourceAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = _accountService.GetByAccountNumber(AccountNumber);

                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check updating
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // transaction is successfull

                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successfull";
                    response.Data = null;
                }
                else
                {
                    // transaction is unsuccessfull

                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transcation failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TranscationDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FORM SOURCE => " +
               $"{JsonSerializer.Serialize(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => " +
               $"{JsonSerializer.Serialize(transaction.TranscationDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => " +
               $"{JsonSerializer.Serialize(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} " +
               $"TRANSACTION STATUS => {transaction.TransactionStatus}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Response response = new();

            Account sourceAccount, destinationAccount;

            Transaction transaction = new();

            var authUser = _accountService.Authenticate(FromAccount, TransactionPin);
            if (authUser == null)
                throw new ApplicationException("Invalid credintials");


            try
            {
                sourceAccount = _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = _accountService.GetByAccountNumber(ToAccount);

                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check updating
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // transaction is successfull

                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successfull";
                    response.Data = null;
                }
                else
                {
                    // transaction is unsuccessfull

                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transcation failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TranscationDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FORM SOURCE => " +
                $"{JsonSerializer.Serialize(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => " +
                $"{JsonSerializer.Serialize(transaction.TranscationDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => " +
                $"{JsonSerializer.Serialize(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} " +
                $"TRANSACTION STATUS => {transaction.TransactionStatus}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new();

            Account sourceAccount, destinationAccount;

            Transaction transaction = new();

            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null)
                throw new ApplicationException("Invalid credintials");


            try
            {
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);

                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check updating
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // transaction is successfull

                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successfull";
                    response.Data = null;
                }
                else
                {
                    // transaction is unsuccessfull

                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transcation failed!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TranscationDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FORM SOURCE => " +
                $"{JsonSerializer.Serialize(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => " +
                $"{JsonSerializer.Serialize(transaction.TranscationDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => " +
                $"{JsonSerializer.Serialize(transaction.TransactionAmount)} TRANSACTION TYPE => {JsonSerializer.Serialize(transaction.TransactionType)} " +
                $"TRANSACTION STATUS => {JsonSerializer.Serialize(transaction.TransactionStatus)}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }
    }
}
