using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteSheetAnimator : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite[] sprites;
    public float framesPerSecond = 10f;
    public bool loop = true;
    public bool playOnAwake = true;

    [Header("Target Renderer (Optional)")]
    public Image uiImageTarget;
    public SpriteRenderer spriteRendererTarget;

    private int currentFrameIndex = 0;
    private float timer;
    private bool isPlaying = false;

    void Awake()
    {
        if (uiImageTarget == null)
        {
            uiImageTarget = GetComponent<Image>();
        }
        if (spriteRendererTarget == null)
        {
            spriteRendererTarget = GetComponent<SpriteRenderer>();
        }

        if (uiImageTarget == null && spriteRendererTarget == null)
        {
            enabled = false;
            return;
        }

        if (sprites == null || sprites.Length == 0)
        {
            enabled = false;
            return;
        }
    }

    void Start()
    {
        if (playOnAwake)
        {
            Play();
        }
        else
        {
            SetSprite(sprites[0]);
        }
    }

    void Update()
    {
        if (!isPlaying || sprites.Length == 0 || framesPerSecond <= 0)
        {
            return;
        }

        timer += Time.deltaTime;
        float frameDuration = 1f / framesPerSecond;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrameIndex++;

            if (currentFrameIndex >= sprites.Length)
            {
                if (loop)
                {
                    currentFrameIndex = 0;
                }
                else
                {
                    currentFrameIndex = sprites.Length - 1;
                    isPlaying = false;
                }
            }
            SetSprite(sprites[currentFrameIndex]);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (uiImageTarget != null)
        {
            uiImageTarget.sprite = sprite;
        }
        else if (spriteRendererTarget != null)
        {
            spriteRendererTarget.sprite = sprite;
        }
    }

    public void Play()
    {
        if (sprites == null || sprites.Length == 0) return;
        isPlaying = true;
        currentFrameIndex = 0;
        SetSprite(sprites[currentFrameIndex]);
        timer = 0f;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Resume()
    {
        if (sprites == null || sprites.Length == 0) return;
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
        currentFrameIndex = 0;
        if (sprites != null && sprites.Length > 0)
        {
            SetSprite(sprites[0]);
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}
