﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

#if NS_TMPRO
using TMPro;
#endif

namespace KRG {

    public class DamageValue : MonoBehaviour {

#pragma warning disable 0414
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;
#pragma warning restore 0414

#if NS_TMPRO
        [SerializeField]
        [FormerlySerializedAs("m_text100")]
		TMP_Text _text100;

        [SerializeField]
        [FormerlySerializedAs("m_text010")]
		TMP_Text _text010;

        [SerializeField]
        [FormerlySerializedAs("m_text001")]
		TMP_Text _text001;

#else
        [SerializeField]
        [FormerlySerializedAs("m_font")]
        Font _font;

        [SerializeField]
        [FormerlySerializedAs("m_material")]
        Material _material;

        [SerializeField]
        [FormerlySerializedAs("m_text100")]
        TextMesh _text100;

        [SerializeField]
        [FormerlySerializedAs("m_text010")]
        TextMesh _text010;

        [SerializeField]
        [FormerlySerializedAs("m_text001")]
        TextMesh _text001;
#endif

#if NS_DG_TWEENING
        IEnd _target;
        List<Sequence> _tweens = new List<Sequence>();

        ITimeThread timeThread {
            get {
                return G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
            }
        }

        void Awake() {
#if !NS_TMPRO
            _text001.font = _font;
            _text010.font = _font;
            _text100.font = _font;

            _text001.GetComponent<Renderer>().sharedMaterial = _material;
            _text010.GetComponent<Renderer>().sharedMaterial = _material;
            _text100.GetComponent<Renderer>().sharedMaterial = _material;
#endif
        }

        public void Init(IEnd target, int damage) {
            _target = target;
            _target.end.actions += OnTargetDestroy;

            string d = damage.ToString();
            int l = d.Length;
            switch (l) {
                case 1:
                    _text100.gameObject.SetActive(false);
                    _text010.gameObject.SetActive(false);
                
                    _text001.text = d[l - 1].ToString();

                    _text001.transform.localPosition = Vector3.zero;

                    Animate(_text001, 1f);
                    break;
                case 2:
                    _text100.gameObject.SetActive(false);

                    _text010.text = d[l - 2].ToString();
                    _text001.text = d[l - 1].ToString();
                
                    _text010.transform.localPosition = Vector3.left * 0.25f;
                    _text001.transform.localPosition = Vector3.right * 0.25f;
                
                    _text010.GetComponent<Renderer>().material.renderQueue += 1;

                    Animate(_text010, 1f);
                    Animate(_text001, 0.75f);
                    break;
                case 3:
                    _text100.text = d[l - 3].ToString();
                    _text010.text = d[l - 2].ToString();
                    _text001.text = d[l - 1].ToString();
                
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
                    G.U.Error("Damage must be 1~3 digits in length. Damage value is: " + d);
                    break;
            }
        }

#if NS_TMPRO
        void Animate(TMP_Text textMesh, float heightMult) {
#else
        void Animate(TextMesh textMesh, float heightMult) {
#endif
            Sequence s = DOTween.Sequence();
            Transform textTf = textMesh.transform;

            //Bounce.
            float h = 5f * heightMult;
            float t = 16f / 30f; //Total bounce time will actually be 15f / 30f.
            for (int i = 0; i < 4; i++) {
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
#if NS_TMPRO
            s.Join(textMesh.DOColor(Color.black, 4f / 30f).SetEase(Ease.Linear));
#else
            s.Join(textMesh.GetComponent<Renderer>().material.DOColor(Color.black, 4f / 30f).SetEase(Ease.Linear));
#endif
            
            //Wait.
            s.AppendInterval(2f / 30f);

            //Dispose.
            s.AppendCallback(Dispose);

            //Register sequence.
            _tweens.Add(s);
            timeThread.AddTween(s);
        }

        void Dispose() {
            G.End(gameObject);
        }

        void OnDestroy() {
            if (_target != null) {
                _target.end.actions -= OnTargetDestroy;
            }

            //some of the following may be null if the scene is changing or the app is quitting
            if (_text100 != null && _text100.gameObject.activeSelf) {
                DestroyImmediate(_text100.GetComponent<Renderer>().material);
            }
            if (_text010 != null && _text010.gameObject.activeSelf) {
                DestroyImmediate(_text010.GetComponent<Renderer>().material);
            }
            if (_text001 != null) {
                DestroyImmediate(_text001.GetComponent<Renderer>().material);
            }

            foreach (Sequence s in _tweens) {
                timeThread.RemoveTween(s);
            }
        }

        void OnTargetDestroy() {
            _target = null;
            transform.parent = null; //This needs to live on even if the target is destroyed.

            //this becomes disabled after reparenting, so we must re-enable it
            //TODO: find out why this happens (it happens in SoAm.TestDamageRing as well)
            //...and create a common solution for it
            var cfb = GetComponent<CameraFacingBillboard>();
            if (cfb != null) {
                cfb.enabled = true;
            }
        }
#else
        public void Init(MonoBehaviour target, int damage) {
            G.U.Error("This class requires DG.Tweening (DOTween).");
        }
#endif
    }
}
