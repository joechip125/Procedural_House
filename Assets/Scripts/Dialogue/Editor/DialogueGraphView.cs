using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
    public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
    private NodeSearchWindow searchWindow;

    public Blackboard blackBoard;

    public List<ExposedProperty> ExposedProperties = new();
    
    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphStyle"));
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        
        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(EditorWindow window)
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(window, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }


    public void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }
        
        
        var property = new ExposedProperty();
        property.PropertyName = localPropertyName;
        property.PropertyValue = localPropertyValue;
        ExposedProperties.Add(property);

        var container = new VisualElement();
        var blackBoardField = new BlackboardField{ text = property.PropertyName, typeText = "string property"};
        container.Add(blackBoardField);

        var propertyValueTextField = new TextField("Value")
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingePropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            ExposedProperties[changingePropertyIndex].PropertyValue = evt.newValue;
        });
        var blackBoardValueRow = new BlackboardRow(blackBoardField, propertyValueTextField);
        container.Add(blackBoardValueRow);

        blackBoard.Add(container);
    }
    
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(funcCall: (port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);  
            }
        });
        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "NEXT";
        node.outputContainer.Add(generatedPort);
        
      //  node.capabilities&= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;
        
        RefreshDialogueNode(node);
        
        node.SetPosition(new Rect(100, 200, 100,150));

        return node;
    }

    public void ClearBlackBoardAndExposedProperties()
    {
        ExposedProperties.Clear();
        blackBoard.Clear();
    }
    
    public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
    {
        if(commentBlockData==null)
            commentBlockData = new CommentBlockData();
        var group = new Group
        {
            autoUpdateGeometry = true,
            title = commentBlockData.Title
        };
        AddElement(group);
        group.SetPosition(rect);
        return group;
    }

    
    public void CreateNode(string nodeName, Vector2 mousePosition)
    {
        AddElement(CreateDialogueNode(nodeName, mousePosition));
    }

    public DialogueNode CreateDialogueNode(string nodeName, Vector2 mousePosition)
    {
        var dialogueNode = new DialogueNode();
        dialogueNode.title = nodeName;
        dialogueNode.DialogueText = nodeName;
        dialogueNode.GUID = Guid.NewGuid().ToString();
        
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        var button = new Button(clickEvent: () =>
        {
            AddChoicePort(dialogueNode);
        });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        AddTextFieldToDialogueNode(dialogueNode);
        
        RefreshDialogueNode(dialogueNode);
        
        dialogueNode.SetPosition(new Rect (mousePosition, DefaultNodeSize));

        return dialogueNode;
    }

    private void RefreshDialogueNode(DialogueNode dialogueNode)
    {
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }
    
    private void AddTextFieldToDialogueNode(DialogueNode dialogueNode)
    {
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overridePortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overridePortName) 
            ? $"Choice {outputPortCount + 1}" : overridePortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
       
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(clickEvent:() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        
        RefreshDialogueNode(dialogueNode);
    }

    public void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        dialogueNode.outputContainer.Remove(generatedPort);
        
        RefreshDialogueNode(dialogueNode);
    }
    
}
