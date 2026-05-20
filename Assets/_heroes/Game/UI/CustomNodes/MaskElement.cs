using UnityEngine;
using UnityEngine.UIElements;

namespace OneJS.Dom
{
    public class MaskElement : VisualElement
    {
        public string MaskSrc
        {
            get => _maskSrc;
            set => SetMask(value);
        }

        private string _maskSrc;
        private readonly VisualElement _content;

        public override VisualElement contentContainer => _content;

        public MaskElement()
        {
            style.position = Position.Relative;
            style.overflow = Overflow.Hidden;

            usageHints |= UsageHints.MaskContainer;

            _content = new VisualElement();
            _content.style.position = Position.Absolute;
            _content.style.left = 0;
            _content.style.top = 0;
            _content.style.right = 0;
            _content.style.bottom = 0;

            hierarchy.Add(_content);
        }

        private void SetMask(string src)
        {
            _maskSrc = src;

            if (string.IsNullOrEmpty(src))
            {
                style.backgroundImage = StyleKeyword.None;
                return;
            }

            var vectorImage = Resources.Load<VectorImage>(src);
            
            if (vectorImage == null)
            {
                Debug.LogWarning($"MaskElement: could not load VectorImage from Resources/{src}");
                style.backgroundImage = StyleKeyword.None;
                return;
            }

            style.backgroundImage = new StyleBackground(vectorImage);
        }
    }
}

