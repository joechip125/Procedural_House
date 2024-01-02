using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class PersistentObject : MonoBehaviour
    {
        public int assetID;

        public Action<int> TargetHit;
    }
}