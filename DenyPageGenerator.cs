using System.Text;
using System.Text.Json;
using DenyPageCustom.Models;

namespace DenyPageCustom
{
    public static class DenyPageGenerator
    {
        public static string Build(DenyPageConf conf)
        {
            string tgUrl  = NormalizeTgUrl(conf.tg_target);
            bool   hasTg  = !string.IsNullOrWhiteSpace(tgUrl);
            string qrSize = "400";

            string jsTgUrl = Js(tgUrl);
            string jsBadge = Js(string.IsNullOrWhiteSpace(conf.page_badge)    ? "Lampac NextGen" : conf.page_badge);
            string jsTitle = Js(string.IsNullOrWhiteSpace(conf.page_title)    ? "Вход в систему" : conf.page_title);
            string jsSub   = Js(string.IsNullOrWhiteSpace(conf.page_subtitle) ? "Для доступа к серверу введите пароль, выданный администратором." : conf.page_subtitle);
            string jsStep1 = Js(string.IsNullOrWhiteSpace(conf.step1_text)    ? "Введите пароль в поле ниже и нажмите «Войти»." : conf.step1_text);
            string jsStep2 = Js(string.IsNullOrWhiteSpace(conf.step2_text)
                ? (hasTg ? "Если пароля нет — напишите боту <b>" + conf.tg_target + "</b>." : "Если пароля нет — обратитесь к администратору.")
                : conf.step2_text);
            string jsQrCap = Js(string.IsNullOrWhiteSpace(conf.qr_caption)    ? "Нет пароля?" : conf.qr_caption);
            string jsQrSub = Js(string.IsNullOrWhiteSpace(conf.qr_subcaption) ? "Отсканируй QR или нажми кнопку, чтобы написать боту." : conf.qr_subcaption);
            string jsTgBtn = Js(string.IsNullOrWhiteSpace(conf.tg_button_text) ? "Написать боту" : conf.tg_button_text);

            var sb = new StringBuilder();
            sb.AppendLine("// DenyPageCustom v2.5 - auto-generated from init.conf[DenyPage]");
            sb.AppendLine("// DO NOT EDIT - overwritten on config reload.");
            sb.AppendLine();
            sb.AppendLine("var network = new Lampa.Reguest();");
            sb.AppendLine();

            // ── CSS ──────────────────────────────────────────────────────────
            sb.AppendLine("(function(){");
            sb.AppendLine("  var s = document.createElement('style');");
            sb.AppendLine("  s.textContent = [");

            // Base overlay — off-black + subtle cool radial depth
            sb.AppendLine("    '#dpc{position:fixed;inset:0;z-index:99999;display:flex;align-items:center;justify-content:center;font-family:-apple-system,BlinkMacSystemFont,\"SF Pro Text\",\"Segoe UI\",system-ui,sans-serif;padding:3rem 2rem;box-sizing:border-box;overflow:auto;background:#080b12;background-image:radial-gradient(ellipse 65% 70% at 8% 50%,rgba(20,40,90,.28) 0%,transparent 65%)}',");
            // Top 1px accent line
            sb.AppendLine("    '#dpc::before{content:\"\";position:fixed;top:0;left:0;right:0;height:1px;background:linear-gradient(90deg,transparent 0%,rgba(120,160,255,.18) 35%,rgba(120,160,255,.08) 70%,transparent 100%);pointer-events:none;z-index:1}',");
            // Wrapper + entrance
            sb.AppendLine("    '#dpc-w{display:flex;gap:5rem;max-width:1020px;width:100%;align-items:center;animation:dpcIn .55s cubic-bezier(.23,1,.32,1) both}',");
            sb.AppendLine("    '@keyframes dpcIn{from{opacity:0;transform:translateY(22px)}to{opacity:1;transform:translateY(0)}}',");

            // ── Left column ──
            sb.AppendLine("    '#dpc-l{flex:1 1 auto;min-width:0}',");

            // Eyebrow — left-border accent, small caps
            sb.AppendLine("    '#dpc-ey{display:inline-flex;align-items:center;gap:.625rem;font-size:.64rem;letter-spacing:.2em;text-transform:uppercase;color:#3a5070;margin-bottom:2rem;padding-left:.875rem;border-left:1px solid #1e3050}',");
            sb.AppendLine("    '#dpc-ey-dot{width:3px;height:3px;border-radius:50%;background:#2a4060;flex-shrink:0}',");

            // Title — big, tight, bright
            sb.AppendLine("    '#dpc-title{font-size:clamp(2.8rem,5vw,4.2rem);font-weight:700;color:#eef1fa;margin:0 0 1.125rem;line-height:1.03;letter-spacing:-.035em}',");

            // Subtitle — clearly readable
            sb.AppendLine("    '#dpc-sub{font-size:1rem;color:#7a8fa8;margin:0 0 2.5rem;line-height:1.7;max-width:40ch}',");

            // Steps
            sb.AppendLine("    '#dpc-steps{list-style:none;padding:0;margin:0 0 2.25rem;display:flex;flex-direction:column;gap:.75rem}',");
            sb.AppendLine("    '#dpc-steps li{display:flex;align-items:baseline;gap:1rem;line-height:1.6}',");
            sb.AppendLine("    '.dpc-n{font-size:.65rem;font-weight:600;color:#243550;flex-shrink:0;letter-spacing:.04em;padding-top:.15em;min-width:2ch}',");
            sb.AppendLine("    '.dpc-t{font-size:.9rem;color:#627085;line-height:1.6}',");
            sb.AppendLine("    '.dpc-t b{color:#8a9db8;font-weight:500}',");

            // Thin separator
            sb.AppendLine("    '#dpc-sep{height:1px;background:linear-gradient(90deg,rgba(255,255,255,.07) 0%,transparent 65%);margin-bottom:2rem}',");

            // Input wrapper
            sb.AppendLine("    '#dpc-iw{display:flex;flex-direction:column;gap:.625rem}',");

            // Input
            sb.AppendLine("    '#dpc-inp{width:100%;box-sizing:border-box;background:rgba(255,255,255,.038);border:1px solid rgba(255,255,255,.09);border-radius:10px;padding:.95rem 1.125rem;font-size:1rem;color:#eef1fa;outline:none;-webkit-appearance:none;appearance:none;font-family:inherit;transition:border-color .2s ease-out,background .2s ease-out,box-shadow .2s ease-out}',");
            sb.AppendLine("    '#dpc-inp::placeholder{color:#243040}',");
            sb.AppendLine("    '#dpc-inp:focus{border-color:rgba(255,255,255,.24);background:rgba(255,255,255,.055);box-shadow:0 0 0 3px rgba(255,255,255,.04)}',");

            // Button — off-white, liquid-glass inner edge, tactile scale
            sb.AppendLine("    '#dpc-btn{padding:.9rem 1.875rem;background:#eef1fa;color:#080b12;border:none;border-radius:10px;font-size:.925rem;font-weight:600;cursor:pointer;font-family:inherit;letter-spacing:-.015em;align-self:flex-start;box-shadow:inset 0 1px 0 rgba(255,255,255,.5);transition:opacity .18s ease-out,transform .1s ease-out}',");
            sb.AppendLine("    '#dpc-btn:disabled{opacity:.28;cursor:default;transform:none}',");

            // Hover — pointer devices only (Emil: gate behind media query)
            sb.AppendLine("    '@media(hover:hover) and (pointer:fine){#dpc-btn:not(:disabled):hover{opacity:.84}#dpc-btn:not(:disabled):active{transform:scale(.97)}#dpc-tgbtn:hover{border-color:rgba(255,255,255,.16);color:#7a90aa}}',");
            // Touch active
            sb.AppendLine("    '@media(hover:none){#dpc-btn:not(:disabled):active{transform:scale(.97)}}',");

            // Error / success message
            sb.AppendLine("    '#dpc-err{font-size:.82rem;min-height:1.15em;line-height:1.5;padding-left:.125rem;transition:color .2s ease-out}',");

            // ── Right column (QR) ──
            sb.AppendLine("    '#dpc-r{flex:0 0 auto;display:flex;flex-direction:column;align-items:center;gap:1.25rem;padding:2rem 1.625rem;border:1px solid rgba(255,255,255,.07);border-radius:20px;background:rgba(255,255,255,.018);box-shadow:inset 0 1px 0 rgba(255,255,255,.06)}',");
            sb.AppendLine("    '#dpc-qr{border-radius:10px;overflow:hidden;background:#fff;padding:9px;line-height:0;flex-shrink:0}',");
            sb.AppendLine("    '#dpc-qr img{display:block;width:clamp(140px,14vw,210px);height:auto}',");
            // Text group inside QR panel
            sb.AppendLine("    '#dpc-rtxt{display:flex;flex-direction:column;align-items:center;gap:1rem}',");
            sb.AppendLine("    '#dpc-qrcap{font-size:.9rem;font-weight:600;color:#c5cedf;text-align:center;letter-spacing:-.015em}',");
            sb.AppendLine("    '#dpc-qrsub{font-size:.76rem;color:#445060;text-align:center;max-width:160px;line-height:1.6}',");
            sb.AppendLine("    '#dpc-tgbtn{padding:.6rem 1.25rem;background:transparent;border:1px solid rgba(255,255,255,.08);border-radius:8px;color:#4a6080;text-decoration:none;font-size:.8rem;text-align:center;display:inline-block;font-family:inherit;transition:border-color .18s ease-out,color .18s ease-out}',");

            // ── Responsive ──
            // Tablet ≤900px: QR сверху (order:-1), внутри — строка: изображение слева, текст справа
            sb.AppendLine("    '@media(max-width:900px){#dpc{padding:2.5rem 1.75rem;align-items:flex-start}#dpc-w{flex-direction:column;gap:2.5rem;max-width:100%}#dpc-l{max-width:100%}#dpc-sub{max-width:none}#dpc-btn{align-self:auto;width:100%}#dpc-r{order:-1;flex-direction:row;align-items:center;gap:1.25rem;width:100%;padding:1.25rem 1.5rem}#dpc-rtxt{align-items:flex-start;gap:.75rem}#dpc-qrcap{text-align:left}#dpc-qrsub{text-align:left;max-width:none}#dpc-ey{margin-bottom:1.5rem}}',");
            // Mobile ≤580px
            sb.AppendLine("    '@media(max-width:580px){#dpc{padding:1.75rem 1.25rem}#dpc-title{font-size:2.4rem}#dpc-sub{font-size:.95rem}#dpc-r{gap:1rem;padding:1rem 1.25rem}#dpc-qr img{width:100px}#dpc-qrcap{font-size:.82rem}#dpc-qrsub{font-size:.72rem}#dpc-inp{font-size:16px;padding:1rem}}',");
            // 2K+ (≥2000px): larger QR
            sb.AppendLine("    '@media(min-width:2000px){#dpc-qr img{width:clamp(280px,14vw,360px)}#dpc-r{padding:2.25rem 2rem;gap:1.5rem}#dpc-rtxt{gap:1.125rem}#dpc-qrcap{font-size:1rem}#dpc-qrsub{font-size:.85rem;max-width:200px}#dpc-tgbtn{padding:.7rem 1.5rem;font-size:.875rem}}'");

            sb.AppendLine("  ].join('');");
            sb.AppendLine("  document.head.appendChild(s);");
            sb.AppendLine("})();");
            sb.AppendLine();

            // ── addDevice ────────────────────────────────────────────────────
            sb.AppendLine("function addDevice(message) {");
            sb.AppendLine("  if (document.getElementById('dpc')) return;");
            sb.AppendLine();

            string rightColHtml = "";
            if (hasTg && conf.show_qr)
            {
                rightColHtml =
                    "<div id=\\\"dpc-r\\\">" +
                    "<div id=\\\"dpc-qr\\\"><img src=\\\"https://api.qrserver.com/v1/create-qr-code/?size=" + qrSize + "x" + qrSize + "&data=' + encodeURIComponent(" + jsTgUrl + ") + '&margin=4\\\" width=\\\"" + qrSize + "\\\" height=\\\"" + qrSize + "\\\" loading=\\\"lazy\\\" /></div>" +
                    "<div id=\\\"dpc-rtxt\\\">" +
                    "<div id=\\\"dpc-qrcap\\\">' + " + jsQrCap + " + '</div>" +
                    "<div id=\\\"dpc-qrsub\\\">' + " + jsQrSub + " + '</div>" +
                    "<a id=\\\"dpc-tgbtn\\\" href=\\\"' + " + jsTgUrl + " + '\\\" target=\\\"_blank\\\" rel=\\\"noopener\\\">' + " + jsTgBtn + " + '</a>" +
                    "</div>" +
                    "</div>";
            }

            sb.AppendLine("  var html = ''");
            sb.AppendLine("    + '<div id=\"dpc\">'");
            sb.AppendLine("    + '<div id=\"dpc-w\">'");
            sb.AppendLine("    + '<div id=\"dpc-l\">'");
            sb.AppendLine("    + '<div id=\"dpc-ey\"><span id=\"dpc-ey-dot\"></span>' + " + jsBadge + " + '</div>'");
            sb.AppendLine("    + '<h1 id=\"dpc-title\">' + " + jsTitle + " + '</h1>'");
            sb.AppendLine("    + '<p id=\"dpc-sub\">' + " + jsSub + " + '</p>'");
            sb.AppendLine("    + '<ul id=\"dpc-steps\">'");
            sb.AppendLine("    + '<li><span class=\"dpc-n\">01</span><span class=\"dpc-t\">' + " + jsStep1 + " + '</span></li>'");
            sb.AppendLine("    + '<li><span class=\"dpc-n\">02</span><span class=\"dpc-t\">' + " + jsStep2 + " + '</span></li>'");
            sb.AppendLine("    + '</ul>'");
            sb.AppendLine("    + '<div id=\"dpc-sep\"></div>'");
            sb.AppendLine("    + '<div id=\"dpc-iw\">'");
            sb.AppendLine("    + '<input id=\"dpc-inp\" type=\"text\" inputmode=\"text\" placeholder=\"Введите пароль\" autocomplete=\"off\" autocapitalize=\"off\" autocorrect=\"off\" spellcheck=\"false\" />'");
            sb.AppendLine("    + '<button id=\"dpc-btn\">Войти</button>'");
            sb.AppendLine("    + '<div id=\"dpc-err\"></div>'");
            sb.AppendLine("    + '</div>'");
            sb.AppendLine("    + '</div>'");
            if (!string.IsNullOrEmpty(rightColHtml))
                sb.AppendLine("    + '" + rightColHtml + "'");
            sb.AppendLine("    + '</div>'");
            sb.AppendLine("    + '</div>';");
            sb.AppendLine();

            sb.AppendLine("  document.body.insertAdjacentHTML('beforeend', html);");
            sb.AppendLine();

            sb.AppendLine("  var _inp  = document.getElementById('dpc-inp');");
            sb.AppendLine("  var _btn  = document.getElementById('dpc-btn');");
            sb.AppendLine("  var _err  = document.getElementById('dpc-err');");
            sb.AppendLine("  var _wrap = document.getElementById('dpc');");
            sb.AppendLine();

            sb.AppendLine("  function keepFocus() {");
            sb.AppendLine("    if (!_inp) return;");
            sb.AppendLine("    if (document.activeElement !== _inp) _inp.focus();");
            sb.AppendLine("  }");
            sb.AppendLine();

            sb.AppendLine("  function doLogin() {");
            sb.AppendLine("    var val = _inp.value;");
            sb.AppendLine("    if (!val) {");
            sb.AppendLine("      _err.style.color = '#d95f5f';");
            sb.AppendLine("      _err.textContent = 'Введите пароль';");
            sb.AppendLine("      keepFocus();");
            sb.AppendLine("      return;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    _btn.disabled = true;");
            sb.AppendLine("    _btn.textContent = '...';");
            sb.AppendLine("    _err.textContent = '';");
            sb.AppendLine();
            sb.AppendLine("    network.clear();");
            sb.AppendLine("    var u = '{localhost}/testaccsdb';");
            sb.AppendLine("    u = Lampa.Utils.addUrlComponent(u, 'account_email=' + encodeURIComponent(val));");
            sb.AppendLine("    var uid = Lampa.Storage.get('lampac_unic_id', '');");
            sb.AppendLine("    if (uid) u = Lampa.Utils.addUrlComponent(u, 'uid=' + encodeURIComponent(uid));");
            sb.AppendLine("    network.silent(u, function(result) {");
            sb.AppendLine("      if (result.success) {");
            sb.AppendLine("        if (result.uid) {");
            sb.AppendLine("          _err.style.color = '#4ec87a';");
            sb.AppendLine("          _err.textContent = 'Аккаунт создан. Пароль: ' + result.uid;");
            sb.AppendLine("          Lampa.Storage.set('lampac_unic_id', result.uid);");
            sb.AppendLine("          setTimeout(function() {");
            sb.AppendLine("            localStorage.removeItem('activity');");
            sb.AppendLine("            window.location.href = '/';");
            sb.AppendLine("          }, 3000);");
            sb.AppendLine("        } else {");
            sb.AppendLine("          Lampa.Storage.set('lampac_unic_id', val);");
            sb.AppendLine("          localStorage.removeItem('activity');");
            sb.AppendLine("          window.location.href = '/';");
            sb.AppendLine("        }");
            sb.AppendLine("      } else {");
            sb.AppendLine("        _err.style.color = '#d95f5f';");
            sb.AppendLine("        _err.textContent = 'Неправильный пароль';");
            sb.AppendLine("        _btn.disabled = false;");
            sb.AppendLine("        _btn.textContent = 'Войти';");
            sb.AppendLine("        keepFocus();");
            sb.AppendLine("      }");
            sb.AppendLine("    }, function() {");
            sb.AppendLine("      _err.style.color = '#d95f5f';");
            sb.AppendLine("      _err.textContent = 'Ошибка соединения';");
            sb.AppendLine("      _btn.disabled = false;");
            sb.AppendLine("      _btn.textContent = 'Войти';");
            sb.AppendLine("      keepFocus();");
            sb.AppendLine("    }, { code: val });");
            sb.AppendLine("  }");
            sb.AppendLine();

            sb.AppendLine("  _btn.addEventListener('click', function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    e.stopPropagation();");
            sb.AppendLine("    doLogin();");
            sb.AppendLine("  });");
            sb.AppendLine();

            sb.AppendLine("  _inp.addEventListener('keydown', function(e) {");
            sb.AppendLine("    e.stopPropagation();");
            sb.AppendLine("    if (e.key === 'Enter') { e.preventDefault(); doLogin(); }");
            sb.AppendLine("  });");
            sb.AppendLine();

            sb.AppendLine("  _inp.addEventListener('keyup',    function(e) { e.stopPropagation(); });");
            sb.AppendLine("  _inp.addEventListener('keypress', function(e) { e.stopPropagation(); });");
            sb.AppendLine("  _inp.addEventListener('click',    function(e) { e.stopPropagation(); });");
            sb.AppendLine();

            sb.AppendLine("  _inp.addEventListener('input', function(e) {");
            sb.AppendLine("    e.stopPropagation();");
            sb.AppendLine("    _err.textContent = '';");
            sb.AppendLine("  });");
            sb.AppendLine();

            sb.AppendLine("  _wrap.addEventListener('click', function() { keepFocus(); });");
            sb.AppendLine();

            sb.AppendLine("  document.addEventListener('keydown', function(e) {");
            sb.AppendLine("    if (!document.getElementById('dpc')) return;");
            sb.AppendLine("    if (document.activeElement !== _inp && e.key && e.key.length === 1) _inp.focus();");
            sb.AppendLine("  }, true);");
            sb.AppendLine();

            sb.AppendLine("  setTimeout(keepFocus, 150);");
            sb.AppendLine("  setTimeout(keepFocus, 500);");
            sb.AppendLine("  setTimeout(keepFocus, 1000);");
            sb.AppendLine("  setInterval(keepFocus, 1000);");
            sb.AppendLine("}");
            sb.AppendLine();

            // ── checkAutch ───────────────────────────────────────────────────
            sb.AppendLine("function checkAutch() {");
            sb.AppendLine("  var url = '{localhost}/testaccsdb';");
            sb.AppendLine("  var email = Lampa.Storage.get('account_email');");
            sb.AppendLine("  if (email) url = Lampa.Utils.addUrlComponent(url, 'account_email=' + encodeURIComponent(email));");
            sb.AppendLine("  var uid = Lampa.Storage.get('lampac_unic_id', '');");
            sb.AppendLine("  if (uid) url = Lampa.Utils.addUrlComponent(url, 'uid=' + encodeURIComponent(uid));");
            sb.AppendLine("  var token = '{token}';");
            sb.AppendLine("  if (token) url = Lampa.Utils.addUrlComponent(url, 'token={token}');");
            sb.AppendLine("  network.silent(url, function(res) {");
            sb.AppendLine("    if (res.accsdb) {");
            sb.AppendLine("      window.start_deep_link = { component: 'denypages', page: 1, url: '' };");
            sb.AppendLine("      if (res.newuid) { Lampa.Storage.set('lampac_unic_id', Lampa.Utils.uid(8).toLowerCase()); }");
            sb.AppendLine("      window.sync_disable = true;");
            sb.AppendLine("      document.getElementById('app').style.display = 'none';");
            sb.AppendLine("      var _pw = document.getElementById('loading-element');");
            sb.AppendLine("      if (_pw) _pw.style.display = 'none';");
            sb.AppendLine("      if (!res.denymsg) { setTimeout(function() { addDevice(res.msg); }, 500); }");
            sb.AppendLine("    } else {");
            sb.AppendLine("      network.clear(); network = null;");
            sb.AppendLine("    }");
            sb.AppendLine("  }, function() {});");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("checkAutch();");

            return sb.ToString();
        }

        private static string NormalizeTgUrl(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            raw = raw.Trim();
            if (raw.StartsWith("https://") || raw.StartsWith("http://") || raw.StartsWith("tg://"))
                return raw;
            return $"https://t.me/{raw.TrimStart('@')}";
        }

        private static string Js(string? value)
            => JsonSerializer.Serialize(value ?? "");
    }
}
