using System.Collections.Generic;
using TMPro;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    public class DamageValue : MonoBehaviour
    {
#pragma warning disable 0414
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        private int _timeThreadIndex = (int) TimeThreadInstance.UseDefault;

        [SerializeField]
        private TMP_Text _text100 = default;

        [SerializeField]
        private TMP_Text _text010 = default;

        [SerializeField]
        private TMP_Text _text001 = default;
#pragma warning restore 0414

#if !NS_DG_TWEENING
        public void Init<T>(IDestroyedEvent<T> target, string damage)
        {
            G.U.Err("This class requires DG.Tweening (DOTween).");
        }
#else
        private List<Sequence> _tweens = new List<Sequence>();

        private TimeThread timeThread => G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);

        public void Init<T>(IDestroyedEvent<T> target, string damage)
        {
            target.Destroyed += _ => OnTargetDestroy();

            int l = damage.Length;
            switch (l)
            {
                case 1:
                    _text100.gameObject.SetActive(false);
                    _text010.gameObject.SetActive(false);

                    _text001.text = damage[l - 1].ToString();

                    _text001.transform.localPosition = Vector3.zero;

                    Animate(_text001, 1f);
                    break;
                case 2:
                    _text100.gameObject.SetActive(false);

                    _text010.text = damage[l - 2].ToString();
                    _text001.text = damage[l - 1].ToString();

                    _text010.transform.localPosition = Vector3.left * 0.25f;
                    _text001.transform.localPosition = Vector3.right * 0.25f;

                    _text010.GetComponent<Renderer>().material.renderQueue += 1;

                    Animate(_text010, 1f);
                    Animate(_text001, 0.75f);
                    break;
                case 3:
                    _text100.text = damage[l - 3].ToString();
                    _text010.text = damage[l - 2].ToString();
                    _text001.text = damage[l - 1].ToString();

                    _text100.transform.localPosition = Vector3.left * 0.5f;
                    _text010.transform.localPosition = Vector3.zero;
                    _text001.transform.localPosition = Vector3.right * 0.5f;

                    _text100.GetComponent<Renderer>().material.renderQueue += 2;
                    _text010.GetComponent<Renderer>().material.renderQueue += 1;

                    Animate(_text100, 0.5f);
                    Animate(_text010, 1f);
                    Animate(_text001, 0.75f);
                    break;
                default:
                    G.U.Err("Damage must be 1~3 characters in length. Damage value is: " + damage);
                    break;
            }
        }

        private void Animate(TMP_Text textMesh, float heightMult)
        {
            Sequence s = DOTween.Sequence();
            Transform textTf = textMesh.transform;

            //Bounce.
            float h = 5f * heightMult;
            float t = 16f / 30f; //Total bounce time will actually be 15f / 30f.
            for (int i = 0; i < 4; i++)
            {
                s.Append(textTf.DOLocalMoveY(h /= 2f, t /= 2f).SetEase(Ease.OutQuad));
                s.Append(textTf.DOLocalMoveY(0f, t).SetEase(Ease.InQuad));
            }
            //Graph of first bounce on https://www.desmos.com/calculator is:
            //$-2\left(\frac{5\left(x-0\right)^2}{\left(\frac{16}{30}\right)^2}-\frac{5\left(x-0\right)}{\frac{16}{30}}\right)$

            //Wait.
            s.AppendInterval(25f / 30f);

            //Collapse.
            s.Append(textTf.DOLocalMoveX(0f, 4f / 30f).SetEase(Ease.Linear));

            //Darken (at same time).
            s.Join(textMesh.DOColor(Color.black, 4f / 30f).SetEase(Ease.Linear));

            //Wait.
            s.AppendInterval(2f / 30f);

            //Dispose.
            s.AppendCallback(Dispose);

            //Register sequence.
            _tweens.Add(s);
            timeThread.AddTween(s);
        }

        private void Dispose()
        {
            if (this == null) return;

            gameObject.Dispose();
        }

        private void OnDestroy()
        {
            //some of the following may be null if the scene is changing or the app is quitting
            if (_text100 != null && _text100.gameObject.activeSelf)
            {
                DestroyImmediate(_text100.GetComponent<Renderer>().material);
            }
            if (_text010 != null && _text010.gameObject.activeSelf)
            {
                DestroyImmediate(_text010.GetComponent<Renderer>().material);
            }
            if (_text001 != null)
            {
                DestroyImmediate(_text001.GetComponent<Renderer>().material);
            }

            foreach (Sequence s in _tweens)
            {
                timeThread.RemoveTween(s);
            }
        }

        private void OnTargetDestroy()
        {
            if (this == null) return;

            transform.parent = null; //This needs to live on even if the target is destroyed.

            //this becomes disabled after reparenting, so we must re-enable it
            //TODO: find out why this happens (it happens in SoAm.TestDamageRing as well)
            //...and create a common solution for it
            var cfb = GetComponent<CameraFacingBillboard>();
            if (cfb != null)
            {
                cfb.enabled = true;
            }
        }
#endif
    }
}