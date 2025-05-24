using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class TextTypingEffect : MonoBehaviour
{
    [Header("Text Settings")]
    public Text textComponent;
    [TextArea(3, 10)]
    public string fullText;
    public float delayBetweenCharacters = 0.05f;
    public bool playOnAwake = true;
    public bool clearTextOnAwake = true;

    [Header("Optional Settings")]
    public AudioClip typingSound;
    private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent onTypingStart;
    public UnityEvent onCharacterTyped;
    public UnityEvent onTypingComplete;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<Text>();
            if (textComponent == null)
            {
                enabled = false;
                return;
            }
        }

        if (typingSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (clearTextOnAwake)
        {
            textComponent.text = "";
        }
    }

    void Start()
    {
        if (playOnAwake)
        {
            StartTyping(fullText);
        }
    }

    public void StartTyping(string textToType)
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
        }
        fullText = textToType;
        textComponent.text = "";
        typingCoroutine = StartCoroutine(TypeText());
    }

    public void StartTyping()
    {
         StartTyping(this.fullText);
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        onTypingStart.Invoke();

        foreach (char letter in fullText.ToCharArray())
        {
            textComponent.text += letter;
            if (typingSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(typingSound);
            }
            onCharacterTyped.Invoke();
            yield return new WaitForSeconds(delayBetweenCharacters);
        }

        isTyping = false;
        onTypingComplete.Invoke();
        typingCoroutine = null;
    }

    public void SkipTyping()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            textComponent.text = fullText;
            isTyping = false;
            onTypingComplete.Invoke();
            typingCoroutine = null;
        }
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public void SetTextAndPlay(string newText)
    {
        fullText = newText;
        StartTyping(fullText);
    }
}
