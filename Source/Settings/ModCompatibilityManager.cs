using System;
using System.Collections.Generic;
using Verse;

namespace MadagascarVanilla.Settings
{
    public class ModCompatibilityManager 
    {
        // setting -> (packageId -> check)
        private Dictionary<string, Dictionary<string, Func<bool>>> _modCompatibilityChecks;
        
        public ModCompatibilityManager()
        {
            _modCompatibilityChecks = new Dictionary<string, Dictionary<string, Func<bool>>>();
        }

        public void Add(string packageId, string setting, Func<bool> check)
        {
            if (!_modCompatibilityChecks.ContainsKey(setting))
                _modCompatibilityChecks[setting] = new Dictionary<string, Func<bool>>();
            _modCompatibilityChecks[setting][packageId] = check;
        }

        // Return true if there is a compatibility issue
        public bool Check(string setting, out List<string> packageIds)
        {
            bool issue = false;
            packageIds = null;

            foreach (var (key, value) in _modCompatibilityChecks[setting])
            {
                if (value.Invoke())
                {
                    issue = true;
                    packageIds ??= new List<string>();
                    packageIds.Add(key);
                }
            }
            return issue;
        }
    }
}
