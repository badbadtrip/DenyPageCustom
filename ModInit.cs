using DenyPageCustom.Models;
using Shared.Models.Events;
using Shared.Models.Module;
using Shared.Models.Module.Interfaces;
using Shared.Services;
using System;
using System.IO;
using System.Threading;

namespace DenyPageCustom
{
    public class ModInit : IModuleLoaded
    {
        private static readonly string OverrideDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", "override");

        private static readonly string StockGatePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "module", "LampaWeb", "plugins", "telegram_auth_gate.js");

        private static readonly string GateOverridePath = Path.Combine(OverrideDir, "telegram_auth_gate.js");
        private static readonly string DenyOverridePath = Path.Combine(OverrideDir, "deny.js");

        private static string _lastHash = "";
        private static Timer? _timer;
        private static readonly object _syncLock = new();

        public void Loaded(InitspaceModel baseconf)
        {
            Directory.CreateDirectory(OverrideDir);

            // Гасим старую форму с паролем (deny.js), чтобы вместе с telegram_auth_gate.js
            // не показывались два независимых гейта одновременно.
            if (!File.Exists(DenyOverridePath) || File.ReadAllText(DenyOverridePath) != "")
                File.WriteAllText(DenyOverridePath, "", System.Text.Encoding.UTF8);

            SyncAndGenerate();
            EventListener.UpdateInitFile += SyncAndGenerate;
            _timer = new Timer(_ => SyncAndGenerate(), null,
                TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        public void Dispose()
        {
            EventListener.UpdateInitFile -= SyncAndGenerate;
            _timer?.Dispose();
        }

        private static void SyncAndGenerate()
        {
            lock (_syncLock)
            {
                try
                {
                    if (!File.Exists(StockGatePath))
                    {
                        Console.WriteLine($"DenyPageCustom: не найден {StockGatePath} — модуль LampaWeb не установлен или путь другой");
                        return;
                    }

                    var conf = ModuleInvoke.Init("DenyPage", new DenyPageConf());
                    string stockGateJs = File.ReadAllText(StockGatePath);
                    string content = DenyPageGenerator.Build(conf, stockGateJs);

                    string hash = content.GetHashCode().ToString();
                    if (hash == _lastHash) return;

                    File.WriteAllText(GateOverridePath, content, System.Text.Encoding.UTF8);
                    _lastHash = hash;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DenyPageCustom: {ex.Message}");
                }
            }
        }
    }
}
