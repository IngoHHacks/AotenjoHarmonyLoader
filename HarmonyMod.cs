namespace HarmonyLoaderAotenjo;

public abstract class HarmonyMod
{
    public abstract string ModName { get; }
    public abstract string ModAuthor { get;  }
    public abstract string ModVersion { get; }
    
    public virtual string ModGuid => $"{ModAuthor.Replace(" ", "")}.Aotenjo.{ModName.Replace(" ", "")}";
    
    public virtual void Init() { }
}