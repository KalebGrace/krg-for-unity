using System.Collections.Generic;
using UnityEngine;

namespace OSH
{
    public class PsuedoRando
    {
        public const float BINARY_MIN = 0.5f; // always a positive number

        public struct Range
        {
            public int RollMin; // first roll is 1
            public int RollMax;
            public float ResultMin;
            public float ResultMax;
        }

        public bool ResetOnTrueResult;

        private List<Range> m_Ranges = new List<Range>();
        private int m_RollCount;

        public void AddRange(Range range)
        {
            m_Ranges.Add(range);
        }

        public void AddRange(int rollMin, int rollMax, float resultMin, float resultMax)
        {
            m_Ranges.Add(new Range {
                RollMin = rollMin,
                RollMax = rollMax,
                ResultMin = resultMin,
                ResultMax = resultMax,
            });
        }

        public void AddRange(int rollMin, int rollMax, float probability)
        {
            Range r = new Range
            {
                RollMin = rollMin,
                RollMax = rollMax,
            };
            if (probability > 0)
            {
                float p = probability * BINARY_MIN;
                r.ResultMin = 0 + p;
                r.ResultMax = BINARY_MIN + p;
            }
            m_Ranges.Add(r);
        }

        public void ClearRanges()
        {
            m_Ranges.Clear();
        }

        public void ResetRolls()
        {
            m_RollCount = 0;
        }

        public float Roll()
        {
            ++m_RollCount;

            float result = 0;

            for (int i = 0; i < m_Ranges.Count; ++i)
            {
                Range r = m_Ranges[i];

                if (m_RollCount >= r.RollMin && m_RollCount <= r.RollMax)
                {
                    result = Random.Range(r.ResultMin, r.ResultMax);
                }
            }

            if (ResetOnTrueResult && result >= BINARY_MIN)
            {
                ResetRolls();
            }

            return result;
        }

        public bool RollBinary()
        {
            return Roll() >= BINARY_MIN;
        }
    }
}
