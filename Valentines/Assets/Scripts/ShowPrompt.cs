using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ShowPrompt : MonoBehaviour
{
    public TextMeshProUGUI promptUI;
    public TextMeshProUGUI messageUI;
    public TextMeshProUGUI tryAgain;
    public GameObject textBackground;
    public GameObject choicePanel;
    public GameObject balloons;
    public Button choiceButton1;
    public Button choiceButton2;
    public GameObject choiceMenuAI;
    public float textSpeed = 0.05f;
    public string[] messages;
    public string[] tryAgainMessages; // Array of "Try Again" messages
    public MonoBehaviour playerMovementScript;
    public AudioSource cameraAudioSource;
    public AudioSource selectAudioSource;
    public AudioClip dialogueMusic;

    public AudioClip selectNoise;

    private bool isPlayerNearby = false;
    private bool isTalking = false;
    private int messageIndex = 0;
    private bool isTyping = false;
    private AudioClip originalMusic;

    private int tryAgainIndex = 0; // Tracks which "Try Again" message to show next
    private Camera mainCamera;
    private float originalZoom;
    public float zoomInSize = 3.5f;
    public float zoomSpeed = 0.5f;

    private void Start()
    {
        promptUI.gameObject.SetActive(false);
        messageUI.gameObject.SetActive(false);
        textBackground.SetActive(false);
        choicePanel.SetActive(false);
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceMenuAI.SetActive(false);
        tryAgain.gameObject.SetActive(false);

        if (cameraAudioSource != null)
        {
            originalMusic = cameraAudioSource.clip;
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalZoom = mainCamera.orthographicSize;
        }

        // Assign button functions
        choiceButton1.onClick.AddListener(() => SelectChoice());
        choiceButton2.onClick.AddListener(() => ShowTryAgain());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            promptUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (isTalking)
            {
                EndDialogue();
            }
            else
            {
                promptUI.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isPlayerNearby && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }

        if (isTalking && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                messageUI.text = messages[messageIndex];
                isTyping = false;
            }
            else
            {
                messageIndex++;
                if (messageIndex < messages.Length)
                {
                    selectAudioSource.clip = selectNoise;
                    selectAudioSource.Play();

                    StartCoroutine(TypeLine());
                }
                else
                {
                    ShowChoices();
                }
            }
        }
    }

    private void StartDialogue()
    {
        isTalking = true;
        promptUI.gameObject.SetActive(false);
        messageUI.gameObject.SetActive(true);
        textBackground.SetActive(true);
        messageIndex = 0;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
        if (cameraAudioSource != null && dialogueMusic != null)
        {
            cameraAudioSource.clip = dialogueMusic;
            cameraAudioSource.Play();
        }

        StartCoroutine(ZoomCamera(zoomInSize));
        StartCoroutine(TypeLine());
    }

    private void ShowChoices()
    {
        messageUI.gameObject.SetActive(false);
        textBackground.SetActive(false);
        choicePanel.SetActive(true);
        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);
        choiceMenuAI.SetActive(true);
    }

    private void ShowTryAgain()
    {
        if (tryAgainMessages.Length == 0) return;

        tryAgain.gameObject.SetActive(true);
        tryAgain.text = tryAgainMessages[tryAgainIndex];

        tryAgainIndex = (tryAgainIndex + 1) % tryAgainMessages.Length; // Loop through messages
    }

    private void SelectChoice()
    {
        choicePanel.SetActive(false);
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceMenuAI.SetActive(false);
        tryAgain.gameObject.SetActive(false);
        balloons.SetActive(true);
        EndDialogue();
    }

    private void EndDialogue()
    {
        isTalking = false;
        if (cameraAudioSource != null && originalMusic != null)
        {
            cameraAudioSource.clip = originalMusic;
            cameraAudioSource.Play();
        }

        StartCoroutine(ZoomCamera(originalZoom, () =>
        {
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = true;
            }
        }));
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        messageUI.text = "";
        foreach (char c in messages[messageIndex].ToCharArray())
        {
            messageUI.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    IEnumerator ZoomCamera(float targetSize, System.Action onComplete = null)
    {
        float elapsedTime = 0;
        float startSize = mainCamera.orthographicSize;
        while (elapsedTime < zoomSpeed)
        {
            float t = elapsedTime / zoomSpeed;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, smoothT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.orthographicSize = targetSize;
        onComplete?.Invoke();
    }
}
