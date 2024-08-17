using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEditor.PackageManager.Requests;
#nullable enable


/// <summary>
/// This saves the evaluated effects ready to be used int the game
/// </summary>
public class DeclaredEffect
{
    public DeclaredEffect? Parent = null;
    public string Name  {get;}
    Dictionary<string, object?> Params {get;}
    public EffectSelector Targets {get; set;}
    public string TargetsNameId {get;}
    public string ContextNameId {get;}
    public DeclaredEffect? PostEffect {get; set;} = null;
    InstructionBlock Action {get;}

    public void Execute(IExecuteScope? scope = null, DeclaredEffect? parent = null)
    {
        //Create a new execute scope
        scope ??= new ExecuteScope();

        //First we define the context in the scope
        scope.Define(ContextNameId, new Context());
        //Then we define the targets (we pass the parent so if it is a posteffect we can access the parent targets if the source is parent)
        scope.Define(TargetsNameId, Targets.GetTargets(parent));

        //Define and save all the parameters of the effects in the scope
        if (Params != null)
        foreach (string nameId in Params.Keys)
        {
            scope.Define(nameId, Params[nameId]);
        }

        Action.Execute(scope);
        //Execute the post effect if it is not null
        PostEffect?.Execute(scope, this);
    }

    public DeclaredEffect(string name, List<string> paramsName, InstructionBlock action, string targetsNameId, string contextNameId)
    {
        Name = name;
        Action = action;
        //Fill the keys of the dictionary
        Params = new Dictionary<string, object?>();
        foreach(string paramName in paramsName)
        {
            Params.Add(paramName, null);
        }
        TargetsNameId = targetsNameId;
        ContextNameId = contextNameId;
    }
    /// <summary>
    /// Fill the values of the dictionary Params
    /// </summary>
    /// <param name="Declared"></param>
    public void FillParamsValues(Dictionary<string, object> paramsDeclared)
    {
        foreach (string key in paramsDeclared.Keys)
        {
            Params[key] = paramsDeclared[key];
        }
    }
}


public static class DeclaredEffects
{
    public static HashSet<DeclaredEffect> declaredEffects = new HashSet<DeclaredEffect>();
    public static void AddEffect(DeclaredEffect effect)
    {
        declaredEffects.Add(effect);
    }

    public static DeclaredEffect Find(string name)
    {
        //is secure that the effect exists and is unique because it was checked in the semantyc 
        return declaredEffects.FirstOrDefault(x => x.Name == name);
    }
}