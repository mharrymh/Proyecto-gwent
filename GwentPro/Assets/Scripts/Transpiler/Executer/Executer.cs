using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#nullable enable

public static class Executer 
{
    #region CardCollection
    public static Dictionary<string, Func<Expression?, CardCollection, IExecuteScope, object>> CollectionFunctions = new()
    {
        {"Push", Push},
        {"SendBottom", SendBottom},
        {"Pop", Pop},
        {"Remove", Remove},
        {"RemoveAt", RemoveAt},
        {"Shuffle", Shuffle},
        {"Add", Add},
        {"Clear", Clear}
    };

    private static object RemoveAt(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.RemoveAt((int)expression.Execute(scope), true);
        return null;
    }

    private static object Clear(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.Clear(true);
        return null;
    }

    private static object Add(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.Add((Card)expression.Execute(scope), true);
        //It return null cause add is a void function
        return null;
    }

    private static object Shuffle(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.Shuffle();
        //Return null cause shuffle is a void function
        return null;
    }

    private static object Remove(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.Remove((Card)expression.Execute(scope), true);
        //It return null cause remove is a void function
        return null;
    }

    private static object Pop(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        return collection.Pop(true);
    }

    private static object SendBottom(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.SendBottom((Card)expression.Execute(scope), true);
        //It return null cause sendbottom is a void function
        return null;
    }

    private static object Push(Expression? expression, CardCollection collection, IExecuteScope scope)
    {
        collection.Push((Card)expression.Execute(scope), true);
        //It return null cause push is a void function
        return null;
    }
    #endregion
    #region Context
    public static Dictionary<string, Func<Context, Expression, IExecuteScope, object>> ContextFunctions = new()
    {
        {"HandOfPlayer", Hand},
        {"FieldOfPlayer", Field},
        {"GraveyardOfPlayer", Graveyard},
        {"DeckOfPlayer", Deck},
    };

    private static CardCollection Hand(Context context, Expression player, IExecuteScope scope)
    {
        return context.HandOfPlayer((Player)player.Execute(scope));
    }
    private static CardCollection Field(Context context, Expression player, IExecuteScope scope)
    {
        return context.FieldOfPlayer((Player)player.Execute(scope));
    }
    private static CardCollection Graveyard(Context context, Expression player, IExecuteScope scope)
    {
        return context.GraveyardOfPlayer((Player)player.Execute(scope));
    }
    private static CardCollection Deck(Context context, Expression player, IExecuteScope scope)
    {
        return context.DeckOfPlayer((Player)player.Execute(scope));
    }
    #endregion
}


