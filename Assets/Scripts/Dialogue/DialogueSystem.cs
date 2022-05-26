using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public Queue<string> Senteneces = new();


    public void StartDialogue(Dialogue dialogue)
    {
        Senteneces.Clear();

        foreach (var sentence in dialogue.sentences)
        {
            Senteneces.Enqueue(sentence);
        }
    }

    public void DisplayNextSentence()
    {
        if (Senteneces.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = Senteneces.Dequeue();
    }

    private void EndDialogue()
    {
        
    }
}
