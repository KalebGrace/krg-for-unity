using UnityEngine;

namespace KRG
{
    public class ActivateSubject : MonoBehaviour
    {
        private IActivate m_Interface;

        public IActivate Interface => m_Interface ?? LazyInitializeInterface();

        private IActivate LazyInitializeInterface()
        {
            m_Interface = GetComponent<IActivate>();
            return m_Interface;
        }
    }
}
