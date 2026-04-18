using System.Collections;
using TMPro;
using UnityEngine;

namespace EC.UI
{
    public class TextAnimation : MonoBehaviour
    {
        private int m_Index;
        private Coroutine m_Coroutine;
        [SerializeField] private TextMeshProUGUI m_Txt;

        private void OnEnable()
        {
            m_Index = -1;
            m_Coroutine = StartCoroutine(TextAnimationFunc());
        }

        private void OnDisable()
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine =null;
        }

        IEnumerator TextAnimationFunc()
        {
            while (true)
            {
                m_Index++;
                if (m_Index > 3)
                    m_Index = 0;
                switch (m_Index)
                {
                    case 0:
                        m_Txt.text = "SEARCHING\r\nFOR OPPONENTS";
                        break;
                    case 1:
                        m_Txt.text = "SEARCHING\r\nFOR OPPONENTS.";
                        break;
                    case 2:
                        m_Txt.text = "SEARCHING\r\nFOR OPPONENTS..";
                        break;
                    case 3:
                        m_Txt.text = "SEARCHING\r\nFOR OPPONENTS...";
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(.43f);
            }
        }
    }
}
