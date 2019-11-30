using System.Collections.Generic;

namespace KRG
{
    [System.Serializable]
    public struct Condition
    {
        public ConditionType Type;

        [Enum(typeof(StatID))]
        public int StatID;

        [Enum(typeof(ItemID))]
        public int ItemID;

        public ConditionComparison Comparison;

        public float Value;
    }

    public enum ConditionComparison
    {
        None = 0,
        EqualTo = 1,
        LessThan = 2,
        LessThanOrEqualTo = 3,
        GreaterThan = 4,
        GreaterThanOrEqualTo = 5,
        NotEqualTo = 6,
        Consumes = 7,
    }

    public enum ConditionType
    {
        None = 0,
        Stat = 1,
        Item = 2,
    }

    public static class ConditionEM
    {
        public static bool AreConditionsMet(this List<Condition> conditions, GameObjectBody body)
        {
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Count; ++i)
                {
                    Condition c = conditions[i];
                    float bodyValue = GetBodyValue(body, c);
                    if (!IsComparisonMet(bodyValue, c.Comparison, c.Value))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void Consume(this List<Condition> conditions, GameObjectBody body)
        {
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Count; ++i)
                {
                    Condition c = conditions[i];
                    if (c.Comparison == ConditionComparison.Consumes)
                    {
                        float bodyValue = GetBodyValue(body, c);
                        bodyValue -= c.Value;
                        SetBodyValue(body, c, bodyValue);
                    }
                }
            }
        }

        private static float GetBodyValue(GameObjectBody body, Condition c)
        {
            switch (c.Type)
            {
                case ConditionType.Stat:
                    return body.StateData.GetStatVal(c.StatID);
                case ConditionType.Item:
                    return body.StateData.GetItemQty(c.ItemID);
                default:
                    G.U.Unsupported(body, c.Type);
                    return 0;
            }
        }

        private static void SetBodyValue(GameObjectBody body, Condition c, float bodyValue)
        {
            switch (c.Type)
            {
                case ConditionType.Stat:
                    body.StateData.SetStatVal(c.StatID, bodyValue);
                    break;
                case ConditionType.Item:
                    body.StateData.SetItemQty(c.ItemID, bodyValue);
                    break;
                default:
                    G.U.Unsupported(body, c.Type);
                    return;
            }
        }

        private static bool IsComparisonMet(float bodyValue, ConditionComparison comparison, float coValue)
        {
            switch (comparison)
            {
                case ConditionComparison.None:
                    return true;
                case ConditionComparison.EqualTo:
                    return bodyValue.Ap(coValue);
                case ConditionComparison.LessThan:
                    return bodyValue < coValue;
                case ConditionComparison.LessThanOrEqualTo:
                    return bodyValue <= coValue;
                case ConditionComparison.GreaterThan:
                    return bodyValue > coValue;
                case ConditionComparison.GreaterThanOrEqualTo:
                case ConditionComparison.Consumes:
                    return bodyValue >= coValue;
                case ConditionComparison.NotEqualTo:
                    return !bodyValue.Ap(coValue);
                default:
                    G.U.Unsupported(null, comparison);
                    return false;
            }
        }
    }
}
