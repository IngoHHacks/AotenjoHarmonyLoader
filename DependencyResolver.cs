namespace HarmonyLoaderAotenjo;

public static class DependencyResolver
{
    private class ModNode
    {
        public Mod Mod;
        public List<ModNode> Dependencies = new();
        public List<ModNode> Dependents = new();
    }
    
    public static Mod[] SortModsByDependencies(Mod[] mods)
    {
        var nodes = mods.ToDictionary(m => m, m => new ModNode { Mod = m });
        foreach (var node in nodes.Values)
        {
            if (File.Exists(Path.Combine(node.Mod.modDir, "dependencies.txt")))
            {
                var deps = File.ReadAllLines(Path.Combine(node.Mod.modDir, "dependencies.txt"));
                foreach (var depName in deps)
                {
                    var depNode = nodes.Values.FirstOrDefault(n => n.Mod.modID == depName);
                    if (depNode != null)
                    {
                        if (depNode == node)
                        {
                            Logger.LogError($"Mod '{node.Mod.modID}' has a circular dependency on itself.");
                            continue;
                        }
                        if (depNode.Dependencies.Contains(node))
                        {
                            Logger.LogError($"Mods '{node.Mod.modID}' and '{depNode.Mod.modID}' have circular dependencies.");
                            continue;
                        }
                        node.Dependencies.Add(depNode);
                        depNode.Dependents.Add(node);
                    }
                }
            }
        }
        var sorted = new List<Mod>();
        var noDependencyNodes = new Queue<ModNode>(nodes.Values.Where(n => n.Dependencies.Count == 0));
        while (noDependencyNodes.Count > 0)
        {
            var node = noDependencyNodes.Dequeue();
            sorted.Add(node.Mod);
            foreach (var dependent in node.Dependents)
            {
                dependent.Dependencies.Remove(node);
                if (dependent.Dependencies.Count == 0)
                {
                    noDependencyNodes.Enqueue(dependent);
                }
            }
        }

        if (sorted.Count != mods.Length)
        {
            Logger.LogError("Circular dependency detected among mods. Unable to resolve load order.");
            return mods;
        }
        return sorted.ToArray();
    }
}