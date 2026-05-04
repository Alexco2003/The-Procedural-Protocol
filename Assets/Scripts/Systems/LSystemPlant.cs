using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystemPlant : MonoBehaviour
{
    [System.Serializable]
    public struct Rule
    {
        public char letter;
        public string result;
    }

    [Header("L-System Grammar")]
    public string axiom = "X";
    public Rule[] rules;
    public int iterations = 5;
    public float angle = 25.7f;
    public float branchLength = 0.1f;

    [Header("3D Settings")]
    public GameObject branchPrefab;
    public Color plantColor = new Color(0.2f, 0.8f, 0.2f);
    [Range(0f, 1f)] public float randomness = 0.1f;
    public float branchThickness = 0.05f;

    private string generatedString;

    private struct TransformState
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    void Start()
    {
        branchLength = Random.Range(0.05f, 0.15f);
        branchThickness = Random.Range(0.03f, 0.07f);
        randomness = Random.Range(0.05f, 0.15f);

        plantColor = Random.ColorHSV(0.0f, 1.0f, 0.5f, 1.0f, 0.5f, 1.0f);

        GenerateString();
        DrawPlant();

        CombineMeshes();
    }

    private void GenerateString()
    {
        generatedString = axiom;

        for (int i = 0; i < iterations; i++)
        {
            StringBuilder currentString = new StringBuilder();

            foreach (char c in generatedString)
            {
                bool ruleApplied = false;
                foreach (Rule rule in rules)
                {
                    if (c == rule.letter)
                    {
                        currentString.Append(rule.result);
                        ruleApplied = true;
                        break;
                    }
                }

                if (!ruleApplied)
                {
                    currentString.Append(c);
                }
            }
            generatedString = currentString.ToString();
        }
    }

    private void DrawPlant()
    {
        Stack<TransformState> transformStack = new Stack<TransformState>();
        Vector3 currentPosition = Vector3.zero;
        Quaternion currentRotation = Quaternion.identity;

        foreach (char cmd in generatedString)
        {
            if (cmd == 'F')
            {
                float currentLength = branchLength * (1.0f + Random.Range(-randomness, randomness));
                Vector3 newPosition = currentPosition + currentRotation * Vector3.up * currentLength;

                GameObject branch = Instantiate(branchPrefab, transform);
                branch.transform.localPosition = currentPosition;
                branch.transform.localRotation = currentRotation;
                branch.transform.localScale = new Vector3(branchThickness, currentLength, branchThickness);

                currentPosition = newPosition;
            }
            else if (cmd == '+')
            {
                float randomTwist = Random.Range(-20f, 20f);
                currentRotation *= Quaternion.Euler(0, randomTwist, angle);
            }
            else if (cmd == '-')
            {
                float randomTwist = Random.Range(-20f, 20f);
                currentRotation *= Quaternion.Euler(0, randomTwist, -angle);
            }
            else if (cmd == '[')
            {
                transformStack.Push(new TransformState() { position = currentPosition, rotation = currentRotation });
            }
            else if (cmd == ']')
            {
                TransformState state = transformStack.Pop();
                currentPosition = state.position;
                currentRotation = state.rotation;
            }
        }
    }

    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;

            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter myMeshFilter = gameObject.AddComponent<MeshFilter>();
        myMeshFilter.mesh = new Mesh();
        myMeshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        myMeshFilter.mesh.CombineMeshes(combine);

        MeshRenderer myRenderer = gameObject.AddComponent<MeshRenderer>();
        if (meshFilters.Length > 0 && meshFilters[0].GetComponent<MeshRenderer>() != null)
        {
            myRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
            myRenderer.material.color = plantColor;
        }

        MeshCollider myCollider = gameObject.AddComponent<MeshCollider>();
        myCollider.sharedMesh = myMeshFilter.sharedMesh;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}