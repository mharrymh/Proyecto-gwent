using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public abstract class ExecutionError : Error
{
    public override abstract string Message {get;}
}

public class OverflowError : ExecutionError
{
    int Line {get;}
    public override string Message {
        get {
            return $"The execution of the effect was stopped because a stack overflow error detected in line: {Line}";
        }
    }
    public OverflowError(int line)
    {
        Line = line;
    }
}
public class OverCardsApplied : ExecutionError
{
    public override string Message {
        get {
            return $"The execution was stopped because you cant instantiate more than 3 cards in one turn.";
        }
    }
    public OverCardsApplied()
    {
    }
}

public class DecoyNeedToBeDropped : ExecutionError
{
    string DecoyName {get;}
    public override string Message {
        get {
            return $"A decoy card with the name: \"{DecoyName}\" was tried to be instantiated and decoy cards must be dropped to be played.";
        }
    }

    public DecoyNeedToBeDropped(string name)
    {
        DecoyName = name;
    }
}
public class Ex_DivisionByZero : ExecutionError
{
    int Line {get;}
    public override string Message {
        get {
            return $"Execution Error: The division expression in line {Line} was stopped because a division by zero was intended.";
        }
    }

    public Ex_DivisionByZero(int line)
    {
        Line = line;
    }
}
public class ExceededInteger : ExecutionError
{
    int Line {get;}
    public override string Message {
        get {
            return $"Execution Error: The pow expression in line {Line} was stopped because an integer was too large. \n Please dont exceed the following amount: {int.MaxValue} next time";
        }
    }

    public ExceededInteger(int line)
    {
        Line = line;
    }
}

public class Ex_IndexOutOfRange : ExecutionError
{
    int Line {get;}
    public override string Message {
        get {
            return $"Index out of range exception detected in line: {Line}";
        }
    }

    public Ex_IndexOutOfRange(int line)
    {
        Line = line;
    }
}

