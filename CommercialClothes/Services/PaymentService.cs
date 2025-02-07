﻿using CommercialClothes.Commons.VNPay;
using CommercialClothes.Models.DAL;
using CommercialClothes.Models.DAL.Repositories;
using CommercialClothes.Models.DTOs.Requests;
using CommercialClothes.Services.Base;
using CommercialClothes.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Model.DTOs;
using Model.Commons.Paypal;
using PayPal;
using Stripe.TestHelpers;
using Stripe;
using System.Collections.Generic;
using CommercialClothes.Models;

namespace CommercialClothes.Services
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IUserRepository _userRepo;
        private readonly VNPaySettings _VNPaySettings;
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _context;
        private readonly IAdminService _adminService;
        private readonly IOrderRepository _orderRepository;
        private const int currency = 23000;

        public PaymentService(IUserRepository userRepo, IUnitOfWork unitOfWork
            , IMapperCustom mapper, IOptions<VNPaySettings> vnPay
            , IOrderService orderService, IHttpContextAccessor context
            , IAdminService adminService, IOptions<StripeSettings> stripe
            , IOrderRepository orderRepository) : base(unitOfWork, mapper)
        {
            _userRepo = userRepo;
            _VNPaySettings = vnPay.Value;
            _stripeSettings = stripe.Value;
            _orderService = orderService;
            _context = context;
            _adminService = adminService;
            _orderRepository = orderRepository;
        }

        #region Stripe

        public async Task<bool> StripeCheckOut(OrderRequest request, int userId)
        {
            var customers = new Stripe.CustomerService();
            var charges = new ChargeService();
            var custom = customers.Create(new CustomerCreateOptions
            {
                Email = "random@gmail.com",
                Source = _stripeSettings.PubKey,
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = 400,
                Description = "Test Payment",
                Currency = "usd",
            });
            
            if (charge.Status == "succeeded")
            {
                string balanceTransactionId = charge.BalanceTransactionId;
                return true;
            }    

            return false;
        }

        #endregion Stripe

        #region VNPay

        public async Task<string> VNPayCheckOut(OrderRequest request, int userId)
        {
            // 1. Add order
            var orderId = await _orderService.AddOrder(request, userId);

            // 2. Find user
            var user = await _userRepo.FindAsync(us => us.Id == userId);

            // Setting variables vnPay
            string vnp_ReturnUrl = _VNPaySettings.ReturnUrl; //URL nhan ket qua tra ve
            string vnp_Url = _VNPaySettings.Url; //URL thanh toan cua VNPAY
            string vnp_TmnCode = _VNPaySettings.TmnCode; //Ma website
            string vnp_HashSecret = _VNPaySettings.HashSecret; //Chuoi bi mat

            //Build URL for VNPAY
            VNPayLibrary vnpay = new();
            vnpay.AddRequestData("vnp_Version", VNPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (request.Total * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_context));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Đơn hàng: " + orderId);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId);
            vnpay.AddRequestData("vnp_BankCode", request.BankCode);

            //Billing
            vnpay.AddRequestData("vnp_Bill_Mobile", request.PhoneNumber.Trim());
            var fullName = user.Name?.Trim();
            if (!String.IsNullOrEmpty(fullName))
            {
                var indexof = fullName.IndexOf(' ');
                vnpay.AddRequestData("vnp_Bill_FirstName", fullName.Substring(0, indexof));
                vnpay.AddRequestData("vnp_Bill_LastName", fullName.Substring(indexof + 1, fullName.Length - indexof - 1));
            }

            vnpay.AddRequestData("vnp_Bill_Address", request.Address.Trim());
            vnpay.AddRequestData("vnp_Bill_City", request.City.Trim());
            vnpay.AddRequestData("vnp_Bill_Country", request.Country.Trim());
            //vnpay.AddRequestData("vnp_Bill_State", "");

            // Invoice
            vnpay.AddRequestData("vnp_Inv_Phone", request.PhoneNumber.Trim());
            //vnpay.AddRequestData("vnp_Inv_Email", txt_inv_email.Text.Trim());
            // vnpay.AddRequestData("vnp_Inv_Customer", txt_inv_customer.Text.Trim());
            vnpay.AddRequestData("vnp_Inv_Address", request.Address.Trim());
            vnpay.AddRequestData("vnp_Inv_Company", request.City);
            vnpay.AddRequestData("vnp_Inv_Taxcode", request.Country);
            //vnpay.AddRequestData("vnp_Inv_Type", cbo_inv_type.SelectedItem.Value);
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            if (paymentUrl != "")
            {
                // add money to admin wallet (just have one admin)
                // when user first buy, the status is 1 - "Chờ xác nhận"
                return paymentUrl;
            }

            return "Error ! Vui lòng liên hệ IT để được hỗ trợ !";
        }
        #endregion VNPay

        public async Task<bool> CODCheckOut(OrderRequest request, int userId)
        {
            var orderId = await _orderService.AddOrder(request, userId);
            return await PaySuccess(orderId);
        }

        public async Task<bool> PaySuccess(string orderInfo)
        {
            var rs = orderInfo.Split("-");
            var listOrderId = new List<string>();
            for (int i = 1; i < rs.Length; i++)
            {
                listOrderId.Add(rs[i]);
            }

            // Change status of each order to IsBought
            foreach (var orderId in listOrderId)
            {
                var order = await _orderRepository.FindAsync(ord => ord.Id == Convert.ToInt32(orderId));
                order.IsSuccess = true;

                // Save transactions
                var transactionDto = new TransactionDTO
                {
                    BillId = order.BillId,
                    CustomerId = order.AccountId,
                    Money = order.Total.HasValue == false ? 0 : order.Total.Value,
                    ShopId = order.ShopId,
                };

                await _adminService.ManageTransaction(transactionDto, 1);
            }
            await _unitOfWork.CommitTransaction();
            return true;
        }
    }
}