using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHangOnline.Models.EF;
using System.Text.RegularExpressions;
using WebBanHangOnline.Models;
using System.Collections.Generic;
using System.Text;

namespace WebBanHangOnline.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetResponse(string message)
        {
            try
            {
                message = NormalizeText(message.ToLower().Trim());

                // Kiểm tra câu hỏi về sản phẩm bán chạy
                if (message.Contains("bán chạy") || message.Contains("hot") || message.Contains("nhiều") || message.Contains("top"))
                {
                    var topProducts = db.Products
                        .OrderByDescending(p => p.ViewCount)
                        .Take(3)
                        .Select(p => new
                        {
                            Name = p.Title,
                            Link = "/chi-tiet/" + p.Alias + "-p" + p.Id,
                            Price = p.Price,
                            Image = p.ProductImage.FirstOrDefault().Image,
                            Id = p.Id
                        })
                        .ToList();

                    if (topProducts.Any())
                    {
                        string response = "🔥 Đây là 3 sản phẩm bán chạy nhất của chúng tôi:\n\n";
                        foreach (var product in topProducts)
                        {
                            response += $"📦 {product.Name}\n";
                            response += $"💰 Giá: {product.Price:0,0}đ\n";
                            response += $"🔗 <a href='{product.Link}' target='_blank'>Xem chi tiết sản phẩm</a>\n";
                            response += "----------------------------------------\n\n";
                        }
                        return Json(new { success = true, message = response, isHtml = true });
                    }
                }

                // Tìm câu trả lời từ database với độ tương đồng cao nhất
                var bestMatch = db.ChatMessages
                    .Where(x => x.IsActive)
                    .AsEnumerable() // Chuyển về xử lý trong memory để có thể normalize text
                    .Select(x => new
                    {
                        Message = x,
                        Similarity = CalculateSimilarity(message, NormalizeText(x.Question.ToLower()))
                    })
                    .OrderByDescending(x => x.Similarity)
                    .FirstOrDefault();

                if (bestMatch != null && bestMatch.Similarity >= 0.3) // Giảm ngưỡng xuống 30%
                {
                    return Json(new { success = true, message = bestMatch.Message.Answer });
                }

                // Nếu không tìm thấy câu trả lời phù hợp
                return Json(new
                {
                    success = true,
                    message = "Xin lỗi, tôi không hiểu rõ câu hỏi của bạn. Bạn có thể:\n" +
                             "1. Đặt câu hỏi ngắn gọn hơn\n" +
                             "2. Sử dụng từ khóa chính xác hơn\n" +
                             "3. Hoặc liên hệ với nhân viên hỗ trợ để được tư vấn chi tiết"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại sau." });
            }
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Chuyển về chữ thường và bỏ dấu cách thừa
            text = text.ToLower().Trim();
            
            // Chuẩn hóa Unicode và dấu thanh
            text = RemoveDiacritics(text);
            
            // Loại bỏ dấu câu và ký tự đặc biệt
            text = Regex.Replace(text, @"[.,!?;:""'()]", "");
            
            // Loại bỏ từ không cần thiết
            var stopWords = new[] { 
                "cho", "toi", "tôi", "hoi", "hỏi", "ve", "về", "van de", "vấn đề", 
                "cai", "cái", "nay", "này", "la", "là", "gi", "gì", "the", "thế", 
                "nao", "nào", "voi", "với", "duoc", "được", "khong", "không",
                "biet", "biết", "muon", "muốn", "can", "cần", "hay", "hoac", "hoặc"
            };
            
            foreach (var word in stopWords)
            {
                text = Regex.Replace(text, $@"\b{word}\b", "", RegexOptions.IgnoreCase);
            }
            
            // Chuẩn hóa khoảng trắng
            text = Regex.Replace(text, @"\s+", " ").Trim();
            
            return text;
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private double CalculateSimilarity(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;

            // Tách thành các từ
            var words1 = s1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var words2 = s2.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Tính số từ giống nhau (bao gồm cả từ gần giống)
            int commonWords = 0;
            foreach (var w1 in words1)
            {
                foreach (var w2 in words2)
                {
                    if (w1.Length > 1 && w2.Length > 1) // Giảm độ dài tối thiểu xuống 2 ký tự
                    {
                        // So sánh cả từ gốc và từ đã bỏ dấu
                        if (w2.Contains(w1) || w1.Contains(w2) || 
                            RemoveDiacritics(w2).Contains(RemoveDiacritics(w1)) || 
                            RemoveDiacritics(w1).Contains(RemoveDiacritics(w2)) ||
                            LevenshteinDistance(w1, w2) <= 2)
                        {
                            commonWords++;
                            break;
                        }
                    }
                }
            }

            // Tính điểm dựa trên tỷ lệ từ giống nhau và độ dài câu
            double lengthScore = 1.0 - Math.Abs(words1.Length - words2.Length) / (double)Math.Max(words1.Length, words2.Length);
            double wordScore = (double)commonWords / Math.Max(words1.Length, words2.Length);

            return (wordScore * 0.8 + lengthScore * 0.2); // Tăng trọng số cho từ giống nhau
        }

        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,      // Deletion
                        d[i, j - 1] + 1),     // Insertion
                        d[i - 1, j - 1] + cost); // Substitution
                }
            }

            return d[s1.Length, s2.Length];
        }
    }
} 