using System.IO;

using UnityEngine;

namespace Game.Utils
{
    public sealed class LogRecorderUtil
    {
        private const string kNameLog = "/Log/Log_{0}.txt";
        private const string kEnter = "___________________________________________\n";

        private const string kPattern = "{0}_{1}";
        private bool _isActive;
        private readonly string _path;

        public LogRecorderUtil(bool isActive, int sessionCount)
        {
            _isActive = isActive;

            if (!_isActive)
                return;

            _path = Application.dataPath + string.Format(kNameLog, sessionCount);
            File.WriteAllText(_path, string.Empty);
        }

        public void AddLog(string text)
        {
            if (!_isActive)
                return;

            WriteLog(text + kEnter);
        }

        private void WriteLog(string text)
        {
            if (!File.Exists(_path))
            {
                File.WriteAllText(_path, string.Empty);
            }

            File.AppendAllText(_path, text);
        }
    }
}