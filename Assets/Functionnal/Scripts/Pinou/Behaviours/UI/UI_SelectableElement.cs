#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou.UI
{
    [RequireComponent(typeof(RectTransform))]
    public partial class UI_SelectableElement : PinouBehaviour
    {
        [System.Serializable]
        public class SelectEffect : CustomDrawedProperty
        {
            public enum SelectEffectType
            {
                TextSize,
                TextBold,
                Color,
                ColorAnimated,
                SpriteChange,
                SelectionBorder,
                PlayAnimators
            }
            public enum _TargetType
            {
                Text,
                Image,
                Both
            }

            [SerializeField] private SelectEffectType _effectType;

            public SelectEffectType EffectType { get { return _effectType; } }

            [SerializeField] private float _unselectedTextSize;
            [SerializeField] private float _selectedTextSize;
            [SerializeField, Range(0.01f, 1f)] private float _textSizeChangeSpeed;

            public float UnselectedTextSize { get => _unselectedTextSize; }
            public float SelectedTextSize { get => _selectedTextSize; }
            public float TextSizeChangeSpeed { get => _textSizeChangeSpeed; }

            [SerializeField] private _TargetType _targetType;
            [SerializeField] private Color _unselectedColor;
            [SerializeField] private Color _selectedColor;
            [SerializeField] private Color[] _selectedColorAnimated;
            [SerializeField] private float _colorChangeSpeed;

            public _TargetType TargetType { get => _targetType; }
            public Color UnselectedColor { get => _unselectedColor; }
            public Color SelectedColor { get => _selectedColor; }
            public Color[] SelectedColorAnimated { get => _selectedColorAnimated; }
            public float ColorChangeSpeed { get => _colorChangeSpeed; }

            [SerializeField] private Sprite _unselectedSprite;
            [SerializeField] private Sprite _selectedSprite;

            public Sprite SelectedSprite { get => _selectedSprite; }
            public Sprite UnselectedSprite { get => _unselectedSprite; }

            [SerializeField] private Animator[] _animators;
            public Animator[] Animators { get => _animators; }


            private int _currentAnimatedColorIndex = 0;

            public void UpdateEffect(bool selected, UI_SelectableElement selectable, ref MaskableGraphic[] graphics, ref Image[] images, ref TextMeshProUGUI[] txts, ref MaskableGraphic[] excluded)
            {
                switch (_effectType)
                {
                    case SelectEffectType.TextSize:
                        UpdateTextSize(ref selected, ref txts, ref excluded);
                        break;
                    case SelectEffectType.TextBold:
                        UpdateTextBold(ref selected, ref txts, ref excluded);
                        break;
                    case SelectEffectType.Color:
                        switch (_targetType)
                        {
                            case _TargetType.Text:
                                UpdateColor(ref selected, ref txts, ref excluded);
                                break;
                            case _TargetType.Image:
                                UpdateColor(ref selected, ref images, ref excluded);
                                break;
                            case _TargetType.Both:
                                UpdateColor(ref selected, ref graphics, ref excluded);
                                break;
                        }
                        break;
                    case SelectEffectType.ColorAnimated:
                        switch (_targetType)
                        {
                            case _TargetType.Text:
                                UpdateColorAnimated(ref selected, ref txts, ref excluded);
                                break;
                            case _TargetType.Image:
                                UpdateColorAnimated(ref selected, ref images, ref excluded);
                                break;
                            case _TargetType.Both:
                                UpdateColorAnimated(ref selected, ref graphics, ref excluded);
                                break;
                        }
                        break;
                    case SelectEffectType.SpriteChange:
                        UpdateSprite(ref selected, ref images, ref excluded);
                        break;
                    case SelectEffectType.SelectionBorder:
                        UpdateSelectionBorder(ref selected, selectable);
                        break;
                    case SelectEffectType.PlayAnimators:
                        UpdateAnimators(ref selected, ref excluded);
                        break;
                }
            }

            private void UpdateTextSize(ref bool selected, ref TextMeshProUGUI[] txts, ref MaskableGraphic[] excluded)
            {
                for (int i = 0; i < txts.Length; i++)
                {
                    if (excluded.Contains(txts[i]) == true)
                    {
                        continue;
                    }

                    txts[i].fontSize = Mathf.Lerp(
                        txts[i].fontSize,
                        selected ? _selectedTextSize : _unselectedTextSize,
                        _textSizeChangeSpeed);
                }
            }
            private void UpdateTextBold(ref bool selected, ref TextMeshProUGUI[] txts, ref MaskableGraphic[] excluded)
            {
                for (int i = 0; i < txts.Length; i++)
                {
                    if (excluded.Contains(txts[i]) == true)
                    {
                        continue;
                    }

                    txts[i].fontStyle = selected ?
                        txts[i].fontStyle | FontStyles.Bold :
                        txts[i].fontStyle & ~FontStyles.Bold;
                }
            }

            private Color GetCurrentColor(ref bool selected)
            {
                if (selected == false)
                {
                    return _unselectedColor;
                }

                switch (_effectType)
                {
                    case SelectEffectType.TextSize:
                    case SelectEffectType.TextBold:
                    case SelectEffectType.Color:
                        return _selectedColor;
                    case SelectEffectType.ColorAnimated:
                        return _selectedColorAnimated[_currentAnimatedColorIndex % _selectedColorAnimated.Length];
                }

                return default;
            }
            private void UpdateColor(ref bool selected, ref TextMeshProUGUI[] txts, ref MaskableGraphic[] excluded)
            {
                for (int i = 0; i < txts.Length; i++)
                {
                    if (excluded.Contains(txts[i]) == true)
                    {
                        continue;
                    }
                    txts[i].color = txts[i].color.MoveTowards(GetCurrentColor(ref selected), _colorChangeSpeed * Time.unscaledDeltaTime);
                }
            }
            private void UpdateColor(ref bool selected, ref Image[] images, ref MaskableGraphic[] excluded)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    if (excluded.Contains(images[i]) == true)
                    {
                        continue;
                    }
                    images[i].color = images[i].color.MoveTowards(GetCurrentColor(ref selected), _colorChangeSpeed * Time.unscaledDeltaTime);
                }
            }
            private void UpdateColor(ref bool selected, ref MaskableGraphic[] graphics, ref MaskableGraphic[] excluded)
            {
                for (int i = 0; i < graphics.Length; i++)
                {
                    if (excluded.Contains(graphics[i]) == true)
                    {
                        continue;
                    }
                    graphics[i].color = graphics[i].color.MoveTowards(GetCurrentColor(ref selected), _colorChangeSpeed * Time.unscaledDeltaTime);
                }
            }

            private void UpdateColorAnimated(ref bool selected, ref TextMeshProUGUI[] txts, ref MaskableGraphic[] excluded)
            {
                if (selected == false)
                {
                    UpdateColor(ref selected, ref txts, ref excluded);
                    return;
                }
                bool shouldIncreaseIndex = true;
                UpdateColor(ref selected, ref txts, ref excluded);
                for (int i = 0; i < txts.Length; i++)
                {
                    if (excluded.Contains(txts[i]) == true)
                    {
                        continue;
                    }
                    if (txts[i].color != GetCurrentColor(ref selected))
                    {
                        shouldIncreaseIndex = false;
                        break;
                    }
                }
                if (shouldIncreaseIndex == true)
                {
                    _currentAnimatedColorIndex++;
                }
            }
            private void UpdateColorAnimated(ref bool selected, ref Image[] images, ref MaskableGraphic[] excluded)
            {
                if (selected == false)
                {
                    UpdateColor(ref selected, ref images, ref excluded);
                    return;
                }
                bool shouldIncreaseIndex = true;
                UpdateColor(ref selected, ref images, ref excluded);
                for (int i = 0; i < images.Length; i++)
                {
                    if (excluded.Contains(images[i]) == true)
                    {
                        continue;
                    }
                    if (images[i].color != GetCurrentColor(ref selected))
                    {
                        shouldIncreaseIndex = false;
                        break;
                    }
                }
                if (shouldIncreaseIndex == true)
                {
                    _currentAnimatedColorIndex++;
                }
            }
            private void UpdateColorAnimated(ref bool selected, ref MaskableGraphic[] graphics, ref MaskableGraphic[] excluded)
            {
                if (selected == false)
                {
                    UpdateColor(ref selected, ref graphics, ref excluded);
                    return;
                }
                bool shouldIncreaseIndex = true;
                UpdateColor(ref selected, ref graphics, ref excluded);
                for (int i = 0; i < graphics.Length; i++)
                {
                    if (excluded.Contains(graphics[i]) == true)
                    {
                        continue;
                    }
                    if (graphics[i].color != GetCurrentColor(ref selected))
                    {
                        shouldIncreaseIndex = false;
                        break;
                    }
                }
                if (shouldIncreaseIndex == true)
                {
                    _currentAnimatedColorIndex++;
                }
            }

            private void UpdateSprite(ref bool selected, ref Image[] imgs, ref MaskableGraphic[] excluded)
            {
                for (int i = 0; i < imgs.Length; i++)
                {
                    if (excluded.Contains(imgs[i]) == true)
                    {
                        continue;
                    }
                    imgs[i].sprite = selected ? _selectedSprite : _unselectedSprite;
                }
            }

            private void UpdateSelectionBorder(ref bool selected, UI_SelectableElement selectable)
            {
                if (selected == false)
                {
                    return;
                }

                UI_SelectorManager.ApplySelectionBorder(selectable.RectTransform);
            }

            private void UpdateAnimators(ref bool selected, ref MaskableGraphic[] excluded)
            {
                if (selected == true)
                {
                    foreach (Animator animator in _animators)
                    {
                        if (excluded.Contains(animator.transform.GetComponent<MaskableGraphic>()))
                        { continue; }
                        animator.speed = 1;
                    }
                    _animators.ForEach(a => a.speed = 1);
                }
                else
                {
                    foreach (Animator animator in _animators)
                    {
                        if (excluded.Contains(animator.transform.GetComponent<MaskableGraphic>()))
                        { continue; }
                        animator.speed = 0;
                        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                    }
                }
            }
        }

        private static UI_SelectableElement _currentSelectedElement;
        public static UI_SelectableElement CurrentSelectedElement { get => _currentSelectedElement; }

        [SerializeField] private bool _selectedSelf = false;
        public bool Selected { get { return _selectedSelf == true ? true : ChildSelected; } }

        [SerializeField] private UI_SelectableElement[] _childSelectables = new UI_SelectableElement[0];
        public bool ChildSelected
        {
            get
            {
                for (int i = 0; i < _childSelectables.Length; i++)
                {
                    if (_childSelectables[i].Selected == true)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [SerializeField, HideInInspector] private UI_SelectableElement _upSelect;
        [SerializeField, HideInInspector] private UI_SelectableElement _downSelect;
        [SerializeField, HideInInspector] private UI_SelectableElement _rightSelect;
        [SerializeField, HideInInspector] private UI_SelectableElement _leftSelect;
        [Space]
        [SerializeField] private SelectEffect[] _selectEffects;
        [SerializeField] private UnityEvent _onSelectEffects;

        [SerializeField] private MaskableGraphic[] _excludedGraphics;
        [SerializeField] private MaskableGraphic[] _graphics;
        [SerializeField] private Image[] _images;
        [SerializeField] private TextMeshProUGUI[] _txts;

        public SelectEffect[] SelectEffects { get { return _selectEffects; } }
        public UnityEvent OnSelectEffects { get { return _onSelectEffects; } }

        protected override void OnAwake()
        {
            if (_selectedSelf == true)
            {
                _currentSelectedElement = this;
            }
        }

        private void Update()
        {
            for (int i = 0; i < _selectEffects.Length; i++)
            {
                _selectEffects[i].UpdateEffect(Selected, this, ref _graphics, ref _images, ref _txts, ref _excludedGraphics);
            }
        }

        public UI_SelectableElement GetNeighbour(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right:
                    return _rightSelect;
                case Direction.Left:
                    return _leftSelect;
                case Direction.Up:
                    return _upSelect;
                case Direction.Down:
                    return _downSelect;
            }
            return null;
        }
        public PinouUtils.Delegate.Action<UI_SelectableElement> OnSelect { get; private set; } = new PinouUtils.Delegate.Action<UI_SelectableElement>();
        public void Select()
        {
            if (GetComponent<Selectable>() && GetComponent<Selectable>().interactable == false)
            {
                return;
            }

            if (_currentSelectedElement != null)
            {
                _currentSelectedElement._selectedSelf = false;
            }

            _currentSelectedElement = this;
            _selectedSelf = true;
            _onSelectEffects.Invoke();
            OnSelect.Invoke(this);
        }
        public void Deselect()
        {
            if (_selectedSelf == true)
            {
                _currentSelectedElement = null;
                _selectedSelf = false;
            }
        }
        public void Invoke()
        {
            GetComponent<Button>().onClick.Invoke();
        }

#if UNITY_EDITOR
        public float E_NavigationGizmosOffset = 10f;
        public float E_NavigationDotSize = 2f;
        protected override void E_OnDrawGizmos()
        {
            if (_upSelect != null)
            {
                if (_downSelect == null)
                {
                    DrawLine(this, _upSelect, E_NavigationGizmosOffset, Color.white);
                }
                else if (_upSelect.transform.position.y > transform.position.y)
                {
                    DrawLine(this, _upSelect, E_NavigationGizmosOffset, Color.white);
                }
                else
                {
                    DrawLine(this, _upSelect, E_NavigationGizmosOffset * 3f, Color.red);
                }
            }
            if (_downSelect != null)
            {
                if (_upSelect == null)
                {
                    DrawLine(this, _downSelect, E_NavigationGizmosOffset, Color.white);
                }
                else if (_downSelect.transform.position.y < transform.position.y)
                {
                    DrawLine(this, _downSelect, E_NavigationGizmosOffset, Color.white);
                }
                else
                {
                    DrawLine(this, _downSelect, E_NavigationGizmosOffset * 3f, Color.red);
                }
            }
            if (_leftSelect != null)
            {
                if (_rightSelect == null)
                {
                    DrawLine(this, _leftSelect, E_NavigationGizmosOffset, Color.white);
                }
                else if (_leftSelect.transform.position.x < transform.position.x)
                {
                    DrawLine(this, _leftSelect, E_NavigationGizmosOffset, Color.white);
                }
                else
                {
                    DrawLine(this, _leftSelect, E_NavigationGizmosOffset * 3f, Color.red);
                }
            }
            if (_rightSelect != null)
            {
                if (_leftSelect == null)
                {
                    DrawLine(this, _rightSelect, E_NavigationGizmosOffset, Color.white);
                }
                else if (_rightSelect.transform.position.x > transform.position.x)
                {
                    DrawLine(this, _rightSelect, E_NavigationGizmosOffset, Color.white);
                }
                else
                {
                    DrawLine(this, _rightSelect, E_NavigationGizmosOffset * 3f, Color.red);
                }
            }
        }

        protected void DrawLine(UI_SelectableElement a, UI_SelectableElement b, float offset, Color color)
        {
            Gizmos.color = color;
            RectTransform aRect = a.RectTransform, bRect = b.RectTransform;
            Vector3 dir = (bRect.position - aRect.position).normalized;
            Vector3 offsetDir = Vector3.Cross(dir, Vector3.forward).normalized;
            Vector3 aPoint = aRect.position + offsetDir * offset;
            Vector3 bPoint = bRect.position + offsetDir * offset;
            Gizmos.DrawWireSphere(aPoint, E_NavigationDotSize);
            Gizmos.DrawWireSphere(bPoint, E_NavigationDotSize);
            Gizmos.DrawLine(aPoint, bPoint);
            Vector3 mid = (aPoint + bPoint) * 0.5f;
            aPoint = mid - dir * E_NavigationGizmosOffset * 0.5f + offsetDir * E_NavigationGizmosOffset * 0.5f;
            bPoint = mid - dir * E_NavigationGizmosOffset * 0.5f - offsetDir * E_NavigationGizmosOffset * 0.5f;
            Gizmos.DrawLine(mid, aPoint);
            Gizmos.DrawLine(mid, bPoint);

        }
        protected override void E_OnValidate()
        {
            _graphics = GetComponentsInChildren<MaskableGraphic>();
            _images = GetComponentsInChildren<Image>();
            _txts = GetComponentsInChildren<TextMeshProUGUI>();

            if (_selectedSelf == true)
            {
                foreach (UI_SelectableElement s in FindObjectsOfType<UI_SelectableElement>())
                {
                    if (s == this)
                    {
                        continue;
                    }

                    s._selectedSelf = false;
                }
            }
        }
#endif
    }


#if UNITY_EDITOR
    public partial class UI_SelectableElement : PinouBehaviour
    {
        [CustomEditor(typeof(UI_SelectableElement), true), CanEditMultipleObjects]
        public class UI_SelectableElementEditor : PinouEditor
        {
            protected override void InspectorGUI()
            {
                DrawScriptField();
                HandleDrawNavigation();
                SelectedVisualField();
                DefaultInspectorGUI(true);
                ButtonsExtension();
            }

            protected void SelectedVisualField()
            {
                GUILayout.Space(100f);
                bool old = GUI.enabled;
                GUI.enabled = false;
                GUILayout.Toggle(((UI_SelectableElement)target).Selected, "Selected");
                GUI.enabled = old;
            }
            protected void HandleDrawNavigation()
            {
                TextAnchor old = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;

                Rect rect = GUILayoutUtility.GetRect(0, 17f);
                rect.y += 17f;
                rect.width = Screen.width * 0.25f;
                rect.x = Screen.width * 0.5f - rect.width * 0.5f;
                EditorGUI.PropertyField(rect, serializedObject.FindProperty("_upSelect"), GUIContent.none, true);
                rect.y += 17f;
                GUI.Label(rect, "UP");
                rect.y += 17f;

                rect.x = 0f;
                EditorGUI.PropertyField(rect, serializedObject.FindProperty("_leftSelect"), GUIContent.none, true);
                rect.x += rect.width - Screen.width * 0.085f;
                GUI.Label(rect, "LEFT");

                rect.x = Screen.width * 0.95f - rect.width;
                EditorGUI.PropertyField(rect, serializedObject.FindProperty("_rightSelect"), GUIContent.none, true);
                rect.x -= rect.width - Screen.width * 0.075f;
                GUI.Label(rect, "RIGHT");
                rect.y += 34f;
                rect.x = Screen.width * 0.5f - rect.width * 0.5f;
                EditorGUI.PropertyField(rect, serializedObject.FindProperty("_downSelect"), GUIContent.none, true);
                rect.y -= 17f;
                GUI.Label(rect, "DOWN");

                GUI.skin.label.alignment = old;
                serializedObject.ApplyModifiedProperties();
            }
            protected void ButtonsExtension()
            {
                if (GUILayout.Button("Auto Find Neighbours"))
                {
                    IEnumerable<UI_SelectableElement> Instances = targets.Cast<UI_SelectableElement>();
                    foreach (UI_SelectableElement Instance in Instances)
                    {
                        UI_SelectableElement[] allElements = FindObjectsOfType<UI_SelectableElement>();
                        List<UI_SelectableElement> elements;
                        if (allElements.Length <= 1)
                        {
                            return;
                        }
                        //Up
                        elements = new List<UI_SelectableElement>(allElements.OrderBy(e => (e.transform.position.y - Instance.transform.position.y)));
                        elements.Remove(Instance);
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if (elements[i].isActiveAndEnabled == false)
                            {
                                continue;
                            }
                            if (elements[i].transform.position.y > Instance.transform.position.y)
                            {
                                Instance._upSelect = elements[i];
                                break;
                            }
                        }
                        //Down
                        elements = new List<UI_SelectableElement>(allElements.OrderBy(e => (Instance.transform.position.y - e.transform.position.y)));
                        elements.Remove(Instance);
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if (elements[i].isActiveAndEnabled == false)
                            {
                                continue;
                            }
                            if (elements[i].transform.position.y < Instance.transform.position.y)
                            {
                                Instance._downSelect = elements[i];
                                break;
                            }
                        }
                        //left
                        elements = new List<UI_SelectableElement>(allElements.OrderBy(e => (Instance.transform.position.x - e.transform.position.x)));
                        elements.Remove(Instance);
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if (elements[i].isActiveAndEnabled == false)
                            {
                                continue;
                            }
                            if (elements[i].transform.position.x < Instance.transform.position.x)
                            {
                                Instance._leftSelect = elements[i];
                                break;
                            }
                        }
                        //right
                        elements = new List<UI_SelectableElement>(allElements.OrderBy(e => (e.transform.position.x - Instance.transform.position.x)));
                        elements.Remove(Instance);
                        for (int i = 0; i < elements.Count; i++)
                        {
                            if (elements[i].isActiveAndEnabled == false)
                            {
                                continue;
                            }
                            if (elements[i].transform.position.x > Instance.transform.position.x)
                            {
                                Instance._rightSelect = elements[i];
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(UI_SelectableElement.SelectEffect))]
    public class SelectEffectPropertyDrawer : PropertyDrawerExtended
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            Space(15f);
            PropField("_effectType");
            Space(7.5f);

            UI_SelectableElement.SelectEffect.SelectEffectType type =
                (UI_SelectableElement.SelectEffect.SelectEffectType)Prop("_effectType").enumValueIndex;

            switch (type)
            {
                case UI_SelectableElement.SelectEffect.SelectEffectType.TextSize:
                    PropField("_unselectedTextSize");
                    PropField("_selectedTextSize");
                    PropField("_textSizeChangeSpeed");
                    break;
                case UI_SelectableElement.SelectEffect.SelectEffectType.Color:
                    PropField("_targetType");
                    PropField("_unselectedColor");
                    PropField("_selectedColor");
                    PropField("_colorChangeSpeed");
                    break;
                case UI_SelectableElement.SelectEffect.SelectEffectType.ColorAnimated:
                    PropField("_targetType");
                    PropField("_unselectedColor");
                    PropField("_selectedColorAnimated");
                    PropField("_colorChangeSpeed");
                    break;
                case UI_SelectableElement.SelectEffect.SelectEffectType.SpriteChange:
                    PropField("_unselectedSprite");
                    PropField("_selectedSprite");
                    break;
                case UI_SelectableElement.SelectEffect.SelectEffectType.PlayAnimators:
                    PropField("_animators");
                    break;
            }


        }
    }
#endif
}