using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }          // Id của đơn hàng
        public string OrderCode { get; set; }    // Mã đơn hàng
        public DateTime CreatedDate { get; set; } // Ngày đặt hàng
        public decimal TotalAmount { get; set; } // Tổng tiền đơn hàng
        public decimal ShippingFee { get; set; } // Phí giao hàng
        public decimal? DiscountAmount { get; set; } // Số tiền giảm giá
        public decimal TotalFinal { get; set; } // Tổng tiền cuối cùng
        public int Status { get; set; }          // Tình trạng đơn hàng

        // Thông tin từ OrderDetail
        public List<OrderDetailViewModel> OrderDetails { get; set; } = new List<OrderDetailViewModel>();
    }

    public class OrderDetailViewModel
    {
        public string ProductName { get; set; } // Tên sản phẩm
        public decimal Price { get; set; }     // Giá sản phẩm
        public int Quantity { get; set; }      // Số lượng
        public bool HasBottleFee { get; set; } // Có phí vỏ hay không
        public decimal TotalPrice => (Price * Quantity) + (HasBottleFee ? 40000 * Quantity : 0); // Thành tiền bao gồm cả phí vỏ
    }
}