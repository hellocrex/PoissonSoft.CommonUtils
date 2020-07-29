using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

namespace PoissonSoft.CommonUtils.Configuring
{
    /// <summary>
    /// Storage on local machine.
    /// <para>
    /// A strategy for selecting the location of data. First we look for the key in the file (%AppData%\PoissonSoft\Environment\vars.xml
    /// in Windows). If it was not found, we look it in the user environment variables. If it is not there we throw an exception.
    /// It is readonly container. To add data you need to manually edit the file and/or environment variables
    /// </para>
    /// <para>
    /// DO NOT USE IT AS A CACHE, because each request will cause the file system to be accessed
    /// </para>
    /// </summary>
    public class LocalKvStorage : IKvStorage
    {

        private readonly string dataFile;

        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="fileName">The file where the data is stored</param>
        public LocalKvStorage(string fileName = null)
        {
            dataFile = fileName ?? GetDefaultFileName();
        }

        /// <inheritdoc />
        public string this[string key]
        {
            get => GetValue(key);
            // ReSharper disable once ValueParameterNotUsed
            set => ThrowReadOnlyException("You can't add/modify the item");
        }

        /// <inheritdoc />
        public string[] AllKeys => GetFileData().Keys.ToArray();

        /// <inheritdoc />
        public void RemoveItem(string key)
        {
            ThrowReadOnlyException("You can't delete the item");
        }

        private void ThrowReadOnlyException(string comment = null)
        {
            var prefix = string.IsNullOrWhiteSpace(comment) ? string.Empty : $"{comment}. ";
            throw new ReadOnlyException($"{prefix}It is readonly container");
        }

        private static string GetDefaultFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PoissonSoft\\Environment\\vars.xml");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotImplementedException("Linux support is not implemented yet");
            }

            throw new Exception($"Unknown system {RuntimeInformation.OSDescription}");
        }

        private string GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var data = GetFileData();

            if (data?.Any() == true && data.ContainsKey(key)) return data[key];

            // Try read from Environment variable
            var res = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
            if (res != null) return res;

            throw new EntryPointNotFoundException($"Key '{key}' not found");
        }

        private Dictionary<string, string> GetFileData()
        {
            if (!File.Exists(dataFile)) return null;

            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.Load(dataFile);
            }
            catch (Exception ex)
            {
                Log($"Xml load exception:\n{ex.Message}");
                return null;
            }
            var res = new Dictionary<string, string>();

            var xmlItems = doc.DocumentElement?.GetElementsByTagName("Item");
                        if (xmlItems == null || xmlItems.Count == 0) return res;

            foreach (var dataItem in xmlItems)
            {
                var node = dataItem as XmlNode;
                if (node?.Attributes == null) continue;
                string key = null;
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].Name.ToUpperInvariant() == "KEY")
                    {
                        key = node.Attributes[i].Value;
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(key) || res.ContainsKey(key)) continue;
                res[key] = node.InnerText;
            }

            return res;
        }

        private void Log(string message)
        {
            try
            {
                File.AppendAllText($"{dataFile}.log", $"{DateTimeOffset.UtcNow}: {message}");
            }
            catch 
            {
                // Ignore problem
            }
        }
    }
}
