using System.Collections;
using UnityEngine;

public class ColorTrap : MonoBehaviour
{
    public float damage;
    public float fuseTime = 1.5f;
    public float cooldownTime = 2f;

    private bool isTriggered = false;
    private Renderer rend;
    private Material mat;

    [ColorUsage(true, true)] public Color safeColor = Color.white;
    [ColorUsage(true, true)] public Color dangerColor = Color.red;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        mat = rend.material;

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", safeColor);

        BoxCollider trigger = gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(0.9f, 1f, 0.9f);
        trigger.center = new Vector3(0, 0.5f, 0);

        damage = Random.Range(15f, 35f);
    }

    private void OnTriggerStay(Collider other)
    {

        if (!isTriggered && (other.CompareTag("Player") || other.transform.root.CompareTag("Player") || other.GetComponent<NPCController>()))
        {
            StartCoroutine(TrapSequence());
        }
    }

    IEnumerator TrapSequence()
    {
        isTriggered = true;
        float timer = 0f;

        while (timer < fuseTime)
        {
            timer += Time.deltaTime;
            float percent = timer / fuseTime; 
            Color lerpedColor = Color.Lerp(safeColor, dangerColor, percent);
            mat.SetColor("_EmissionColor", lerpedColor);
            yield return null; 
        }

        mat.SetColor("_EmissionColor", dangerColor);
        DealDamage();

        yield return new WaitForSeconds(0.25f);

        timer = 0f;
        while (timer < cooldownTime)
        {
            timer += Time.deltaTime;
            float percent = timer / cooldownTime;
            Color lerpedColor = Color.Lerp(dangerColor, safeColor, percent);
            mat.SetColor("_EmissionColor", lerpedColor);
            yield return null;
        }

        mat.SetColor("_EmissionColor", safeColor);
        isTriggered = false;
    }

    void DealDamage()
    {
        Collider[] hits = Physics.OverlapBox(transform.position + new Vector3(0, 0.5f, 0), new Vector3(0.45f, 0.5f, 0.45f));

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") || hit.transform.root.CompareTag("Player"))
            {
                PlayerStats pStats = hit.transform.root.GetComponent<PlayerStats>();
                if (pStats != null) pStats.TakeDamage(damage);
            }
            else
            {
                NPCController npc = hit.GetComponent<NPCController>();
                if (npc != null) npc.TakeDamage(damage);
            }
        }
    }
}