using System.Text.RegularExpressions;
using DenyPageCustom.Models;

namespace DenyPageCustom
{
    public static class DenyPageGenerator
    {
        /// <summary>
        /// Патчит официальный Modules/LampaWeb/plugins/telegram_auth_gate.js (stockGateJs) —
        /// подставляет botUsername/serviceName из init.conf вместо плейсхолдеров.
        /// Не переизобретаем разметку/логику гейта — только донастраиваем.
        /// </summary>
        public static string Build(DenyPageConf conf, string stockGateJs)
        {
            string botUsername = ExtractBotUsername(conf.tg_target);
            string serviceName = string.IsNullOrWhiteSpace(conf.service_name) ? "Lampa NextGen" : conf.service_name;

            string js = stockGateJs;
            js = Regex.Replace(js, @"botUsername:\s*'[^']*'", "botUsername: '" + JsEscape(botUsername) + "'");
            js = Regex.Replace(js, @"serviceName:\s*'[^']*'", "serviceName: '" + JsEscape(serviceName) + "'");
            return js;
        }

        private static string ExtractBotUsername(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            raw = raw.Trim();

            var m = Regex.Match(raw, @"(?:t\.me/|domain=)([a-zA-Z0-9_]{5,32})", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups[1].Value;

            return raw.TrimStart('@');
        }

        private static string JsEscape(string value)
            => value.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
    }
}
