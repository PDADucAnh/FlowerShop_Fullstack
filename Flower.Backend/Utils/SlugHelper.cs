using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Flower.Backend.Utils
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string str = RemoveSign4VietnameseString(text.ToLowerInvariant());
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            str = Regex.Replace(str, @"-+", "-");
            return str.Trim('-');
        }

        public static string GenerateSku(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "PROD-" + new Random().Next(1000, 9999);

            string unsignedName = RemoveSign4VietnameseString(name);
            var words = unsignedName.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var prefix = string.Join("", words.Select(w => char.ToUpper(w[0])));
            if (string.IsNullOrEmpty(prefix))
                prefix = "PROD";

            var random = new Random();
            return $"{prefix}-{random.Next(1000, 9999)}";
        }

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyYoOuUrRuU",
            "áàảãạăắằẳẵặâấầẩẫậ",
            "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ",
            "éèẻẽẹêếềểễệ",
            "ÉÈẺẼẸÊẾỀỂỄỆ",
            "óòỏõọôốồổỗộơớờởỡợ",
            "ÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢ",
            "úùủũụưứừửữự",
            "ÚÙỦŨỤƯỨỪỬỮỰ",
            "íìỉĩị",
            "ÍÌỈĨỊ",
            "đ",
            "Đ",
            "ýỳỷỹỵ",
            "ÝỲỶỸỴ"
        };

        private static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                {
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                }
            }
            return str;
        }
    }
}
