using UnityEngine;

namespace OI.ScriptableTypes
{

    [UnityEngine.CreateAssetMenu(fileName = "New GameObject Channel", menuName = "ScriptableChannels/GameObjectChannel")]
    public class ScriptableGameObjectChannel : ScriptableChannel<GameObject> { }

}