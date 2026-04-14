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
            string jsHint   = Js(string.IsNullOrWhiteSpace(conf.step1_text)     ? "Введите пароль в поле ниже и нажмите «Войти»." : conf.step1_text);
            string jsQrCap  = Js(string.IsNullOrWhiteSpace(conf.qr_caption)     ? "Нет пароля?" : conf.qr_caption);
            string jsQrSub  = Js(string.IsNullOrWhiteSpace(conf.qr_subcaption)  ? "Отсканируй QR или нажми кнопку, чтобы написать боту." : conf.qr_subcaption);
            string jsTgBtn  = Js(string.IsNullOrWhiteSpace(conf.tg_button_text) ? "Открыть Telegram" : conf.tg_button_text);
            string jsTgName = Js(string.IsNullOrWhiteSpace(conf.tg_target)      ? "" : conf.tg_target);

            var sb = new StringBuilder();
            sb.AppendLine("// DenyPageCustom v3.0 - auto-generated from init.conf[DenyPage]");
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

            // Divider
            sb.AppendLine("    '#dpc-sep{height:1px;background:#2a2a2a;width:100%}',");

            // Form
            sb.AppendLine("    '#dpc-iw{display:flex;flex-direction:column;gap:12px}',");
            sb.AppendLine("    '#dpc-inp-wrap{position:relative}',");
            sb.AppendLine("    '#dpc-inp-icon{position:absolute;left:14px;top:50%;transform:translateY(-50%);pointer-events:none;display:flex}',");
            sb.AppendLine("    '#dpc-inp{width:100%;box-sizing:border-box;padding:14px 44px 14px 42px;background:#141516;border:1px solid #2e2f32;border-radius:10px;color:#ececec;font-family:inherit;font-size:14px;outline:none;-webkit-appearance:none;appearance:none;transition:border-color .2s,box-shadow .2s}',");
            sb.AppendLine("    '#dpc-inp::placeholder{color:#555}',");
            sb.AppendLine("    '#dpc-inp:focus{border-color:#666;box-shadow:0 0 0 3px rgba(255,255,255,.05)}',");
            sb.AppendLine("    '#dpc-eye{position:absolute;right:12px;top:50%;transform:translateY(-50%);background:none;border:none;padding:4px;cursor:pointer;display:flex;align-items:center;color:#555;transition:color .2s}',");
            sb.AppendLine("    '#dpc-eye:hover{color:#999}',");
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
            sb.AppendLine("    '@media(min-width:1400px){#dpc{padding:40px}#dpc-w{max-width:1100px;max-height:85vh}#dpc-l{padding:40px 48px;gap:24px}#dpc-r{width:320px;padding:40px 32px;gap:20px}#dpc-qr-box{width:180px;height:180px}}'");

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
            sb.AppendLine("  var svgLock = '<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"#555\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" style=\"width:18px;height:18px\"><rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\" ry=\"2\"/><path d=\"M7 11V7a5 5 0 0110 0v4\"/></svg>';");
            sb.AppendLine("  var svgEyeOff = '<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94\"/><path d=\"M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19\"/><line x1=\"1\" y1=\"1\" x2=\"23\" y2=\"23\"/></svg>';");
            sb.AppendLine("  var svgEyeOn  = '<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/></svg>';");
            sb.AppendLine("  var svgTg   = '<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" style=\"width:18px;height:18px\"><circle cx=\"12\" cy=\"12\" r=\"12\" fill=\"#d0d0d0\"/><path d=\"M17.5 7.5l-2.2 10.4c-.16.72-.6.9-1.22.56l-3.36-2.48-1.62 1.56c-.18.18-.33.33-.67.33l.24-3.4 6.17-5.57c.27-.24-.06-.37-.41-.14L6.3 13.86l-3.28-1.03c-.71-.22-.73-.71.15-1.05l12.82-4.94c.59-.21 1.1.14.51 1.66z\" fill=\"#1e1f21\"/></svg>';");
            sb.AppendLine();

            // Left column HTML
            sb.AppendLine("  var leftHtml = ''");
            sb.AppendLine("    + '<div id=\"dpc-l\">'");
            sb.AppendLine("    + '<div id=\"dpc-logo\"><span id=\"dpc-logo-next\">NEXTGEN</span></div>'");
            sb.AppendLine("    + '<h1 id=\"dpc-title\">' + " + jsTitle + " + '</h1>'");
            sb.AppendLine("    + '<div id=\"dpc-warn\">' + svgWarn + '<span>' + " + jsWarn + " + '</span></div>'");
            sb.AppendLine("    + '<p id=\"dpc-hint\">' + " + jsHint + " + '</p>'");
            sb.AppendLine("    + '<div id=\"dpc-iw\">'");
            sb.AppendLine("    + '<div id=\"dpc-inp-wrap\"><span id=\"dpc-inp-icon\">' + svgLock + '</span>'");
            sb.AppendLine("    + '<input id=\"dpc-inp\" type=\"password\" inputmode=\"text\" placeholder=\"Введите пароль\" autocomplete=\"new-password\" autocapitalize=\"off\" autocorrect=\"off\" spellcheck=\"false\" readonly />'");
            sb.AppendLine("    + '<button id=\"dpc-eye\" type=\"button\" tabindex=\"-1\">' + svgEyeOff + '</button></div>'");
            sb.AppendLine("    + '<button id=\"dpc-btn\">Войти</button>'");
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
                sb.AppendLine("    + '<div id=\"dpc-qrcap\">' + " + jsQrCap + " + '</div>'");
                sb.AppendLine("    + '<p id=\"dpc-qrsub\">' + " + jsQrSub + " + '</p>'");
                sb.AppendLine("    + '<div id=\"dpc-tgname\">' + tgName + '</div>'");
                sb.AppendLine("    + '<a id=\"dpc-tgbtn\" href=\"' + tgUrl + '\" target=\"_blank\" rel=\"noopener\">' + svgTg + ' ' + " + jsTgBtn + " + '</a>'");
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

            sb.AppendLine("  var _inp  = document.getElementById('dpc-inp');");
            sb.AppendLine("  var _btn  = document.getElementById('dpc-btn');");
            sb.AppendLine("  var _err  = document.getElementById('dpc-err');");
            sb.AppendLine("  var _wrap = document.getElementById('dpc');");
            sb.AppendLine("  var _eye  = document.getElementById('dpc-eye');");
            sb.AppendLine();
            sb.AppendLine("  setTimeout(function() { _inp.removeAttribute('readonly'); }, 100);");
            sb.AppendLine();
            sb.AppendLine("  var _eyeVisible = false;");
            sb.AppendLine("  _eye.addEventListener('click', function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    e.stopPropagation();");
            sb.AppendLine("    _eyeVisible = !_eyeVisible;");
            sb.AppendLine("    _inp.type = _eyeVisible ? 'text' : 'password';");
            sb.AppendLine("    _eye.innerHTML = _eyeVisible ? svgEyeOn : svgEyeOff;");
            sb.AppendLine("    _inp.focus();");
            sb.AppendLine("  });");
            sb.AppendLine();

            sb.AppendLine("  function keepFocus() {");
            sb.AppendLine("    if (!_inp) return;");
            sb.AppendLine("    if (document.activeElement !== _inp && document.activeElement !== _eye) _inp.focus();");
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
            sb.AppendLine("    if (document.activeElement !== _inp && document.activeElement !== _eye && e.key && e.key.length === 1) _inp.focus();");
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
