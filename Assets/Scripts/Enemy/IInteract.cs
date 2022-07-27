using System;
using UnityEngine;

namespace Enemy
{
    public interface IInteract
    {
        public void SetInstruction(AiAgent agent);

        public GameObject GetItem();

        public void GiveItem(GameObject theItem);
    }
}