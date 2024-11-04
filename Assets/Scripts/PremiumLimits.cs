using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PremiumLimits
{
    public static int WordsPerCustomSetLimit { get { return customSetWordMax; } }
    private static int customSetWordMax = 25;

    public static int MaxNumberOfCustomSets {  get { return maxNumberOfCustomSets; } }
    private static int maxNumberOfCustomSets = 10;

    public static int CurrentNumberOfCustomSets { get { return currentNumberOfCustomSets; } }
    private static int currentNumberOfCustomSets;
}
