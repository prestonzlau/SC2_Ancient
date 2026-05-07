using System.Text;

namespace SC2.SC2Code.Extensions;

public static class StringExtensions
    {
        /// <summary>Converts PascalCase to snake_case to match CARDS.md portrait filenames.</summary>
        public static string ToSnakeCase(this string pascalCase)
        {
            if (string.IsNullOrEmpty(pascalCase)) return pascalCase;
            var sb = new StringBuilder();
            for (int i = 0; i < pascalCase.Length; i++)
            {
                char c = pascalCase[i];
                if (char.IsUpper(c) && i > 0 && (char.IsLower(pascalCase[i - 1]) || (i + 1 < pascalCase.Length && char.IsLower(pascalCase[i + 1]))))
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }
        private static string ResPath(params string[] parts)
        {
            return Path.Join(parts).Replace('\\', '/');
        }

        public static string ImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", path);
        }

        public static string CardImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "card_portraits", path);
        }

        public static string BigCardImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "card_portraits", "big", path);
        }

        public static string PowerImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "powers", path);
        }

        public static string BigPowerImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "powers", "big", path);
        }

        public static string RelicImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "relics", path);
        }

        public static string BigRelicImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "relics", "big", path);
        }

        public static string CharacterUiPath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "charui", path);
        }
        
        public static string AncientImagePath(this string path)
        {
            return ResPath(MainFile.ModId, "images", "ancients", path);
        }
    }