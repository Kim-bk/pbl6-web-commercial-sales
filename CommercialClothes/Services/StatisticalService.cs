using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommercialClothes.Models;
using CommercialClothes.Models.DAL;
using CommercialClothes.Models.DAL.Interfaces;
using CommercialClothes.Models.DAL.Repositories;
using CommercialClothes.Models.DTOs;
using CommercialClothes.Models.DTOs.Requests;
using CommercialClothes.Models.DTOs.Responses;
using CommercialClothes.Services.Base;
using CommercialClothes.Services.Interfaces;

namespace CommercialClothes.Services
{
    public class StatisticalService : BaseService, IStatisticalService
    {
        private readonly IShopRepository _shopRepo;
        private readonly IImageRepository _imageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IItemRepository _itemRepo;
        private readonly IOrderRepository _orderRepo;

        public StatisticalService(IUnitOfWork unitOfWork, IMapperCustom mapper, IShopRepository shopRepository
                                  , IImageRepository imageRepository, IUserRepository userRepo, IItemRepository itemRepository
                                  , IOrderRepository orderRepository) : base(unitOfWork, mapper)
        {
            _shopRepo = shopRepository;
            _imageRepo = imageRepository;
            _itemRepo = itemRepository;
            _userRepo = userRepo;
            _orderRepo = orderRepository;
        }

        public async Task<IntervalResponse> CountOrders(string type, int idUser)
        {
            DateTime dateTime = DateTime.UtcNow;
            var findUser = await _userRepo.FindAsync(sh => sh.Id == idUser);
            var findShop = await _shopRepo.FindAsync(sh => sh.Id == findUser.ShopId.Value);
            var labels1 = new IntervalResponse();
            var lb = new List<string>();
            var dt = new List<int>();
            if (type.Equals("7Days"))
            {
                for (int i = 1; i < 8; i++)
                {
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#1c64f2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn đặt";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("30Days"))
            {
                for (int i = 1; i < 31; i++)
                {
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    if (i % 2 == 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add("Ngày " + date.Day.ToString());
                    }
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#1c64f2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn đặt";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Yesterday"))
            {
                var date = dateTime.AddDays(-1);
                var listItemInMonth = new List<Order>();
                for (int h = 0; h < 25; h++)
                {
                    if (h % 6 != 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add(h + ":00");
                    }
                    if (date.Day >= 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day >= 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#1c64f2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn đặt";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Weekly"))
            {
                IDictionary<string, int> dateOfWeekDic = new Dictionary<string, int>()
                {
                    {"Monday",0},
                    {"Tuesday",1},
                    {"Wednesday",2},
                    {"Thursday",3},
                    {"Friday",4},
                    {"Saturday",5},
                    {"Sunday",6}
                };
                var dateOfWeek = dateTime.DayOfWeek.ToString();
                int interval = 0;
                foreach (var dateow in dateOfWeekDic)
                {
                    if (dateow.Key.Equals(dateOfWeek))
                    {
                        interval = dateow.Value;
                    }
                }
                if (interval == 0)
                {
                    return new IntervalResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Hôm nay là thứ hai không thể thống kê trong tuần được!",
                    };
                }
                if (interval == 1)
                {
                    var date = dateTime.AddDays(-1);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                    labels1.Unit = "";
                    labels1.Color = "#1c64f2";
                    labels1.IsSuccess = true;
                    labels1.Title = "Số lượng đơn đặt";
                    labels1.Labels = lb;
                    labels1.Data = dt;
                    return labels1;
                }
                for (int i = 1; i < interval + 1; i++)
                {
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#1c64f2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn đặt";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            return new IntervalResponse()
            {
                IsSuccess = false,
                ErrorMessage = "Thông số nhập vào lỗi!",
            };
        }

        public async Task<IntervalResponse> CountOrdersCancel(string type, int idUser)
        {
            DateTime dateTime = DateTime.UtcNow;
            var findUser = await _userRepo.FindAsync(sh => sh.Id == idUser);
            var findShop = await _shopRepo.FindAsync(sh => sh.Id == findUser.ShopId.Value);
            var labels1 = new IntervalResponse();
            var lb = new List<string>();
            var dt = new List<int>();
            if (type.Equals("7Days"))
            {
                for (int i = 1; i < 8; i++)
                {
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#7e3af2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("30Days"))
            {
                for (int i = 1; i < 31; i++)
                {
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    if (i % 2 == 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add("Ngày " + date.Day.ToString());
                    }
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#7e3af2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Yesterday"))
            {
                var date = dateTime.AddDays(-1);
                var listItemInMonth = new List<Order>();
                for (int h = 0; h < 25; h++)
                {
                    if (h % 6 != 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add(h + ":00");
                    }
                    if (date.Day >= 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day >= 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#7e3af2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Weekly"))
            {
                IDictionary<string, int> dateOfWeekDic = new Dictionary<string, int>()
                {
                    {"Monday",0},
                    {"Tuesday",1},
                    {"Wednesday",2},
                    {"Thursday",3},
                    {"Friday",4},
                    {"Saturday",5},
                    {"Sunday",6}
                };
                var dateOfWeek = dateTime.DayOfWeek.ToString();
                int interval = 0;
                foreach (var dateow in dateOfWeekDic)
                {
                    if (dateow.Key.Equals(dateOfWeek))
                    {
                        interval = dateow.Value;
                    }
                }
                if (interval == 0)
                {
                    return new IntervalResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Hôm nay là thứ hai không thể thống kê trong tuần được!",
                    };
                }
                if (interval == 1)
                {
                    var date = dateTime.AddDays(-1);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                    labels1.IsSuccess = true;
                    labels1.Title = "Số lượng đơn hủy";
                    labels1.Labels = lb;
                    labels1.Data = dt;
                    return labels1;
                }
                for (int i = 1; i < interval + 1; i++)
                {
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    dt.Add(listItemInMonth.Count);
                }
                labels1.Unit = "";
                labels1.Color = "#7e3af2";
                labels1.IsSuccess = true;
                labels1.Title = "Số lượng đơn hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            return new IntervalResponse()
            {
                IsSuccess = false,
                ErrorMessage = "Thông số nhập vào lỗi!",
            };
        }

        public async Task<IntervalResponse> ListIntervalCancelOrder(string type, int idUser)
        {
            DateTime dateTime = DateTime.UtcNow;
            var findUser = await _userRepo.FindAsync(sh => sh.Id == idUser);
            var findShop = await _shopRepo.FindAsync(sh => sh.Id == findUser.ShopId.Value);
            var labels1 = new IntervalResponse();
            var lb = new List<string>();
            var dt = new List<int>();
            var dateFrom = dateTime.Day;
            if (type.Equals("7Days"))
            {
                for (int i = 1; i < 8; i++)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#057a55";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("30Days"))
            {
                for (int i = 1; i < 31; i++)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    if (i % 2 == 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add("Ngày " + date.Day.ToString());
                    }
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#057a55";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Yesterday"))
            {
                var date = dateTime.AddDays(-1);
                var listItemInMonth = new List<Order>();
                for (int h = 0; h < 25; h++)
                {
                    var total = 0;
                    if (h % 6 != 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add(h + ":00");
                    }
                    if (date.Day >= 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day >= 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#057a55";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Weekly"))
            {
                IDictionary<string, int> dateOfWeekDic = new Dictionary<string, int>()
                {
                    {"Monday",0},
                    {"Tuesday",1},
                    {"Wednesday",2},
                    {"Thursday",3},
                    {"Friday",4},
                    {"Saturday",5},
                    {"Sunday",6}
                };
                var dateOfWeek = dateTime.DayOfWeek.ToString();
                int interval = 0;
                foreach (var dateow in dateOfWeekDic)
                {
                    if (dateow.Key.Equals(dateOfWeek))
                    {
                        interval = dateow.Value;
                    }
                }
                if (interval == 0)
                {
                    return new IntervalResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Hôm nay là thứ hai không thể thống kê trong tuần được!",
                    };
                }
                if (interval == 1)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-1);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    labels1.Unit = "đ";
                    labels1.Color = "#057a55";
                    dt.Add(total);
                    labels1.IsSuccess = true;
                    labels1.Title = "Doanh số hủy";
                    labels1.Labels = lb;
                    labels1.Data = dt;
                    return labels1;
                }
                for (int i = 1; i < interval + 1; i++)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersCancelByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#057a55";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số hủy";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            return new IntervalResponse()
            {
                IsSuccess = false,
                ErrorMessage = "Thông số nhập vào lỗi!",
            };
        }

        public async Task<IntervalResponse> ListItemSoldBy7Days(string type, int idUser)
        {
            DateTime dateTime = DateTime.UtcNow;
            var findUser = await _userRepo.FindAsync(sh => sh.Id == idUser);
            var findShop = await _shopRepo.FindAsync(sh => sh.Id == findUser.ShopId.Value);
            var labels1 = new IntervalResponse();
            var lb = new List<string>();
            var dt = new List<int>();
            var dateFrom = dateTime.Day;
            if (type.Equals("7Days"))
            {
                for (int i = 1; i < 8; i++)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#e02424";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("30Days"))
            {
                for (int i = 1; i < 31; i++)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    if (i % 2 == 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add("Ngày " + date.Day.ToString());
                    }
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#e02424";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Yesterday"))
            {
                var date = dateTime.AddDays(-1);
                var listItemInMonth = new List<Order>();
                for (int h = 0; h < 25; h++)
                {
                    var total = 0;
                    if (h % 6 != 0)
                    {
                        lb.Add("");
                    }
                    else
                    {
                        lb.Add(h + ":00");
                    }
                    if (date.Day >= 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " " + h.ToString(), findShop.Id);
                    }
                    if (date.Day >= 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    if (date.Day < 10 && h < 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString() + " 0" + h.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#e02424";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            if (type.Equals("Weekly"))
            {
                IDictionary<string, int> dateOfWeekDic = new Dictionary<string, int>()
                {
                    {"Monday",0},
                    {"Tuesday",1},
                    {"Wednesday",2},
                    {"Thursday",3},
                    {"Friday",4},
                    {"Saturday",5},
                    {"Sunday",6}
                };

                var dateOfWeek = dateTime.DayOfWeek.ToString();
                int interval = 0;

                foreach (var dateow in dateOfWeekDic)
                {
                    if (dateow.Key.Equals(dateOfWeek))
                    {
                        interval = dateow.Value;
                    }
                }

                if (interval == 0)
                {
                    return new IntervalResponse()
                    {
                        IsSuccess = false,
                        ErrorMessage = "Hôm nay là thứ hai không thể thống kê trong tuần được!",
                    };
                }

                if (interval == 1)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-1);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                    labels1.Unit = "đ";
                    labels1.Color = "#e02424";
                    labels1.IsSuccess = true;
                    labels1.Title = "Doanh số";
                    labels1.Labels = lb;
                    labels1.Data = dt;
                    return labels1;
                }

                for (int i = 1; i < interval + 1; i++)
                {
                    var total = 0;
                    var date = dateTime.AddDays(-i);
                    var listItemInMonth = new List<Order>();
                    lb.Add(date.Day.ToString() + " Tháng " + date.Month.ToString());
                    if (date.Day >= 10)
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString(), findShop.Id);
                    }
                    else
                    {
                        listItemInMonth = await _orderRepo.GetOrdersByDate(date.Year.ToString() + "-" + date.Month.ToString() + "-0" + date.Day.ToString(), findShop.Id);
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            total += item.Price;
                        }
                    }
                    dt.Add(total);
                }
                labels1.Unit = "đ";
                labels1.Color = "#e02424";
                labels1.IsSuccess = true;
                labels1.Title = "Doanh số";
                labels1.Labels = lb;
                labels1.Data = dt;
                return labels1;
            }
            return new IntervalResponse()
            {
                IsSuccess = false,
                ErrorMessage = "Thông số nhập vào lỗi!",
            };
        }

        public async Task<List<StatisticalDTO>> ListItemSoldByInterval(StatisticalRequest req)
        {
            var listStatistical = new List<StatisticalDTO>();
            if (Int32.Parse(req.From.Substring(0, 4)) < Int32.Parse(req.To.Substring(0, 4)))
            {
                for (int i = Int32.Parse(req.From.Substring(0, 4)); i <= Int32.Parse(req.To.Substring(0, 4)); i++)
                {
                    var listItemInMonth = await _orderRepo.GetOrdersByDate(i.ToString(), req.ShopId);
                    if (listItemInMonth == null)
                    {
                        return new List<StatisticalDTO>();
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            if (item.Item.ShopId == req.ShopId)
                            {
                                var itemMonth = new StatisticalDTO
                                {
                                    ItemId = item.ItemId,
                                    NameItem = item.Item.Name,
                                    From = req.From,
                                    To = req.To,
                                    CountSold = item.Quantity.Value,
                                };
                                var existedItem = listStatistical.Where(exi => exi.ItemId == item.ItemId).FirstOrDefault();
                                if (existedItem != null)
                                {
                                    existedItem.CountSold += item.Quantity.Value;
                                    existedItem.Turnover += item.Price;
                                }
                                else
                                {
                                    itemMonth.Turnover = item.Price;
                                    listStatistical.Add(itemMonth);
                                }
                            }
                        }
                    }
                }
            }
            if (req.From.Substring(0, 8).Equals(req.To.Substring(0, 8)))
            {
                for (int i = Int32.Parse(req.From.Substring(8)); i <= Int32.Parse(req.To.Substring(8)); i++)
                {
                    var listItemInMonth = await _orderRepo.GetOrdersByDate(req.From.Substring(0, 8) + i.ToString(), req.ShopId);
                    if (listItemInMonth == null)
                    {
                        return new List<StatisticalDTO>();
                    }
                    foreach (var litem in listItemInMonth)
                    {
                        foreach (var item in litem.OrderDetails)
                        {
                            if (item.Item.ShopId == req.ShopId)
                            {
                                var itemMonth = new StatisticalDTO
                                {
                                    ItemId = item.ItemId,
                                    NameItem = item.Item.Name,
                                    From = req.From,
                                    To = req.To,
                                    CountSold = item.Quantity.Value,
                                };
                                var existedItem = listStatistical.Where(exi => exi.ItemId == item.ItemId).FirstOrDefault();
                                if (existedItem != null)
                                {
                                    existedItem.CountSold += item.Quantity.Value;
                                    existedItem.Turnover += item.Price;
                                }
                                else
                                {
                                    itemMonth.Turnover = item.Price;
                                    listStatistical.Add(itemMonth);
                                }
                            }
                        }
                    }
                }
            }
            return listStatistical;
        }

        public async Task<List<StatisticalDTO>> ListItemsSold(int idShop)
        {
            var request = await _itemRepo.GetItemsByShop(idShop);
            var listStatistical = new List<StatisticalDTO>();
            foreach (var item in request)
            {
                var date = new StatisticalDTO
                {
                    NameItem = item.Name,
                    From = item.DateCreated.ToString(),
                    To = DateTime.UtcNow.ToString(),
                    CountSold = item.Quantity,
                };
                listStatistical.Add(date);
            };
            return listStatistical;
        }

        public async Task<List<StatisticalDTO>> ListItemsSoldByDate(int idShop, string date)
        {
            var listItemInMonth = await _orderRepo.GetOrdersByDate(date, idShop);
            if (listItemInMonth == null)
            {
                return new List<StatisticalDTO>();
            }
            var listStatistical = new List<StatisticalDTO>();
            foreach (var litem in listItemInMonth)
            {
                foreach (var item in litem.OrderDetails)
                {
                    if (item.Item.ShopId == idShop)
                    {
                        var itemMonth = new StatisticalDTO
                        {
                            ItemId = item.ItemId,
                            NameItem = item.Item.Name,
                            CountSold = item.Quantity.Value,
                        };
                        var existedItem = listStatistical.Where(exi => exi.ItemId == item.ItemId).FirstOrDefault();
                        if (existedItem != null)
                        {
                            existedItem.CountSold += item.Quantity.Value;
                        }
                        else
                        {
                            listStatistical.Add(itemMonth);
                        }
                    }
                }
            }
            return listStatistical;
        }
    }
}