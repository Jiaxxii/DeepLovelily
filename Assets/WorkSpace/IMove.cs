using UnityEngine;

namespace WorkSpace
{
    public interface IMove
    {
        Vector2 Origin { get; }
        Vector2 Size { get; }
        Vector2 Position { get; set; }
        Vector3 LocalScale { get; set; }
        
        Vector3 EulerAngles { get; set; }
        
        float Depth { get; set; }
        
    }
}