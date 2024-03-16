using UnityEngine;
using System;

namespace DG.Tweening
{
    public static class DOTweenCustomExtensions
    {
        public static Sequence DOCartoonJumpUpAnchored(
            this RectTransform rectTransform
            , float hight
            , float duration
            , float jellyPower = 1
            , System.Action onJumpMoment = null
            , System.Action onLandingMoment = null)
        {
            float halfTime = duration / 2;
            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 endPos = startPos;
            endPos.y += hight;
            var sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOScaleX(1 + .2f * jellyPower, halfTime / 4));
            sequence.Join(rectTransform.DOScaleY(1 - .2f * jellyPower, halfTime / 4));
            sequence.Append(rectTransform.DOScaleX(1 - .1f * jellyPower, halfTime / 4));
            sequence.Join(rectTransform.DOScaleY(1 + .1f * jellyPower, halfTime / 4));
            sequence.Append(rectTransform.DOAnchorPosY(endPos.y, halfTime / 2).OnStart(()=> onJumpMoment?.Invoke()));
            sequence.Join(rectTransform.DOScaleX(1 + .2f * jellyPower, halfTime / 2));
            sequence.Join(rectTransform.DOScaleY(1 - .2f * jellyPower, halfTime / 2));

            sequence.Append(rectTransform.DOAnchorPosY(startPos.y, halfTime / 2));
            sequence.Join(rectTransform.DOScaleX(1f, halfTime / 2));
            sequence.Join(rectTransform.DOScaleY(1f, halfTime / 2));
            sequence.Append(rectTransform.DOScaleX(1 + .2f * jellyPower, halfTime / 4).OnStart(() => onLandingMoment?.Invoke()));
            sequence.Join(rectTransform.DOScaleY(1 - .2f * jellyPower, halfTime / 4));
            sequence.Append(rectTransform.DOScaleX(1 - .1f * jellyPower, halfTime / 4));
            sequence.Join(rectTransform.DOScaleY(1 + .1f * jellyPower, halfTime / 4));
            sequence.Append(rectTransform.DOScaleX(1, halfTime / 4));
            sequence.Join(rectTransform.DOScaleY(1, halfTime / 4));

            return sequence;
        }

        public static Sequence DOJellyYoyo(this Transform transform, float scaleOut, float scaleIn, float time, float finalScale = -1)
        {
            Vector3 originScale;
            if (finalScale == -1)
            {
                originScale = transform.localScale;
            }
            else
            {
                originScale = Vector3.one * finalScale;
            }
 
            float partTime = time / 3;
            var sequence = DOTween.Sequence();
            var tween1 = transform.DOScale(scaleOut, partTime);
            sequence.Append(tween1);
            var tween2 = transform.DOScale(scaleIn, partTime);
            sequence.Append(tween2);
            var tween3 = transform.DOScale(originScale, partTime);
            sequence.Append(tween3);
            return sequence;
        }

        public static Sequence DoColorBlink(this UnityEngine.UI.Image image, Color targetColor, float duration, float maxColorPoint = 0.33f)
        {
            Color originColor = image.color;
            float firstPhaseTime = duration * maxColorPoint;
            float secondPhaseTime = duration - firstPhaseTime;
            var sequence = DOTween.Sequence();
            var tween1 = image.DOColor(targetColor, firstPhaseTime);
            sequence.Append(tween1);
            var tween2 = image.DOColor(originColor, secondPhaseTime);
            sequence.Append(tween2);
            return sequence;
        }

        public static Sequence DOJellyRotation(this Transform transform, float maxAngle, int phases, float duration)
        {
            float phaseDuration = duration / phases;
            float angle = Math.Abs(maxAngle);
            float degreeOffset = angle / phases;
            bool isAbsolute = maxAngle > 0;

            Vector3 rotation = new Vector3(0, 0, maxAngle);
            var sequence = DOTween.Sequence();
            for (int i = 0; i < phases; i++)
            {
                sequence.Append(transform.DORotate(rotation, phaseDuration / 2).SetEase(Ease.Linear));
                sequence.Append(transform.DORotate(Vector3.zero, phaseDuration / 2).SetEase(Ease.Linear));
                angle = angle - degreeOffset;
                isAbsolute = !isAbsolute;
                float rotZ = isAbsolute ? angle : GetReverted(angle);
                rotation.z = rotZ;
            }

            return sequence;

            float GetReverted(float inputed)
            {
                return inputed * -1;
            }
        }
    }
}




