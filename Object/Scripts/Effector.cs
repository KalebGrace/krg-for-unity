﻿using System;
using UnityEngine;

namespace KRG
{
    [Serializable]
    public struct Effector
    {
        //4D Value Type
        [Enum(typeof(EffectorCondition))]
        public int condition;
        [Enum(typeof(EffectorSubject))]
        public int subject;
        [Enum(typeof(EffectorProperty))]
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
                operation = (int)EffectorOperation.PercentOfMax;
                value = 100;
            }
            else if (min)
            {
                operation = (int)EffectorOperation.PercentOfMax;
                value = 0;
            }
            else if (v.HasValue)
            {
                operation = (int)EffectorOperation.Set;
                value = v.Value + d;
            }
            else if (!d.Ap(0))
            {
                operation = (int)EffectorOperation.Add;
                value = d;
            }
            else
            {
                operation = (int)EffectorOperation.None;
                value = 0;
            }
        }

        public bool Operate(ref float pv, float min, float max)
        {
            if (min > max)
            {
                G.Err("Min is greater than max.");
                return false;
            }

            float ev = (float)value;

            switch ((EffectorOperation)operation)
            {
                case EffectorOperation.Set:
                    //
                    break;
                case EffectorOperation.Add:
                    ev += pv;
                    break;
                case EffectorOperation.Multiply:
                    ev *= pv;
                    break;
                case EffectorOperation.PercentOfMax:
                    ev *= 0.01f * (max - min);
                    ev += min;
                    break;
                default:
                    G.U.Unsupported(null, (EffectorOperation)operation);
                    return false;
            }

            pv = Mathf.Clamp(ev, min, max);

            return true;
        }
    }

    public enum EffectorCondition
    {
        None = 0,
    }

    public enum EffectorSubject
    {
        None = 0,
        Self = 100,
        Target = 200,
        PlayerCharacter = 1000,
    }

    public enum EffectorProperty
    {
        None = 0,
        HP = 1100,
        SP = 1200,
        HPMax = 2100,
        SPMax = 2200,
    }

    public enum EffectorOperation
    {
        None = 0,
        //
        Set = 100,
        //add, or subtract (if value is negative)
        Add = 200,
        //multiply, or divide (via decimal value)
        Multiply = 300,
        //percentage of the maximum value (0~100)
        PercentOfMax = 400,
    }
}