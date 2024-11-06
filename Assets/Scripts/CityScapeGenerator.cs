using System.Collections;
using UnityEngine;

public class CityscapeGenerator : MonoBehaviour
{
    // ---- Public variables ----
    public GameObject buildingPrefab;
    public GameObject roadPrefab;
    public int gridWidth = 10;
    public int gridDepth = 10;
    public float buildingSpacing = 3f;
    public float waveSpeed = 0.5f;

    private Coroutine _generationCoroutine;

    private bool _isGenerating = false;
    
    void Start()
    {
        // ---- Initial Generation ----
        _generationCoroutine = StartCoroutine(GenerateCityscapeWithWave());
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isGenerating)
        {
            if (_generationCoroutine != null)
            {
                StopCoroutine(_generationCoroutine);
            }
            ClearCityscape();
            _generationCoroutine = StartCoroutine(GenerateCityscapeWithWave());
        }
    }

    
    
    // ---- Coroutine to generate cityscape with wave effect ----
    IEnumerator GenerateCityscapeWithWave()
    {
        _isGenerating = true;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                // ---- Create buildings ----
                
                Vector3 position = new Vector3(x * buildingSpacing, 0, z * buildingSpacing);
                GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity);
                building.transform.parent = this.transform;

                float randomHeight = Random.Range(1f, 10f);
                building.transform.localScale = new Vector3(1, 0.1f, 1);

                StartCoroutine(AnimateBuildingHeight(building, randomHeight));

                // ---- Create roads ----
                
                if (x < gridWidth - 1)
                {
                    Vector3 roadPosX = new Vector3(position.x + buildingSpacing / 2, 0, position.z);
                    Instantiate(roadPrefab, roadPosX, Quaternion.identity, this.transform);
                }

                if (z < gridDepth - 1)
                {
                    Vector3 roadPosZ = new Vector3(position.x, 0, position.z + buildingSpacing / 2);
                    Instantiate(roadPrefab, roadPosZ, Quaternion.Euler(0, 90, 0), this.transform);
                }

                yield return new WaitForSeconds(waveSpeed);
            }
        }
        _isGenerating = false;
    }

    IEnumerator AnimateBuildingHeight(GameObject building, float targetHeight)
    {
        // ---- Check if building is null before starting the animation ----
        if (building == null) yield break;

        float initialHeight = building.transform.localScale.y;
        float popHeight = targetHeight * 1.2f; // ---- 20% bigger than target height for the pop effect ----
        float popDuration = 0.1f; // ---- Duration of the pop effect ----
        float settleDuration = 0.3f; // ---- Duration to settle back to target height ----

        // ---- Initial pop up ----
        float elapsedTime = 0f;
        while (elapsedTime < popDuration)
        {
            if (building != null)
            {
                // ---- Scale up to pop height ----
                float newHeight = Mathf.Lerp(initialHeight, popHeight, elapsedTime / popDuration);
                building.transform.localScale = new Vector3(1, newHeight, 1);
                elapsedTime += Time.deltaTime;
            }
            else
            {
                yield break; // ---- Exit if building was destroyed ----
            }
            yield return null;
        }

        // ---- Set height to pop height before the settle animation ----
        building.transform.localScale = new Vector3(1, popHeight, 1);

        // ---- Settle back down to target height ----
        elapsedTime = 0f;
        while (elapsedTime < settleDuration)
        {
            if (building != null)
            {
                // ---- Scale down to target height ----
                float newHeight = Mathf.Lerp(popHeight, targetHeight, elapsedTime / settleDuration);
                building.transform.localScale = new Vector3(1, newHeight, 1);
                elapsedTime += Time.deltaTime;
            }
            else
            {
                yield break; // ---- Exit if building was destroyed ----
            }
            yield return null;
        }

        // ---- Final scale setting ----
        if (building != null)
        {
            building.transform.localScale = new Vector3(1, targetHeight, 1);
        }
    }



    void ClearCityscape()
    {
        // ---- Destroy all child objects (buildings and roads) of this GameObject ----
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
