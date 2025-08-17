using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_ProductType")]
    public class ProductType
    {
        public ProductType()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }  // Tên loại sản phẩm

        [Required]
        [StringLength(150)]
        public string Alias { get; set; }  // URL thân thiện

        public DateTime CreatedDate { get; set; }  // Ngày tạo
        public DateTime ModifiedDate { get; set; }  // Ngày sửa đổi

        public virtual ICollection<Product> Products { get; set; }
    }
}