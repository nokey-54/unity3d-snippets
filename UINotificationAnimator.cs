using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class UINotificationAnimator : MonoBehaviour
{
    public enum SlideDirection
    {
        FromTop,
        FromBottom,
        FromLeft,
        FromRight
    }

    [Header("Target Panel")]
    public RectTransform panelRectTransform;

    [Header("Animation Settings")]
    public SlideDirection slideInDirection = SlideDirection.FromTop;
    public float animationDuration = 0.5f;
    public float onScreenDuration = 3f;
    public bool showOnEnable = false;
    public bool hideOnStart = true;

    [Header("Events")]
    public UnityEvent onShowStart;
    public UnityEvent onShowComplete;
    public UnityEvent onHideStart;
    public UnityEvent onHideComplete;

    private Vector2 onScreenAnchoredPosition;
    private Vector2 offScreenAnchoredPosition;
    private Coroutine activeNotificationCoroutine;
    private bool isShown = false;

    void Awake()
    {
        if (panelRectTransform == null)
        {
            panelRectTransform = GetComponent<RectTransform>();
        }

        onScreenAnchoredPosition = panelRectTransform.anchoredPosition;
        CalculateOffScreenPosition();

        if (hideOnStart)
        {
            panelRectTransform.anchoredPosition = offScreenAnchoredPosition;
        }
    }

    void OnEnable()
    {
        if (showOnEnable)
        {
            ShowNotification();
        }
    }

    void CalculateOffScreenPosition()
    {
        Rect panelRect = panelRectTransform.rect;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            offScreenAnchoredPosition = onScreenAnchoredPosition;
            return;
        }
        
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        
        Vector2 size = new Vector2(panelRect.width, panelRect.height);
        switch (slideInDirection)
        {
            case SlideDirection.FromTop:    offScreenAnchoredPosition = onScreenAnchoredPosition + new Vector2(0, size.y + 10); break;
            case SlideDirection.FromBottom: offScreenAnchoredPosition = onScreenAnchoredPosition - new Vector2(0, size.y + 10); break;
            case SlideDirection.FromLeft:   offScreenAnchoredPosition = onScreenAnchoredPosition - new Vector2(size.x + 10, 0); break;
            case SlideDirection.FromRight:  offScreenAnchoredPosition = onScreenAnchoredPosition + new Vector2(size.x + 10, 0); break;
        }
    }

    public void ShowNotification()
    {
        if (panelRectTransform == null) return;

        if (activeNotificationCoroutine != null)
        {
            StopCoroutine(activeNotificationCoroutine);
        }
        panelRectTransform.anchoredPosition = offScreenAnchoredPosition;
        activeNotificationCoroutine = StartCoroutine(AnimateNotification());
    }

    public void HideNotification()
    {
        if (panelRectTransform == null || !isShown) return;

        if (activeNotificationCoroutine != null)
        {
            StopCoroutine(activeNotificationCoroutine);
        }
        activeNotificationCoroutine = StartCoroutine(AnimateOut());
    }

    private IEnumerator AnimateNotification()
    {
        isShown = true;
        onShowStart.Invoke();

        yield return AnimatePanel(offScreenAnchoredPosition, onScreenAnchoredPosition, animationDuration);
        onShowComplete.Invoke();

        if (onScreenDuration > 0)
        {
            yield return new WaitForSeconds(onScreenDuration);
        }
        else
        {
            yield break; 
        }

        yield return AnimateOut();
    }
    
    private IEnumerator AnimateOut()
    {
        onHideStart.Invoke();
        yield return AnimatePanel(onScreenAnchoredPosition, offScreenAnchoredPosition, animationDuration);
        isShown = false;
        onHideComplete.Invoke();
        activeNotificationCoroutine = null;
    }

    private IEnumerator AnimatePanel(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panelRectTransform.anchoredPosition = endPos;
    }

    public void ShowNotificationWithMessage(string message, Text textComponent)
    {
        if (textComponent != null)
        {
            textComponent.text = message;
        }
        ShowNotification();
    }
}
