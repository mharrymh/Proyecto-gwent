#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System;

public interface IExecuteScope {
    /// <summary>
    /// Check is a variable has been defined before
    /// </summary>
    /// <param name="varaiable"></param>
    /// <returns></returns>
    public bool IsDefined(string variable);
    /// <summary>
    /// Define the used variables with the names and its values
    /// </summary>
    /// <param name="nameId"></param>
    /// <param name="value"></param>
    void Define(string nameId, object value);
    /// <summary>
    /// Returns the value when variable name is passed
    /// </summary>
    /// <param name="nameId"></param>
    /// <returns></returns>
    object GetValue(string nameId);
    /// <summary>
    /// Create a subyacent scope
    /// </summary>
    /// <returns></returns>
    IExecuteScope CreateChildScope();
}

public class ExecuteScope : IExecuteScope
{
    //Create a reference to the scope parent
    IExecuteScope? parent;
    Dictionary<string, object> variables = new Dictionary<string, object>();
    //Create the scope child
    public IExecuteScope CreateChildScope() => new ExecuteScope {parent = this};
    public bool IsDefined(string variable)
    {
        return variables.ContainsKey(variable) || (parent != null && parent.IsDefined(variable));
    }
    public void Define(string nameId, object value)
    {
        //If the variable is not defined add it to the dictionary
        if (!IsDefined(nameId)) {
            variables.Add(nameId, value);
        }
        //else change its value
        else {
            //change its value, dynamic type
            variables[nameId] = value;
        }
    }

    public object GetValue(string nameId)
    {
        if (variables.ContainsKey(nameId))
            return variables[nameId];
        if (parent != null)
            return parent.GetValue(nameId);
        //else
        return null;
    }

}