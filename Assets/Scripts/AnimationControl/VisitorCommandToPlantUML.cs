using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Msagl.Drawing;
using OALProgramControl;
using UnityEngine;

public class VisitorCommandToPlantUML : Visitor
{
    private readonly StringBuilder commandString;
    private bool simpleFormatting;

    private int indentationLevel;

    public Stack<string> classNames;

    private bool addingParameters = false;
    public List<long> superScopeLoopIDs;
    public Dictionary<long,long> commandsInLoops;
    public Dictionary<long,long> scopeMethodIdsForCalls;

    public VisitorCommandToPlantUML()
    {
        commandString = new StringBuilder();
        classNames = new Stack<string>();
        superScopeLoopIDs = new List<long>();
        commandsInLoops = new Dictionary<long,long>();
        scopeMethodIdsForCalls = new Dictionary<long, long>();
        ResetState();
    }

    public string GetCommandString() {
        string result = commandString.ToString();
        return result;
    }

    public void AppendToCommandString(string keyword) {
        commandString.Append(keyword);
    }

    private void ResetState() {
        simpleFormatting = true;
        indentationLevel = 0;
        commandString.Clear();
    }

    public void ActivateSimpleFormatting() {
        simpleFormatting = true;
        indentationLevel = 0;
    }
    
    public void AddPlantUmlHeader(string arguments = "") {
        AppendToCommandString("@startuml" + (arguments.Length > 0 ? " " + arguments : "") );
    }
    
    public void AddPlantUmlFutter() {
        AppendToCommandString("@enduml");
    }
    
    public void AddTransparentBackground()
    {
        AddEOL();
        AppendToCommandString("skinparam backgroundColor transparent");
    }
    
    public void SetArrowColor(string color)
    {
        AddEOL();
        AppendToCommandString("skinparam ArrowColor " + color);
    }
    
    public void DeactivateSimpleFormatting() {
        simpleFormatting = false;
    }

    private void IncreaseIndentation() {
        indentationLevel++;
    }

    private void DecreaseIndentation() {
        indentationLevel--;
    }

    private void WriteIndentation() {
        if (simpleFormatting) {
            commandString.Append(string.Concat(Enumerable.Repeat("\t", indentationLevel)));
        }
    }

    private void AddEOL() {
        if (simpleFormatting) {
            commandString.Append("\n");
        }
    }

    public void CloseAndRemoveLoopsFromIndex(int index){
        index = index + 1;
        int removeCount = superScopeLoopIDs.Count - index;
        CloseLoopWithIndentationByCount(removeCount);

        var removedLoopIDs = superScopeLoopIDs.GetRange(index, removeCount);
        superScopeLoopIDs.RemoveRange(index, removeCount);

        // Odstráň všetky commandID, ktoré majú loopID medzi zmazanými
        foreach (var key in commandsInLoops
                            .Where(kv => removedLoopIDs.Contains(kv.Value))
                            .Select(kv => kv.Key)
                            .ToList())
        {
            commandsInLoops.Remove(key);
        }
    }

    public void CloseLoopWithIndentationByCount(int Count){
        for (int i = 0; i < Count; i++)
        {
            AddEOL();
            DecreaseIndentation();
            WriteIndentation();
            commandString.Append("end\n");
        }
    }

    public void ModifySuperScopeIdsForCommandReturn(EXECommandReturn command){
        EXEScopeBase parentScope = command.SuperScope;
        bool foundLoop = false;

        while (parentScope != null)
        {
            long parentID = parentScope.CommandID;

            if (scopeMethodIdsForCalls.ContainsKey(parentID)) 
            {
                long loopID = commandsInLoops[scopeMethodIdsForCalls[parentID]];
                scopeMethodIdsForCalls.Remove(parentID);

                int index = superScopeLoopIDs.LastIndexOf(loopID);
                if (index != -1)
                {
                    commandsInLoops[parentID] = loopID;
                    CloseAndRemoveLoopsFromIndex(index);
                }

                foundLoop = true;
                break;
            }

            parentScope = parentScope.SuperScope;
        }

        if (!foundLoop) {
            CloseAndClearAllLoops();
        }
    }

    public void ModifySuperScopeIDsForCommandCall(EXECommandCall command) {
        EXEScopeBase parentScope = command.SuperScope;
        bool foundLoop = false;

        while (parentScope != null)
        {
            long parentID = parentScope.CommandID;

            int index = superScopeLoopIDs.LastIndexOf(parentID);
            if (index != -1)
            {   
                long loopID = superScopeLoopIDs[index];

                long commandID = command.InvokedMethod.CommandID;

                commandsInLoops[command.CommandID] = loopID;
                scopeMethodIdsForCalls[commandID] = command.CommandID;

                CloseAndRemoveLoopsFromIndex(index);
                
                foundLoop = true;
                break;
            }

            parentScope = parentScope.SuperScope;
        }

        if (!foundLoop) {
            CloseAndClearAllLoops();
        }

    }

    public void CloseAndClearAllLoops(){
        CloseLoopWithIndentationByCount(superScopeLoopIDs.Count);
        superScopeLoopIDs.Clear();
        commandsInLoops.Clear();
    }

    private void HandleBasicEXECommand(EXECommand command, Func<VisitorCommandToPlantUML, bool> addCommandSimpleString) {
        WriteIndentation();
        addCommandSimpleString(this);
        AddEOL();
    }

    public override void VisitExeCommand(EXECommand command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            return false;
        });
    }

    public override void VisitExeCommandBreak(EXECommandBreak command)
    {   
        HandleBasicEXECommand(command, (visitor) => {
            return false;
        });
    }

    public override void VisitExeCommandCall(EXECommandCall command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            if (commandsInLoops.ContainsKey(command.CommandID)) {
                commandsInLoops[command.InvokedMethod.CommandID] = commandsInLoops[command.CommandID];
                return false;
            } 

            ModifySuperScopeIDsForCommandCall(command);

            string callerClass = classNames.Peek();
            string nextClass = command.MethodAccessChainS;
            if (nextClass == "self"){
                nextClass = callerClass;
            }
            visitor.commandString.Append(callerClass + " -> " + nextClass  + " : ");
            classNames.Push(nextClass);

            command.MethodCall.Accept(visitor);
            AddEOL();
            WriteIndentation();
            visitor.commandString.Append("activate " + nextClass);

            return false;
        });
    }

    public override void VisitExeCommandContinue(EXECommandContinue command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            return false;
        });
    }

    public override void VisitExeCommandListOperation(EXECommandListOperation command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            return false;
        });
    }

    public override void VisitExeCommandAddingToList(EXECommandAddingToList command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.AddedElement.Accept(visitor);
            command.Array.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandAssignment(EXECommandAssignment command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.AssignmentTarget.Accept(visitor);
            command.AssignedExpression.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandCreateList(EXECommandCreateList command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.AssignmentTarget.Accept(visitor);
            foreach (var item in command.Items)
            {
                item.Accept(this);
            }
            return false;
        });
    }

    public override void VisitExeCommandFileAppend(EXECommandFileAppend command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.StringToWrite.Accept(visitor);
            command.FileToWriteTo.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandFileExists(EXECommandFileExists command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.AssignmentTarget.Accept(visitor);
            command.FileToCheck.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandFileRead(EXECommandFileRead command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.AssignmentTarget.Accept(visitor);
            command.FileToReadFrom.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandFileWrite(EXECommandFileWrite command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.StringToWrite.Accept(visitor);
            command.FileToWriteTo.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandQueryCreate(EXECommandQueryCreate command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            if (command.AssignmentTarget != null)
            {
                command.AssignmentTarget.Accept(visitor);
            }
            return false;
        });
    }

    public override void VisitExeCommandQueryDelete(EXECommandQueryDelete command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.DeletedVariable.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandRead(EXECommandRead command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.AssignmentTarget.Accept(visitor);

            if (command.Prompt != null)
            {
                command.Prompt.Accept(visitor);
            }
            return false;
        });
    }

    public override void VisitExeCommandRemovingFromList(EXECommandRemovingFromList command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.Item.Accept(visitor);
            command.Array.Accept(visitor);
            return false;
        });
    }

    public override void VisitExeCommandReturn(EXECommandReturn command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            if (commandsInLoops.ContainsKey(command.SuperScope.CommandID)) {
                return false;
            }
            ModifySuperScopeIdsForCommandReturn(command);
            if (command.Expression != null)
            {
                command.Expression.Accept(visitor);
            }
            if (classNames.Count != 1){ 
                AddEOL();
                WriteIndentation();
                visitor.commandString.Append("return done");
            }
            if (classNames.Count > 0){
                classNames.Pop();
            }
            
            
            return false;
        });
    }


    public override void VisitExeCommandWait(EXECommandWait command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            command.WaitTime.Accept(visitor);

            return false;
        });
    }

    public override void VisitExeCommandWrite(EXECommandWrite command)
    {
        HandleBasicEXECommand(command, (visitor) => {
            foreach (var arg in command.Arguments)
            {
                arg.Accept(this);
            }
            return false;
        });
    }

    public override void VisitExeScopeNull(EXEScopeNull scope)
    {
        HandleBasicEXECommand(scope, (visitor) => {
            return false;
        });
    }

    public override void VisitExeScope(EXEScope scope)
    {
    }

    public override void VisitExeScopeLoop(EXEScopeLoop scope)
    {
    }

    public override void VisitExeScopeMethod(EXEScopeMethod scope)
    {
    }

    public override void VisitExeScopeForEach(EXEScopeForEach scope)
    { 
        if (superScopeLoopIDs.Contains(scope.CommandID)){
            return;
        }

        // first time going through loop
        superScopeLoopIDs.Add(scope.CommandID);
        commandString.Append("loop " + scope.IteratorName);

        IncreaseIndentation();

        AddEOL();
    }

    public override void VisitExeScopeParallel(EXEScopeParallel scope)
    {
        WriteIndentation();

        if (scope.Threads != null)
        {
            foreach (EXEScope Thread in scope.Threads)
            {
                WriteIndentation();

                IncreaseIndentation();
                IncreaseIndentation();
                foreach (EXECommand Command in Thread.Commands)
                {
                    Command.Accept(this);
                }
                DecreaseIndentation();
                DecreaseIndentation();

                WriteIndentation();
                AddEOL();
            }
        }

        WriteIndentation();
        AddEOL();
    }

    public override void VisitExeScopeCondition(EXEScopeCondition scope)
    {
        WriteIndentation();
        scope.Condition.Accept(this);

        IncreaseIndentation();
        foreach (EXECommand Command in scope.Commands)
        {
            Command.Accept(this);
        }
        DecreaseIndentation();

        if (scope.ElifScopes != null)
        {
            foreach (EXEScopeCondition Elif in scope.ElifScopes)
            {
                WriteIndentation();
                Elif.Condition.Accept(this);

                IncreaseIndentation();
                foreach (EXECommand Command in Elif.Commands)
                {
                    Command.Accept(this);
                }
                DecreaseIndentation();
            }
        }
            if (scope.ElseScope != null)
            {
                WriteIndentation();

                IncreaseIndentation();
                foreach (EXECommand Command in scope.ElseScope.Commands)
                {
                    Command.Accept(this);
                }
                DecreaseIndentation();
            }

            WriteIndentation();
            AddEOL();

    }

    public override void VisitExeScopeLoopWhile(EXEScopeLoopWhile scope)
    {
        WriteIndentation();
        scope.Condition.Accept(this);

        IncreaseIndentation();
        foreach (EXECommand Command in scope.Commands)
        {
            Command.Accept(this);
        }
        //DecreaseIndentation();

        WriteIndentation();
        AddEOL();

    }

    public override void VisitExeASTNodeAccesChain(EXEASTNodeAccessChain node)
    {
        foreach (var element in node.GetElements()) {
            element.NodeValue.Accept(this);
        }
    }

    public override void VisitExeASTNodeComposite(EXEASTNodeComposite node)
    {
        if (node.Operands.Count == 1)
        {
            node.Operands.First().Accept(this);
        }
        else
        {
            foreach (var operand in node.Operands) {
                operand.Accept(this);
            }
        }
    }

    public override void VisitExeASTNodeLeaf(EXEASTNodeLeaf node)
    {
        if (addingParameters){
            commandString.Append(node.Value);
        }
    }

    public override void VisitExeASTNodeMethodCall(EXEASTNodeMethodCall node)
    {
        commandString.Append(node.MethodName + "(");
        bool first = true;
        addingParameters = true;
        foreach (var arg in node.Arguments) {
            if (first)
            {
                first = false;
            }
            else
            {
                commandString.Append(", ");
            }
            
            arg.Accept(this);
        }
        addingParameters = false;
        commandString.Append(")");
    }

    public override void VisitExeValueArray(EXEValueArray value)
    {
        if (value.Elements != null)
        {
            foreach (var element in value.Elements) {
                element.Accept(this);
            }
        }
    }

    public override void VisitExeValueBool(EXEValueBool value)
    {
    }

    public override void VisitExeValueInt(EXEValueInt value)
    {
    }

    public override void VisitExeValueReal(EXEValueReal value)
    {
    }

    public override void VisitExeValueReference(EXEValueReference value)
    {
    }

    public override void VisitExeValueString(EXEValueString value)
    {
    }

    public override void VisitExeASTNodeIndexation(EXEASTNodeIndexation node)
    {
        node.List.Accept(this);
        node.Index.Accept(this);
    }
}
