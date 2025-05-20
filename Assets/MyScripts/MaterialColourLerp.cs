using UnityEngine;

public class MaterialColourLerp : MonoBehaviour
{
    [Header("Material Setup")]
    public Material targetMaterial; // Assign the material in the Inspector

    [Header("Lerp Parameters")]
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public Color startEmissionColor = Color.black;
    public Color endEmissionColor = Color.yellow;
    public float duration = 2.0f;

    private float elapsedTime = 0f;
    private bool isLerping = false; // Flag to control if lerp is active
    private bool isInitialized = false; // Ensure material is set up once

    // Shader Property IDs for efficiency
    private static readonly int ColorProp = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");

    // Awake is called before Start and OnEnable, good for initialization
    void Awake()
    {
        InitializeMaterial();
    }

    // Called when the script instance is enabled
    void OnEnable()
    {
        // Automatically start the lerp if the material is ready
        if (isInitialized)
        {
            StartLerp();
        }
    }

    // Initializes the target material reference
    private void InitializeMaterial()
    {
        if (isInitialized) return; // Don't re-initialize

        if (targetMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Creates an instance of the material for this object only
                targetMaterial = renderer.material;
            }
        }

        if (targetMaterial != null)
        {
            // IMPORTANT: Ensure Emission is enabled on the material itself
            // in the Editor for the emission color to have an effect.
            // You can uncomment this line to try forcing it via script:
            // targetMaterial.EnableKeyword("_EMISSION");

            isInitialized = true; // Mark as initialized
        }
        else
        {
            Debug.LogError("ControllableMaterialLerp: No target material assigned or found on Renderer!", this);
            enabled = false; // Disable script if no material
        }
    }

    /// <summary>
    /// Starts or restarts the color and emission lerp process.
    /// </summary>
    public void StartLerp()
    {
        if (!isInitialized || targetMaterial == null)
        {
            Debug.LogWarning("Cannot start lerp - material not initialized.", this);
            return;
        }

        elapsedTime = 0f; // Reset timer
        isLerping = true; // Start the lerp process

        // Set initial colors immediately when lerp starts
        targetMaterial.SetColor(ColorProp, startColor);
        targetMaterial.SetColor(EmissionColorProp, startEmissionColor);
    }

    void Update()
    {
        // Only proceed if lerping, initialized, and material exists
        if (!isLerping || !isInitialized || targetMaterial == null) return;

        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Calculate the lerp factor (0 to 1)
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Lerp the base color
            targetMaterial.SetColor(ColorProp, Color.Lerp(startColor, endColor, t));

            // Lerp the emission color
            targetMaterial.SetColor(EmissionColorProp, Color.Lerp(startEmissionColor, endEmissionColor, t));
        }
        else
        {
            // Lerp finished
            // Ensure final colors are set exactly
            targetMaterial.SetColor(ColorProp, endColor);
            targetMaterial.SetColor(EmissionColorProp, endEmissionColor);
            isLerping = false; // Stop the lerp process in Update
        }
    }
}
