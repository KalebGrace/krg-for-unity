using System;
using UnityEngine;

namespace _0G.Legacy
{
    [Serializable]
    public struct Effector
    {
        //4D Value Type
        [Enum(typeof(EffectorCondition))]
        public int condition;
        [Enum(typeof(EffectorSubject))]
        public int subject;
        [Enum(typeof(StatID))]
        public int property;
        [Enum(typeof(EffectorOperation))]
        public int operation;
        //
        public double value;

        //shortcut constructor
        public Effector(float? v = null, float d = 0, bool min = false, bool max = false)
        {
            condition = 0;
            subject = 0;
            property = 0;

            if (max)
            {
                operation = (int) EffectorOperation.SetToXPercentOfMax;
                value = 100;
            }
            else if (min)
            {
                operation = (int) EffectorOperation.SetToXPercentOfMax;
                value = 0;
            }
            else if (v.HasValue)
            {
                operation = (int) EffectorOperation.SetTo;
                value = v.Value + d;
            }
            else if (!d.Ap(0))
            {
                operation = (int) EffectorOperation.Add;
                value = d;
            }
            else
            {
                operation = (int) EffectorOperation.None;
                value = 0;
            }
        }

        public bool Operate(ref float pv, float min, float max)
        {
            if (min > max)
            {
                G.U.Err("Min is greater than max.");
                return false;
            }

            float ev = (float) value;

            switch ((EffectorOperation) operation)
            {
                case EffectorOperation.None:
                    // do nothing
                    return true;
                case EffectorOperation.SetTo:
                    // keep ev as is
                    break;
                case EffectorOperation.Add:
                    ev += pv;
                    break;
                case EffectorOperation.MultiplyBy:
                    ev *= pv;
                    break;
                case EffectorOperation.SetToXPercentOfMax:
                    ev *= 0.01f * (max - min);
                    ev += min;
                    break;
                case EffectorOperation.AddXPercentOfMax:
                    ev *= 0.01f * (max - min);
                    ev += pv;
                    break;
                default:
                    G.U.Unsupported(null, (EffectorOperation) operation);
                    return false;
            }

            pv = Mathf.Clamp(ev, min, max);

            return true;
        }
    }

    public enum EffectorCondition
    {
        // 0 ~ 99 reserved for _0G.Legacy
        None = 0,
        Use = 1,
        Equip = 2,
    }

    public enum EffectorSubject
    {
        None = 0,
        Self = 100,
        Target = 200,
        PlayerCharacter = 1000,
    }

    public enum EffectorOperation
    {
        None = 0,
        //
        SetTo = 100,
        //add, or subtract (if value is negative)
        Add = 200,
        //multiply, or divide (via decimal value)
        MultiplyBy = 300,
        //percentage of the maximum value (0~100)
        SetToXPercentOfMax = 400,
        //add specified percent
        AddXPercentOfMax = 500,
    }
}