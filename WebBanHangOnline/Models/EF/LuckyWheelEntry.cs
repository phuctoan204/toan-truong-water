using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    public class LuckyWheelEntry
    {
            [Key]
            public int Id { get; set; }

            [Required, StringLength(100)]
            public string Name { get; set; }

            [Required, EmailAddress, StringLength(100)]
            public string Email { get; set; }

            public int DiscountCodeId { get; set; } // Mã giảm giá được nhận

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }
    }
