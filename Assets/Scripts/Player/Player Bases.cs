using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The type of base
/// </summary>

public enum BaseType
{
    Beer = 0,
    Gin,
    Whiskey,
    Wine
}


/// <summary>
/// Container for all bases that can be equipped and shot, using its stats and modifiers of the mixer
/// </summary>
public class PlayerBases : MonoBehaviour
{
    [SerializeField] Base[] bases;
    Dictionary<BaseType, Base> basesDict = new Dictionary<BaseType, Base>();

    Base selectedBase;

    private void Start()
    {
        if (bases.Length != sizeof(BaseType))
        {
            throw new InvalidImplementationException("Base array must match the size of BaseType and in order of base type");
        }
        for (int i = 0; i < bases.Length; i++)
        {
            basesDict[(BaseType)i] = bases[i];
        }
    }
    /// <summary>
    /// Returns the base of the associated <see cref="BaseType"/>
    /// </summary>
    /// <param name="type"></param>
    public Base GetBase(BaseType type)
    {
        return basesDict[type];
    }
   
}
