using AutoMapper;
using BankAPI.Models;
using BankAPI.Services.Interfaces;
using BankAPI.Shared.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;
        private IMapper _mapper;

        public AccountsController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            var accountsEntity = _accountService.GetAllAccounts();
            var accounts = _mapper.Map<IList<GetAccountDto>>(accountsEntity);
            return Ok(accounts);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetByAccountNumber(int id)
        {
            var accountEntity = _accountService.GetById(id);
            var account = _mapper.Map<GetAccountDto>(accountEntity);

            return Ok(account);
        }

        [HttpGet("{accountNumber}")]
        public IActionResult GetByAccountNumber(string accountNumber)
        {
            if (!Regex.IsMatch(accountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account number must be 10-digit");

            var accountEntity = _accountService.GetByAccountNumber(accountNumber);
            var account = _mapper.Map<GetAccountDto>(accountEntity);

            return Ok(account);
        }

        [HttpPost("register")]
        public IActionResult RegisterNewAccount([FromBody] RegisterNewAccountDto newAccount)
        {
            if (!ModelState.IsValid)
                return BadRequest(newAccount);

            var account = _mapper.Map<Account>(newAccount);

            return Ok(_accountService.Create(account, newAccount.Pin, newAccount.ConfirmPin));
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateDto authDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(authDto);

            return Ok(_accountService.Authenticate(authDto.AccountNumber, authDto.Pin));
        }

        [HttpPut]
        public IActionResult UpdateAccount([FromBody] UpdateAccountDto newAccount)
        {
            if (!ModelState.IsValid)
                return BadRequest(newAccount);

            var accountEntity = _mapper.Map<Account>(newAccount);

            _accountService.Update(accountEntity, newAccount.Pin);
            return Ok();
        }

    }
}
