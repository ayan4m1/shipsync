using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public interface IPathService
    {
        string RemotePath { get; }
        
        string CreateRemotePath(GameSave save);
        string CreateRemotePath(Ship ship);

        string CleanName(string name);

        SaveSource FindSaveSource();
    }
}
