using UnityEngine;

namespace Kayfo
{

    public class VirtualCurrency : Singleton<VirtualCurrency>
    {
        PersistentInt m_amount = new PersistentInt("virtual_currency", 0);
        int Get { get { return m_amount.Get();  } }

        void Add(int _amountToAdd)
        {
            Debug.Log("VirtualCurrency::Add Adding " + _amountToAdd + " to " + m_amount.Get());
            m_amount.Set(m_amount.Get() + _amountToAdd);
            Debug.Log("VirtualCurrency::Add New amount " + m_amount.Get());
        }

        bool Consume(int _amountToRemove)
        {
            Debug.Log("VirtualCurrency::Consume Adding " + _amountToRemove + " to " + m_amount.Get());
            if (m_amount.Get() - _amountToRemove < 0)
            {
                Debug.Log("VirtualCurrency::Consume Failed, not enough money");
                return false;
            }
            else
            {
                m_amount.Set(m_amount.Get() - _amountToRemove);
                Debug.Log("VirtualCurrency::Consume Success, remaining: " + m_amount.Get());
                return true;
            }
        }

        void Reset()
        {
            Debug.Log("VirtualCurrency::Reset");
            m_amount.Set(0);
        }
    }
}
