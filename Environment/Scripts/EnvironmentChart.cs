using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    [CreateAssetMenu(
        fileName = "SomeWhere_EnvironmentChart.asset",
        menuName = "KRG Scriptable Object/Environment Chart",
        order = 503
    )]
    public class EnvironmentChart : Docket
    {
        // CONSTANTS

        public const string ENVIRONMENT_CHART_SUFFIX = "_EnvironmentChart";
        public const string DEFAULT_ANIMATION_SUFFIX = "_Default_RasterAnimation";

        // SERIALIZED FIELDS

        [Header("Game Object Data")]

        [Enum(typeof(EnvironmentID))]
        public int EnvironmentID;

        [Header("Environment Data")]

        public EnvironmentData Data;

        [Header("Animation Data")]

        public List<string> AnimationNames;

        // PROPERTIES

        public override int ID => EnvironmentID;
        public override string BundleName => GetBundleName(EnvironmentID);
        public override string DocketSuffix => ENVIRONMENT_CHART_SUFFIX;
        public override string DefaultAnimationSuffix => DEFAULT_ANIMATION_SUFFIX;

        // METHODS

        public static string GetBundleName(int environmentID)
        {
            return "_e" + environmentID.ToString("D5");
        }
    }
}
