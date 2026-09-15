using UnityEngine;
using System.Collections;

public class IntroCameraPanner : MonoBehaviour
{
    [Header("System References")]
    public IntroSequenceManager introManager;

    [Header("Camera Targets (Empty GameObjects)")]
    public Transform goddessView;   // Where the camera starts
    public Transform statMenuView;  // Where the camera looks during stat allocation
    public Transform portalView;    // Where the camera looks before transitioning

    [Header("Pan Settings")]
    public float panDuration = 2.0f;
    // An animation curve allows you to make the movement smooth (e.g., ease-in, ease-out)
    public AnimationCurve panCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool isPanning = false;

    void Start()
    {
        // Snap the camera to the starting Goddess view immediately
        if (goddessView != null)
        {
            transform.position = goddessView.position;
            transform.rotation = goddessView.rotation;
        }
    }

    // --- Public Triggers ---

    public void PanToStatMenu()
    {
        if (!isPanning)
        {
            StartCoroutine(PanToTarget(statMenuView, () =>
            {
                // Tell the Intro Manager we arrived, so it can open the UI
                introManager.PanCamera();
            }));
        }
    }

    public void PanToPortal()
    {
        if (!isPanning)
        {
            StartCoroutine(PanToTarget(portalView, () =>
            {
                // Tell the Intro Manager we arrived, so it can load the next scene
                introManager.OnPortalPanComplete();
            }));
        }
    }

    public void PanToGoddess()
    {
        if (!isPanning)
        {
            StartCoroutine(PanToTarget(goddessView, () =>
            {
                // Tell the Intro Manager we arrived, so it can start the dialogue
                introManager.OnGoddessPanComplete();
            }));
        }
    }

    // --- The Panning Logic ---

    private IEnumerator PanToTarget(Transform target, System.Action onComplete)
    {
        isPanning = true;

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < panDuration)
        {
            elapsedTime += Time.deltaTime;
            float timeRatio = elapsedTime / panDuration;
            float curveRatio = panCurve.Evaluate(timeRatio);

            // Interpolate X and Y as a Vector2
            Vector2 lerpedPos = Vector2.Lerp(startPosition, target.position, curveRatio);

            // Apply X and Y, but safely lock the Z depth to the camera's starting Z
            transform.position = new Vector3(lerpedPos.x, lerpedPos.y, startPosition.z);

            yield return null;
        }

        // Snap exactly to the target X/Y at the end, maintaining Z depth
        transform.position = new Vector3(target.position.x, target.position.y, startPosition.z);

        isPanning = false;
        onComplete?.Invoke();
    }
}