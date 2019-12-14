using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
	public sealed class Destructible : MonoBehaviour, IBodyComponent
	{
		[SerializeField]
		private GameObjectBody m_Body = default;

		public GameObjectBody Body => m_Body;

		public void InitBody(GameObjectBody body)
		{
			m_Body = body;
		}

        private void Update()
        {
            if (m_Body.StateData.GetStatVal(StatID.HP) <= 0)
            {
                Destruct();
            }
        }

        private void Destruct()
        {
            // TODO: BLOW CHUNKS

            m_Body.Dispose();
        }
    }
}
