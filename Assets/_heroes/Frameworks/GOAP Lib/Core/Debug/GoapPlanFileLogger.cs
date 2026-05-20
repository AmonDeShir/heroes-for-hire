using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    public static class GoapPlanFileLogger
    {
        private static readonly object _gate = new object();
        private static readonly List<string> _buffer = new List<string>(capacity: 512);
        private static int _plansStarted;
        private static string _path;
        private static float _nextFlushAt;
        private static bool _announcedPath;

        public static int BeginPlanIfNeeded(string goalName, int maxPlans)
        {
            if (!PlanningDebugSettings.Enabled || !PlanningDebugSettings.LogToFile)
            {
                return -1;
            }

            lock (_gate)
            {
                if (_plansStarted >= maxPlans)
                {
                    AnnounceOnce();
                    return -1;
                }

                _plansStarted++;

                EnsurePath();
                AppendLine_NoLock($"\n=== GOAP PLAN START #{_plansStarted} goal='{goalName ?? string.Empty}' t={Time.unscaledTime:0.###} ===");

                if (_plansStarted == maxPlans)
                {
                    
                    AnnounceOnce();
                }
                return _plansStarted;
            }
        }

        private static void AnnounceOnce()
        {
            if (_announcedPath)
            {
                return;
            }

            EnsurePath();
            _announcedPath = true;
            UnityEngine.Debug.Log($"[GOAP-PLAN] Planner logs are being written to '{_path}' (first {PlanningDebugSettings.MaxPlansToLog} plans)." );
        }

        public static void AppendLine(string line)
        {
            if (!PlanningDebugSettings.Enabled || !PlanningDebugSettings.LogToFile)
            {
                return;
            }

            lock (_gate)
            {
                
                if (_plansStarted <= 0 || _plansStarted > PlanningDebugSettings.MaxPlansToLog)
                {
                    return;
                }

                EnsurePath();
                AppendLine_NoLock(line);
            }
        }

        public static void FlushIfDue(float now, float flushIntervalSeconds, int maxBufferedLines)
        {
            if (!PlanningDebugSettings.Enabled || !PlanningDebugSettings.LogToFile)
            {
                return;
            }

            lock (_gate)
            {
                if (_buffer.Count == 0)
                {
                    _nextFlushAt = now + flushIntervalSeconds;
                    return;
                }

                if (now < _nextFlushAt && _buffer.Count < maxBufferedLines)
                {
                    return;
                }

                EnsurePath();
                Flush_NoLock();
                _nextFlushAt = now + flushIntervalSeconds;
            }
        }

        private static void EnsurePath()
        {
            if (!string.IsNullOrWhiteSpace(_path))
            {
                return;
            }

            
            _path = Path.Combine(Application.persistentDataPath, "goap_plans.log");
        }

        private static void AppendLine_NoLock(string line)
        {
            _buffer.Add(line);
        }

        private static void Flush_NoLock()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? Application.persistentDataPath);
                using var stream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using var writer = new StreamWriter(stream, Encoding.UTF8);

                for (var i = 0; i < _buffer.Count; i++)
                {
                    writer.WriteLine(_buffer[i]);
                }

                _buffer.Clear();
            }
            catch (Exception e)
            {
                
                UnityEngine.Debug.LogError($"[GOAP-PLAN] Failed to write log file: {e.Message}");
                _buffer.Clear();
            }
        }
    }
}


