using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    public class DiscountCode
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; } // Mã giảm giá

        [Required]
        public double DiscountPercent { get; set; } // % giảm giá

        [Required]
        public int MaxDiscountAmount { get; set; } // Số tiền giảm tối đa (VND)

        [Required]
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn

        public bool IsUsed { get; set; } // Mã đã được sử dụng hay chưa
    }
}