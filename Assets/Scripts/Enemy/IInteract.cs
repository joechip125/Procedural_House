using System;

namespace Enemy
{
    public interface IInteract
    {
        public void GetInstruction(Action<Instruction> instruction);

        public void SetInstruction(AiAgent agent);
    }
}