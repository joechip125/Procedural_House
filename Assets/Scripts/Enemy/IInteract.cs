using System;

namespace Enemy
{
    public interface IInteract
    {
        public void GetInstruction(Action<Instruction> instruction);
    }
}