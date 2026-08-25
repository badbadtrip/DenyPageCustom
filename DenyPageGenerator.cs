using System.Text;
using System.Text.Json;
using DenyPageCustom.Models;

namespace DenyPageCustom
{
    public static class DenyPageGenerator
    {
        public static string Build(DenyPageConf conf)
        {
            string tgUrl = NormalizeTgUrl(conf.tg_target);
            bool   hasTg = !string.IsNullOrWhiteSpace(tgUrl);
            string qrSize = "400";

            string jsTgUrl  = Js(tgUrl);
            string jsBadge  = Js(string.IsNullOrWhiteSpace(conf.page_badge)     ? "Lampac NextGen" : conf.page_badge);
            string jsTitle  = Js(string.IsNullOrWhiteSpace(conf.page_title)     ? "Вход в Lampa" : conf.page_title);
            string jsWarn   = Js(string.IsNullOrWhiteSpace(conf.page_subtitle)  ? "Доступ ограничен. Пароль можно получить у администратора." : conf.page_subtitle);
            string jsHint   = Js(string.IsNullOrWhiteSpace(conf.step1_text)     ? "Нажмите «Войти» и введите пароль." : conf.step1_text);
            string jsQrCap  = Js(string.IsNullOrWhiteSpace(conf.qr_caption)     ? "Нет пароля?" : conf.qr_caption);
            string jsQrSub  = Js(string.IsNullOrWhiteSpace(conf.qr_subcaption)  ? "Отсканируй QR или нажми кнопку, чтобы написать боту." : conf.qr_subcaption);
            string jsTgBtn  = Js(string.IsNullOrWhiteSpace(conf.tg_button_text) ? "Открыть Telegram" : conf.tg_button_text);
            string jsTgName = Js(string.IsNullOrWhiteSpace(conf.tg_target)      ? "" : conf.tg_target);

            var sb = new StringBuilder();
            sb.AppendLine("// DenyPageCustom v4.0 - auto-generated from init.conf[DenyPage]");
            sb.AppendLine("// DO NOT EDIT - overwritten on config reload.");
            sb.AppendLine();
            sb.AppendLine("var network = new Lampa.Reguest();");
            sb.AppendLine();

            // ── CSS ──────────────────────────────────────────────────────────
            sb.AppendLine("(function(){");
            sb.AppendLine("  var s = document.createElement('style');");
            sb.AppendLine("  s.textContent = [");

            sb.AppendLine("    '#dpc{position:fixed;inset:0;z-index:99999;display:flex;align-items:center;justify-content:center;font-family:\"Manrope\",\"Segoe UI\",system-ui,sans-serif;padding:24px;box-sizing:border-box;overflow:auto;background:#1e1f21}',");
            sb.AppendLine("    '@keyframes dpcIn{from{opacity:0;transform:translateY(20px)}to{opacity:1;transform:translateY(0)}}',");

            // Container
            sb.AppendLine("    '#dpc-w{display:flex;gap:0;max-width:960px;width:100%;max-height:90vh;background:#1a1b1d;border:1px solid #2e2f32;border-radius:16px;overflow:hidden;box-shadow:0 8px 60px rgba(0,0,0,.7);animation:dpcIn .6s ease-out}',");

            // Left column
            sb.AppendLine("    '#dpc-l{flex:1;padding:32px 40px;display:flex;flex-direction:column;gap:20px;overflow-y:auto}',");

            // Logo
            sb.AppendLine("    '#dpc-logo{font-family:\"Manrope\",\"Segoe UI\",system-ui,sans-serif;font-weight:700;font-size:20px;letter-spacing:2px;color:#e8e8e8;text-transform:uppercase;display:flex;align-items:center;gap:8px}',");
            sb.AppendLine("    '#dpc-logo-next{font-weight:400;color:#666;letter-spacing:2px}',");

            // Title
            sb.AppendLine("    '#dpc-title{font-size:28px;font-weight:700;color:#f5f5f5;line-height:1.3;margin:0}',");

            // Warning block
            sb.AppendLine("    '#dpc-warn{background:rgba(255,255,255,.05);border:1px solid #383838;border-radius:10px;padding:12px 16px;display:flex;align-items:flex-start;gap:10px;font-size:13px;line-height:1.5;color:#c0c0c0}',");
            sb.AppendLine("    '#dpc-warn svg{flex-shrink:0;margin-top:1px;width:18px;height:18px;min-width:18px;min-height:18px}',");

            // Hint
            sb.AppendLine("    '#dpc-hint{font-size:13.5px;color:#808080;line-height:1.6;margin:0}',");

            // Form
            sb.AppendLine("    '#dpc-iw{display:flex;flex-direction:column;gap:12px}',");
            sb.AppendLine("    '#dpc-btn{width:100%;padding:14px;background:#2c2d30;color:#d8d8d8;border:1px solid #3a3b3e;border-radius:10px;font-family:inherit;font-size:15px;font-weight:600;cursor:pointer;letter-spacing:.3px;transition:background .2s,border-color .2s,transform .1s}',");
            sb.AppendLine("    '#dpc-btn:disabled{opacity:.3;cursor:default;transform:none}',");
            sb.AppendLine("    '@media(hover:hover) and (pointer:fine){#dpc-btn:not(:disabled):hover{background:#353639;border-color:#444}#dpc-btn:not(:disabled):active{transform:scale(.98)}}',");
            sb.AppendLine("    '@media(hover:none){#dpc-btn:not(:disabled):active{transform:scale(.98)}}',");
            sb.AppendLine("    '#dpc-err{font-size:.82rem;min-height:1.15em;line-height:1.5;padding-left:.125rem;transition:color .2s}',");

            // Right column
            sb.AppendLine("    '#dpc-r{width:280px;flex-shrink:0;background:#141516;border-left:1px solid #2a2b2e;padding:32px 24px;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:16px;text-align:center}',");
            sb.AppendLine("    '#dpc-qr-box{width:160px;height:160px;background:#fff;border-radius:12px;padding:10px;display:flex;align-items:center;justify-content:center;box-shadow:0 2px 20px rgba(0,0,0,.5);flex-shrink:0}',");
            sb.AppendLine("    '#dpc-qr-box img{display:block;width:100%;height:auto}',");
            sb.AppendLine("    '#dpc-qrcap{font-size:14px;font-weight:600;color:#d8d8d8}',");
            sb.AppendLine("    '#dpc-qrsub{font-size:12.5px;color:#686868;line-height:1.5;margin:0}',");
            sb.AppendLine("    '#dpc-tgname{font-family:\"JetBrains Mono\",\"Courier New\",monospace;font-size:13px;color:#909090;font-weight:600;letter-spacing:.3px}',");
            sb.AppendLine("    '#dpc-tgbtn{display:inline-flex;align-items:center;gap:8px;padding:10px 20px;background:#252628;color:#d0d0d0;border:1px solid #383a3d;border-radius:10px;font-family:inherit;font-size:13px;font-weight:600;cursor:pointer;text-decoration:none;transition:background .2s,border-color .2s,transform .1s;white-space:nowrap}',");
            sb.AppendLine("    '@media(hover:hover) and (pointer:fine){#dpc-tgbtn:hover{background:#2e3033;border-color:#444}#dpc-tgbtn:active{transform:scale(.97)}}',");
            sb.AppendLine("    '@media(hover:none){#dpc-tgbtn:active{transform:scale(.97)}}',");
            sb.AppendLine("    '#dpc-r-info{display:flex;flex-direction:column;align-items:center;gap:12px;text-align:center}',");

            // Responsive
            sb.AppendLine("    '@media(max-width:900px){#dpc{padding:16px}#dpc-w{max-width:720px}#dpc-l{padding:24px 28px}#dpc-r{width:240px;padding:24px 20px}}',");
            sb.AppendLine("    '@media(max-width:700px){#dpc{background:transparent;padding:0;align-items:flex-start}#dpc-w{flex-direction:column;border-radius:0;border:none;box-shadow:none;min-height:100dvh;justify-content:flex-start;max-height:none}#dpc-l{flex:0 0 auto;padding:28px 24px;overflow:visible}#dpc-r{flex:0 0 auto;width:100%;flex-direction:row;align-items:flex-start;justify-content:flex-start;border-left:none;border-top:1px solid #2a2b2e;padding:20px 24px;gap:20px;text-align:left}#dpc-qr-box{width:120px;height:120px;flex-shrink:0}#dpc-r-info{align-items:flex-start;text-align:left;flex:1;min-width:0}}',");
            sb.AppendLine("    '@media(max-width:420px){#dpc-l{padding:24px 20px}#dpc-r{padding:16px 20px}}',");
            // Landscape mobile — QR рядом с формой, overflow scroll
            sb.AppendLine("    '@media(max-height:500px) and (orientation:landscape){#dpc{align-items:flex-start;padding:0}#dpc-w{flex-direction:row;min-height:100dvh;border-radius:0;border:none;box-shadow:none}#dpc-r{width:220px;flex-shrink:0;border-left:1px solid #2a2b2e;border-top:none;overflow-y:auto;padding:20px 16px;justify-content:flex-start}#dpc-qr-box{width:120px;height:120px}#dpc-l{overflow-y:auto;padding:20px 24px}}',");

            // TV — ограничиваем максимальные размеры
            sb.AppendLine("    '@media(min-width:1400px){#dpc{padding:40px}#dpc-w{max-width:1100px;max-height:85vh;overflow:hidden}#dpc-l{padding:40px 48px;gap:24px;overflow-y:auto}#dpc-r{width:320px;padding:40px 32px;gap:20px;overflow-y:auto}#dpc-qr-box{width:180px;height:180px}}',");

            // TV focus ring — светлая обводка при навигации пультом (.dpc-nav — визуальная подсветка,
            // без реального DOM-фокуса; ввод пароля делает Lampa.Input.edit, свой WebKit-safe механизм у tvOS)
            sb.AppendLine("    '#dpc-btn:focus,#dpc-btn.dpc-nav{background:#444547!important;border-color:#bbb!important;color:#fff!important;box-shadow:0 0 0 3px rgba(255,255,255,.15)!important;outline:none}',");
            sb.AppendLine("    '#dpc-tgbtn:focus,#dpc-tgbtn.dpc-nav{background:#333537!important;border-color:#bbb!important;color:#fff!important;box-shadow:0 0 0 3px rgba(255,255,255,.15)!important;outline:none}'");

            sb.AppendLine("  ].join('');");
            sb.AppendLine("  document.head.appendChild(s);");
            sb.AppendLine("})();");
            sb.AppendLine();

            // ── addDevice ────────────────────────────────────────────────────
            sb.AppendLine("function addDevice(message) {");
            sb.AppendLine("  if (document.getElementById('dpc')) return;");
            sb.AppendLine();

            // SVG icons as JS variables
            sb.AppendLine("  var svgWarn = '<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"#909090\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" style=\"flex-shrink:0;width:18px;height:18px\"><path d=\"M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z\"/><line x1=\"12\" y1=\"9\" x2=\"12\" y2=\"13\"/><line x1=\"12\" y1=\"17\" x2=\"12.01\" y2=\"17\"/></svg>';");
            sb.AppendLine("  var svgTg   = '<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" style=\"width:18px;height:18px\"><circle cx=\"12\" cy=\"12\" r=\"12\" fill=\"#d0d0d0\"/><path d=\"M17.5 7.5l-2.2 10.4c-.16.72-.6.9-1.22.56l-3.36-2.48-1.62 1.56c-.18.18-.33.33-.67.33l.24-3.4 6.17-5.57c.27-.24-.06-.37-.41-.14L6.3 13.86l-3.28-1.03c-.71-.22-.73-.71.15-1.05l12.82-4.94c.59-.21 1.1.14.51 1.66z\" fill=\"#1e1f21\"/></svg>';");
            sb.AppendLine();

            // Left column HTML
            // Текстовые узлы оставлены пустыми и заполняются через textContent ниже —
            // insertAdjacentHTML не должен получать значения из init.conf напрямую (XSS).
            sb.AppendLine("  var leftHtml = ''");
            sb.AppendLine("    + '<div id=\"dpc-l\">'");
            sb.AppendLine("    + '<div id=\"dpc-logo\"><span id=\"dpc-logo-next\"></span></div>'");
            sb.AppendLine("    + '<h1 id=\"dpc-title\"></h1>'");
            sb.AppendLine("    + '<div id=\"dpc-warn\">' + svgWarn + '<span id=\"dpc-warn-text\"></span></div>'");
            sb.AppendLine("    + '<p id=\"dpc-hint\"></p>'");
            sb.AppendLine("    + '<div id=\"dpc-iw\">'");
            sb.AppendLine("    + '<button id=\"dpc-btn\" type=\"button\">Войти</button>'");
            sb.AppendLine("    + '<div id=\"dpc-err\"></div>'");
            sb.AppendLine("    + '</div>'");
            sb.AppendLine("    + '</div>';");
            sb.AppendLine();

            // Right column HTML (only if tg + show_qr)
            if (hasTg && conf.show_qr)
            {
                sb.AppendLine("  var tgUrl  = " + jsTgUrl + ";");
                sb.AppendLine("  var tgName = " + jsTgName + ";");
                sb.AppendLine("  var rightHtml = ''");
                sb.AppendLine("    + '<div id=\"dpc-r\">'");
                sb.AppendLine("    + '<div id=\"dpc-qr-box\"><img src=\"https://api.qrserver.com/v1/create-qr-code/?size=" + qrSize + "x" + qrSize + "&data=' + encodeURIComponent(tgUrl) + '&margin=4\" loading=\"lazy\" /></div>'");
                sb.AppendLine("    + '<div id=\"dpc-r-info\">'");
                sb.AppendLine("    + '<div id=\"dpc-qrcap\"></div>'");
                sb.AppendLine("    + '<p id=\"dpc-qrsub\"></p>'");
                sb.AppendLine("    + '<div id=\"dpc-tgname\"></div>'");
                sb.AppendLine("    + '<a id=\"dpc-tgbtn\" target=\"_blank\" rel=\"noopener\">' + svgTg + ' <span id=\"dpc-tgbtn-text\"></span></a>'");
                sb.AppendLine("    + '</div>'");
                sb.AppendLine("    + '</div>';");
            }
            else
            {
                sb.AppendLine("  var rightHtml = '';");
            }

            sb.AppendLine();
            sb.AppendLine("  var html = '<div id=\"dpc\"><div id=\"dpc-w\">' + leftHtml + rightHtml + '</div></div>';");
            sb.AppendLine("  document.body.insertAdjacentHTML('beforeend', html);");
            sb.AppendLine();

            // Текст из init.conf проставляется через textContent/href, а не в разметку —
            // так браузер сам экранирует HTML-спецсимволы вместо ручного экранирования.
            sb.AppendLine("  document.getElementById('dpc-logo-next').textContent = " + jsBadge + ";");
            sb.AppendLine("  document.getElementById('dpc-title').textContent = " + jsTitle + ";");
            sb.AppendLine("  document.getElementById('dpc-warn-text').textContent = " + jsWarn + ";");
            sb.AppendLine("  document.getElementById('dpc-hint').textContent = " + jsHint + ";");
            sb.AppendLine();
            if (hasTg && conf.show_qr)
            {
                sb.AppendLine("  document.getElementById('dpc-qrcap').textContent = " + jsQrCap + ";");
                sb.AppendLine("  document.getElementById('dpc-qrsub').textContent = " + jsQrSub + ";");
                sb.AppendLine("  document.getElementById('dpc-tgname').textContent = tgName;");
                sb.AppendLine("  document.getElementById('dpc-tgbtn-text').textContent = " + jsTgBtn + ";");
                sb.AppendLine("  document.getElementById('dpc-tgbtn').href = tgUrl;");
                sb.AppendLine();
            }

            sb.AppendLine("  var _btn  = document.getElementById('dpc-btn');");
            sb.AppendLine("  var _err  = document.getElementById('dpc-err');");
            sb.AppendLine("  var _tgbtn = document.getElementById('dpc-tgbtn');");
            sb.AppendLine();

            // ── doLogin ──────────────────────────────────────────────────────
            sb.AppendLine("  function doLogin(val) {");
            sb.AppendLine("    if (!val) return;");
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
            sb.AppendLine("      }");
            sb.AppendLine("    }, function() {");
            sb.AppendLine("      _err.style.color = '#d95f5f';");
            sb.AppendLine("      _err.textContent = 'Ошибка соединения';");
            sb.AppendLine("      _btn.disabled = false;");
            sb.AppendLine("      _btn.textContent = 'Войти';");
            sb.AppendLine("    }, { code: val });");
            sb.AppendLine("  }");
            sb.AppendLine();

            // ── openInput ────────────────────────────────────────────────────
            // Тот же Lampa.Input.edit, что использует стоковый deny.js — это встроенная
            // в Lampa текстовая клавиатура (не нативный HTML input), она уже умеет
            // работать на Apple TV/tvOS и других TV-платформах без наших ручных хаков.
            sb.AppendLine("  function openInput() {");
            sb.AppendLine("    Lampa.Input.edit({");
            sb.AppendLine("      free: true,");
            sb.AppendLine("      title: 'Введите пароль',");
            sb.AppendLine("      nosave: true,");
            sb.AppendLine("      value: '',");
            sb.AppendLine("      nomic: true");
            sb.AppendLine("    }, function(new_value) { doLogin(new_value); });");
            sb.AppendLine("  }");
            sb.AppendLine();

            sb.AppendLine("  _btn.addEventListener('click', function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    e.stopPropagation();");
            sb.AppendLine("    if (_btn.disabled) return;");
            sb.AppendLine("    openInput();");
            sb.AppendLine("  });");
            sb.AppendLine();

            if (hasTg && conf.show_qr)
            {
                sb.AppendLine("  if (_tgbtn) { _tgbtn.addEventListener('click', function(e) { e.stopPropagation(); }); }");
                sb.AppendLine();
            }

            // ── TV-навигация между кнопками (Войти / Написать боту) ────────────
            sb.AppendLine("  var _focusables = [_btn, _tgbtn].filter(function(el){ return !!el; });");
            sb.AppendLine("  var _navIdx = 0;");
            sb.AppendLine();
            sb.AppendLine("  function markNav(idx) {");
            sb.AppendLine("    _navIdx = (idx + _focusables.length) % _focusables.length;");
            sb.AppendLine("    for (var i = 0; i < _focusables.length; i++) _focusables[i].classList.remove('dpc-nav');");
            sb.AppendLine("    _focusables[_navIdx].classList.add('dpc-nav');");
            sb.AppendLine("    _focusables[_navIdx].focus();");
            sb.AppendLine("  }");
            sb.AppendLine();
            sb.AppendLine("  markNav(0);");
            sb.AppendLine();

            sb.AppendLine("  document.addEventListener('keydown', function(e) {");
            sb.AppendLine("    if (!document.getElementById('dpc')) return;");
            sb.AppendLine("    var key = e.key;");
            sb.AppendLine("    if (key === 'ArrowDown' || key === 'ArrowRight') {");
            sb.AppendLine("      e.preventDefault(); e.stopPropagation();");
            sb.AppendLine("      markNav(_navIdx + 1);");
            sb.AppendLine("      return;");
            sb.AppendLine("    }");
            sb.AppendLine("    if (key === 'ArrowUp' || key === 'ArrowLeft') {");
            sb.AppendLine("      e.preventDefault(); e.stopPropagation();");
            sb.AppendLine("      markNav(_navIdx - 1);");
            sb.AppendLine("      return;");
            sb.AppendLine("    }");
            sb.AppendLine("    if (key === 'Enter' || key === ' ') {");
            sb.AppendLine("      var el = _focusables[_navIdx];");
            sb.AppendLine("      if (el === _btn) { e.preventDefault(); e.stopPropagation(); if (!_btn.disabled) openInput(); return; }");
            sb.AppendLine("      if (el === _tgbtn) {");
            sb.AppendLine("        e.preventDefault(); e.stopPropagation();");
            sb.AppendLine("        try { _tgbtn.click(); } catch(_) { try { window.location.href = _tgbtn.href; } catch(__) {} }");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  }, true);");
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
