﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComercialClothes.Models.DTOs.Requests;
using CommercialClothes.Models;
using CommercialClothes.Models.DAL;
using CommercialClothes.Models.DAL.Interfaces;
using CommercialClothes.Models.DAL.Repositories;
using CommercialClothes.Models.DTOs;
using CommercialClothes.Models.DTOs.Requests;
using CommercialClothes.Models.DTOs.Responses;
using CommercialClothes.Services.Base;
using CommercialClothes.Services.Interfaces;
using Model.DAL.Interfaces;
using Model.DTOs.Responses;

namespace CommercialClothes.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IHistoryTransactionRepository _historyTransactionRepo;
        private readonly IShopRepository _shopRepo;
        private readonly Encryptor _encryptor;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _map;

        public UserService(IUserRepository userRepo, IUnitOfWork unitOfWork, Encryptor encryptor
            , IEmailSender emailSender, IMapperCustom mapper, IOrderRepository orderRepository
            , IRefreshTokenRepository refreshTokenRepossitory, IMapper map
            , IHistoryTransactionRepository historyTransactionRepo
            , IShopRepository shopRepo) : base(unitOfWork, mapper)
        {
            _userRepo = userRepo;
            _encryptor = encryptor;
            _emailSender = emailSender;
            _refreshTokenRepo = refreshTokenRepossitory;
            _historyTransactionRepo = historyTransactionRepo;
            _orderRepo = orderRepository;
            _map = map;
            _shopRepo = shopRepo;
        }

        public async Task<UserResponse> FindById(int userId)
        {
            try
            {
                var user = await _userRepo.FindAsync(us => us.Id == userId);
                var userDTO = _map.Map<Account, UserDTO>(user);
                return new UserResponse
                {
                    IsSuccess = true,
                    UserDTO = userDTO
                };
            }
            catch (Exception e)
            {
                return new UserResponse
                {
                    ErrorMessage = e.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<bool> Logout(int userId)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                await _refreshTokenRepo.DeleteAll(userId);
                await _unitOfWork.CommitTransaction();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckUserByActivationCode(Guid activationCode)
        {
            var user = await _userRepo.FindAsync(us => us.ActivationCode == activationCode);
            if (user == null)
                return false;

            user.IsActivated = true;
            await _unitOfWork.CommitTransaction();
            return true;
        }

        public async Task<UserResponse> ForgotPassword(string userEmail)
        {
            try
            {
                // 1. Find user by email
                var user = await _userRepo.FindAsync(us => us.Email == userEmail && us.IsActivated == true);

                // 2. Check
                if (user == null)
                {
                    return new UserResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Không thể tìm thấy Email được đăng ký !",
                    };
                }

                // 3. Generate reset password code to validate
                var resetCode = Guid.NewGuid();
                user.ResetPasswordCode = resetCode;

                // 3. Send email to user to reset password
                await _emailSender.SendEmailVerificationAsync(userEmail, resetCode.ToString(), "reset-password");
                await _unitOfWork.CommitTransaction();

                return new UserResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                };
            }
        }

        public async Task<bool> GetUserByResetCode(Guid resetPassCode)
        {
            return await _userRepo.FindAsync(us => us.ResetPasswordCode == resetPassCode) != null;
        }

        public async Task<UserResponse> Login(LoginRequest req)
        {
            // 1. Find user by user name
            var user = await _userRepo.FindAsync(us => us.Email == req.Email);

            // 2. Check if user exist
            if (user == null)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Không thể tìm thấy tài khoản !",
                };
            }

            // 3. Check if user is activated
            if (!user.IsActivated)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Vui lòng kiểm tra Email đã đăng ký để kích hoạt tài khoản !",
                };
            }

            // 4. Check if login password match
            if (_encryptor.MD5Hash(req.Password) != user.Password)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Sai mật khẩu hoặc tên đăng nhập !",
                };
            }

            return new UserResponse
            {
                User = user,
                IsSuccess = true
            };
        }

        public async Task<UserResponse> Register(RegistRequest req)
        {
            try
            {
                // 1. Check if duplicated account created
                var getUser = await _userRepo.FindAsync(us => us.Email == req.Email && us.IsActivated == true);

                if (getUser != null)
                {
                    return new UserResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Email đã được sử dụng !",
                    };
                }

                // 2. Check pass with confirm pass
                if (!String.Equals(req.Password, req.ConfirmPassWord))
                {
                    return new UserResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Mật khẩu xác nhận không khớp !",
                    };
                }

                await _unitOfWork.BeginTransaction();

                // 3. Create new account
                var user = new Account
                {
                    Name = req.Name,
                    Email = req.Email,
                    IsActivated = false,
                    ActivationCode = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow.Date,
                    UserGroupId = 2,  // CUSTOMER

                    // 4. Encrypt password
                    Password = _encryptor.MD5Hash(req.Password),
                };

                // 5. Add user
                await _userRepo.AddAsync(user);
                await _unitOfWork.CommitTransaction();

                // 6. Send an email activation
                await _emailSender.SendEmailVerificationAsync(user.Email, user.ActivationCode.ToString(), "verify-account");

                return new UserResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                };
            }
        }

        public async Task<UserResponse> ResetPassword(ResetPasswordRequest req)
        {
            try
            {
                // 1. Find user by reset password code
                var user = await _userRepo.FindAsync(us => us.ResetPasswordCode == new Guid(req.ResetPasswordCode) && us.IsActivated == true);

                // 2. Check
                if (user == null)
                {
                    return new UserResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Không tìm thấy tài khoản !",
                    };
                }

                user.Password = _encryptor.MD5Hash(req.NewPassword);
                user.ResetPasswordCode = new Guid();

                await _unitOfWork.CommitTransaction();

                return new UserResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                };
            }
        }

        public async Task<UserResponse> UpdateUser(UserRequest req, int idAccount)
        {
            try
            {
                var userReq = await _userRepo.FindAsync(it => it.Id == idAccount);

                if (userReq == null)
                {
                    return new UserResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Không tìm thấy tài khoản !",
                    };
                }
                await _unitOfWork.BeginTransaction();
                userReq.Name = req.Name;
                userReq.PhoneNumber = req.PhoneNumber;
                userReq.Address = req.Address;
                _userRepo.Update(userReq);
                await _unitOfWork.CommitTransaction();

                return new UserResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                return new UserResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message,
                };
            }
        }

        public async Task<OrderResponse> GetOrders(int userId)
        {
            try
            {
                var userBills = new List<OrderDTO>();
                var orders = _orderRepo.ViewHistoriesOrder(userId);
                foreach (var i in orders)
                {
                    var userBill = new OrderDTO
                    {
                        Id = i.Id,
                        BillId = i.BillId,
                        PaymentName = i.Payment.Type,
                        StatusId = i.StatusId.Value,
                        StatusName = i.Status.Name,
                        DateCreated = i.DateCreate,
                        PhoneNumber = i.PhoneNumber,
                        ShopName = i.Shop.Name,
                        Address = i.Address + ", " + i.City + ", " + i.Country,
                        OrderDetails = _mapper.MapOrderDetails(i.OrderDetails.ToList()),
                    };
                    userBills.Add(userBill);
                }
                return new OrderResponse
                {
                    IsSuccess = true,
                    Orders = userBills
                };
            }
            catch (Exception e)
            {
                return new OrderResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<List<TransactionResponse>> GetTransactions(int userId)
        {
            var result = new List<TransactionResponse>();
            var allTransactions = await _historyTransactionRepo.GetTransactionsOfCustomer(userId);
            var customerName = (await _userRepo.FindAsync(us => us.Id == userId)).Name;
            foreach (var transaction in allTransactions)
            {
                var transactionRes = new TransactionResponse
                {
                    BillId = transaction.BillId,
                    ShopName = (await _shopRepo.FindAsync(s => s.Id == transaction.ShopId)).Name,
                    CustomerName = customerName,
                    TransactionDate = transaction.TransactionDate,
                    Status = transaction.Status.Name,
                };

                if (transaction.StatusId == 1)
                {
                    transactionRes.Money = "-" + transaction.Money.ToString();
                }

                if (transaction.StatusId == 3)
                {
                    transactionRes.Money = "-" + transaction.Money.ToString();
                }

                if (transaction.StatusId == 4)
                {
                    transactionRes.Money = "+" + transaction.Money.ToString();
                }
                result.Add(transactionRes);
            }

            return result.OrderByDescending(rs => rs.TransactionDate).ToList();
        }

        public async Task<int> GetAccountWallet(int userId)
        {
            var user = await _userRepo.FindAsync(us => us.Id == userId);
            var wallet = user.Wallet.HasValue == false ? 0 : user.Wallet.Value;
            return wallet;
            
        }
    }
}