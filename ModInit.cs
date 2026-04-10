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
        private static readonly string OverridePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", "override", "deny.js");

        private static string _lastHash = "";
        private static Timer? _timer;

        public void Loaded(InitspaceModel baseconf)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OverridePath)!);
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
            try
            {
                var conf = ModuleInvoke.Init("DenyPage", new DenyPageConf());
                string content = DenyPageGenerator.Build(conf);
                string hash = content.GetHashCode().ToString();
                if (hash == _lastHash) return;
                File.WriteAllText(OverridePath, content, System.Text.Encoding.UTF8);
                _lastHash = hash;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DenyPageCustom: {ex.Message}");
            }
        }
    }
}
