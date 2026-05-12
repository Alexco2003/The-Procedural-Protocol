using UnityEngine;
using TMPro;

public class CollectibleItem : MonoBehaviour
{
    public ItemData data;
    public Transform player;

    private TextMeshPro textMesh;
    private float showDistance = 20f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = 1.5f;
            sc.isTrigger = true;
        }
        else
        {
            col.isTrigger = true;
        }

        GameObject textObj = new GameObject("ItemTooltip");
        textObj.transform.SetParent(this.transform);
        textObj.transform.localPosition = new Vector3(0, 2f, 0);

        textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 3;
        textMesh.text = GenerateTooltip();
        textMesh.enabled = false;

        FindPlayer();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 60f * Time.deltaTime, Space.World);
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * 2f) * 0.3f, 0);

        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= showDistance)
            {
                textMesh.enabled = true;

                textMesh.transform.LookAt(textMesh.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            }
            else
            {
                textMesh.enabled = false;
            }
        }
    }

    string GenerateTooltip()
    {
        string colorHex = "#FFFFFF"; // Common
        if (data.rarity == ItemRarity.Uncommon) colorHex = "#00FF00"; // Green
        if (data.rarity == ItemRarity.Rare) colorHex = "#0088FF";     // Blue
        if (data.rarity == ItemRarity.Epic) colorHex = "#A020F0";     // Purple
        if (data.rarity == ItemRarity.Legendary) colorHex = "#FFFF00"; // Yellow

        string text = $"<color={colorHex}>{data.itemName}\n[{data.rarity}]</color>\n";

        if (data.hpBoost > 0) text += $"<color=#ff4444>+{data.hpBoost} HP</color>\n";
        if (data.maxHpBoost > 0) text += $"<color=#ff0000>+{data.maxHpBoost} Max HP</color>\n";
        if (data.armorBoost > 0) text += $"<color=#aaaaaa>+{data.armorBoost} Armor</color>\n";
        if (data.damageBoost > 0) text += $"<color=#ffaa00>+{data.damageBoost} Dmg</color>\n";
        if (data.moveSpeedBoost > 0) text += $"<color=#00ff00>+{data.moveSpeedBoost} Speed</color>\n";
        if (data.dashForceBoost > 0) text += $"<color=#00ffff>+{data.dashForceBoost} Dash Force</color>\n";
        if (data.dashCooldownReduction > 0) text += $"<color=#ff00ff>-{data.dashCooldownReduction}s Dash CD</color>\n";

        return text;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            PlayerStats pStats = other.GetComponentInParent<PlayerStats>();
            PlayerMovement pMove = other.GetComponentInParent<PlayerMovement>();

            if (pStats != null)
            {
                PlayerData.MaxHP += data.maxHpBoost;
                pStats.currentMaxHP = PlayerData.MaxHP;

                PlayerData.HP += data.hpBoost;
                pStats.currentHP += data.hpBoost;

                if (pStats.currentHP > pStats.currentMaxHP) pStats.currentHP = pStats.currentMaxHP;

                PlayerData.Armor += data.armorBoost;
                PlayerData.Damage += data.damageBoost;
            }

            if (pMove != null)
            {
                pMove.moveSpeed += data.moveSpeedBoost;
                if (pMove.moveSpeed > 18f) pMove.moveSpeed = 18f;

                pMove.dashForce += data.dashForceBoost;
                if (pMove.dashForce > 60f) pMove.dashForce = 60f;

                pMove.dashCooldown -= data.dashCooldownReduction;
                if (pMove.dashCooldown < 0.3f) pMove.dashCooldown = 0.3f;
            }

            Debug.Log($"Item collected: {data.itemName} (Rarity: {data.rarity})");

            Destroy(gameObject);
        }
    }

    void FindPlayer()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }
}